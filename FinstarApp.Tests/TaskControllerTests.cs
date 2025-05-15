using FinstarApp.Domain.Core;
using FinstarApp.Domain.Interfaces;
using FinstarApp.Dto;
using FinstarApp.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TaskStatus = FinstarApp.Domain.Core.TaskStatus;

namespace FinstarApp.Tests;

[TestClass]
public class TaskControllerTests
{
    [TestMethod]
    public async Task Create_WhenRepoCorrect_ReturnsOk()
    {
        var repo = Substitute.For<IRepository<TaskEntity>>();
        repo.TryAddAsync(Arg.Any<TaskEntity>()).Returns(true);
        
        var logger = Substitute.For<ILogger<TaskController>>();
        var producer = Substitute.For<IKafkaProducerService>();
        var config = Substitute.For<IConfiguration>();
        
        var controller = new TaskController(repo, logger, producer, config);
        var dto = new TaskDto(Guid.Empty, "title", "description", TaskStatus.Created);

        var result = await controller.Create(dto);
        
        Assert.IsInstanceOfType<OkObjectResult>(result);
        Assert.IsInstanceOfType<TaskEntity>((result as OkObjectResult)?.Value);
    }
    
    [TestMethod]
    public async Task Create_WhenRepoIncorrect_Returns500()
    {
        var repo = Substitute.For<IRepository<TaskEntity>>();
        repo.TryAddAsync(Arg.Any<TaskEntity>()).Returns(false);
        
        var logger = Substitute.For<ILogger<TaskController>>();
        var producer = Substitute.For<IKafkaProducerService>();
        var config = Substitute.For<IConfiguration>();
        
        var controller = new TaskController(repo, logger, producer, config);
        var dto = new TaskDto(Guid.Empty, "title", "description", TaskStatus.Created);

        var result = await controller.Create(dto);
        
        Assert.IsInstanceOfType<StatusCodeResult>(result);
        Assert.AreEqual((result as StatusCodeResult)?.StatusCode, 500);
    }
}