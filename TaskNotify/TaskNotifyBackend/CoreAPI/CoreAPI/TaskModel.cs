namespace CoreAPI
{
    public class TaskModel
    {
        public Guid TaskId { get; set; }
        public string? TaskName { get; set; }
        public string? UserName { get; set; }
        public int TaskProcessTimeInSeconds { get; set; }
    }
}
