
using SignalRNotifyAPI.Hubs;

namespace SignalRNotifyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(config["CorsSettings:AllowedOrigins"]!).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                    });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<KafkaConsumerService>();
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<TaskNotifyHub>("/taskNotifyHub");

            app.Run();
        }
    }
}
