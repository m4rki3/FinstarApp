namespace FinstarApp.KafkaConsumer;

public readonly struct TaskInfo
{
    public Guid TaskId { get; init; }
    public TaskStatus? OldStatus { get; init; }
    public TaskStatus? NewStatus { get; init; }
    public DateTime UpdatedAt { get; init; }
    
    public TaskInfo(Guid taskId, TaskStatus? oldStatus, TaskStatus? newStatus, DateTime updatedAt)
    {
        TaskId = taskId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        UpdatedAt = updatedAt;
    }
}