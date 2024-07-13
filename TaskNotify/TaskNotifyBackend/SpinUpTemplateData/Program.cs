using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace SpinUpTemplateData
{
    internal class Program
    {

        #region Scripts
        private static string CREATE_USERS_TABLE = """
                        CREATE TABLE users (
                        UserName VARCHAR(20) PRIMARY KEY,
                        Password VARCHAR(250) NOT NULL
                    );
                    """;
        private static string CREATE_PROCEDURE_GET_USER_BY_NAME = """
                   CREATE PROCEDURE get_user_by_name
                       @p_username VARCHAR(20)
                   AS
                   BEGIN
                       SELECT UserName, Password
                       FROM users
                       WHERE UserName = @p_username;
                   END;
                   """;

        private static string CREATE_TASKS_TABLE = """
                        CREATE TABLE tasks (
                        TaskId uniqueidentifier NOT NULL PRIMARY KEY,
                        TaskName VARCHAR(255) NOT NULL,
                        UserName VARCHAR(20) NOT NULL,
                        RecordDate DATETIME NOT NULL DEFAULT GETDATE()
                    );
                    """;
        private static string CREATE_PROCEDURE_GET_TASKS_BY_USER = """"
                CREATE PROCEDURE get_tasks_by_user_name
                    @p_user_name VARCHAR(20)
                AS
                BEGIN
                    SELECT TaskId, TaskName, UserName, RecordDate
                    FROM tasks
                    WHERE UserName = @p_user_name;
                END;
                """";
        private static string CREATE_PROCEDURE_INSERT_USER_TASK = """"
                CREATE PROCEDURE insert_user_task
                    @p_task_id UNIQUEIDENTIFIER,
                    @p_task_name VARCHAR(255),
                    @p_user_name VARCHAR(20),
                    @p_record_date DATETIME
                AS
                BEGIN
                    INSERT INTO tasks (TaskId, TaskName, UserName, RecordDate)
                    VALUES (@p_task_id, @p_task_name, @p_user_name, @p_record_date);
                END;
                """";
        #endregion

        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var configuration = Configure();
            
            // pre-defined users...
            var spongeBobPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("spongebob123", 13);
            var patricStarPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("patricstar123", 13);

            var connectionString = configuration["SpinUpSettings:MSSQLDb:ConnectionString"];
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        #region Users
                        await connection.ExecuteAsync(CREATE_USERS_TABLE, transaction: transaction);
                        await connection.ExecuteAsync(@" insert into users (UserName,Password) values ('PatricStar',@PatricStarPassword);", new { PatricStarPassword = patricStarPassword }, transaction: transaction);
                        await connection.ExecuteAsync(@" insert into users (UserName,Password) values ('SpongeBob', @SpongeBobPassword);", new { SpongeBobPassword = spongeBobPassword }, transaction: transaction);
                        await connection.ExecuteAsync(CREATE_PROCEDURE_GET_USER_BY_NAME, transaction: transaction);
                        
                        Console.WriteLine("created 'users' table with users as PatricStar (pw: patricstar123) and SpongeBob (pw: spongebob123) ");
                        #endregion

                        #region Tasks
                        await connection.ExecuteAsync(CREATE_TASKS_TABLE, transaction: transaction);
                        await connection.ExecuteAsync(CREATE_PROCEDURE_GET_TASKS_BY_USER, transaction: transaction);
                        await connection.ExecuteAsync(CREATE_PROCEDURE_INSERT_USER_TASK, transaction: transaction);

                        Console.WriteLine("created 'tasks' table with procedures insert_user_task, get_tasks_by_user_name");
                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Transaction Rolled Back, cause of Exception:", ex.ToString());
                    }
                }
            }

            Console.ResetColor();
        }

        private static IConfiguration Configure()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            return builder.Build();
        }
    }
}
