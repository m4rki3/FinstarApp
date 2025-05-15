namespace FinstarApp.Domain.Core;

public class TaskEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public TaskEntity(
        Guid id, string title, string description, TaskStatus status, DateTime createdAt, DateTime updatedAt
    )
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static TaskEntity CreateNew(string title, string description)
    {
        return new TaskEntity(
            Guid.NewGuid(), title, description, TaskStatus.Created, DateTime.Now, DateTime.Now
        );
    }

    public void Update(string? title = null, string? description = null, TaskStatus? status = null)
    {
        Title = title ?? Title;
        Description = description ?? Description;
        Status = status ?? Status;
        UpdatedAt = DateTime.Now;
    }
}