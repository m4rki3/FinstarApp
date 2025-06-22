using TaskStatus = FinstarApp.Domain.Core.TaskStatus;

namespace FinstarApp.Dto;

public record TaskDto
{
    public Guid Id { get; }

    public string Title { get; }

    public string Description { get; }

    public TaskStatus Status { get; }

    public TaskDto(Guid id, string title, string description, TaskStatus status)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
    }
}