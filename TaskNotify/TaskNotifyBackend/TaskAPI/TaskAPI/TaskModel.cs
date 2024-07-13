namespace TaskAPI
{
    public class TaskModel
    {
        public Guid TaskId { get; set; } = Guid.NewGuid();
        public string? TaskName { get; set; }
        public string? UserName { get; set; }
        public int TaskProcessTimeInSeconds { get; set; }
    }
}
