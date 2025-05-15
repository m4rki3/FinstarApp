using FinstarApp.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskStatus = FinstarApp.Domain.Core.TaskStatus;

namespace FinstarApp.Infrastructure.Data.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("Tasks");
        
        builder.HasKey(task => task.Id)
               .HasName("PK_Tasks_Id");

        builder.Property(task => task.Title)
               .IsRequired()
               .HasColumnType("varchar(30)");
        
        builder.Property(task => task.Description)
               .IsRequired()
               .HasColumnType("varchar(50)");

        builder.Property(task => task.Status)
               .IsRequired()
               .HasColumnType("varchar(20)")
               .HasConversion(
                   status => status.ToString(),
                   status => (TaskStatus)Enum.Parse(typeof(TaskStatus), status)
               );

        builder.Property(task => task.CreatedAt)
               .IsRequired()
               .HasColumnType("datetime2(0)");
        
        builder.Property(task => task.UpdatedAt)
               .IsRequired()
               .HasColumnType("datetime2(0)");
    }
}