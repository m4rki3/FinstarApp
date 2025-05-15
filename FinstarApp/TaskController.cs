using System.Text.Json;
using FinstarApp.Domain.Core;
using FinstarApp.Domain.Interfaces;
using FinstarApp.Dto;
using FinstarApp.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace FinstarApp;

[ApiController]
[Route("api/[controller]/[action]")]
public class TaskController : ControllerBase
{
    private readonly IRepository<TaskEntity> _repository;
    private readonly ILogger<TaskController> _logger;
    private readonly IKafkaProducerService _producer;
    private readonly string? _kafkaTopic;
    
    public TaskController(
        IRepository<TaskEntity> repository, ILogger<TaskController> logger, IKafkaProducerService producer,
        IConfiguration config
    )
    {
        _repository = repository;
        _logger = logger;
        _producer = producer;
        
        _kafkaTopic = config["Kafka:Topic"];

        if (_kafkaTopic is null)
        {
            _logger.LogWarning("Kafka configuration is missing or empty");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskDto dto)
    {
        if (IsValid(dto) is false)
        {
            _logger.LogError("Task is not valid");
            return BadRequest("Task is not valid");
        }

        var entity = TaskEntity.CreateNew(dto.Title, dto.Description);
        var isEntityAdded = await _repository.TryAddAsync(entity);

        if (isEntityAdded is false)
        {
            _logger.LogError("Failed to add Task");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (_kafkaTopic is not null)
        {
            var info = new TaskInfo(entity.Id, oldStatus: null, newStatus: entity.Status, updatedAt: DateTime.Now);
            var message = JsonSerializer.Serialize(info);
            await _producer.ProduceAsync(_kafkaTopic, message);
        }
        
        return Ok(entity);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var entity = await _repository.GetAsync(id);
        
        if (entity is null)
        {
            _logger.LogError("Task is not found");
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var entities = await _repository.GetAllAsync();

        if (entities.Any() is false)
        {
            _logger.LogWarning("Tasks are not found");
        }
        
        return Ok(entities);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TaskDto dto)
    {
        if (IsValid(dto) is false)
        {
            _logger.LogError("Task is not valid");
            return BadRequest("Task is not valid");
        }
        var entity = await _repository.GetAsync(dto.Id);

        if (entity is null)
        {
            _logger.LogError("Task is not found");
            return NotFound();
        }
        
        var oldStatus = entity.Status;
        entity.Update(dto.Title, dto.Description, dto.Status);

        var isEntityUpdated = await _repository.TryUpdateAsync(entity);

        if (isEntityUpdated is false)
        {
            _logger.LogError("Failed to update Task");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        if (_kafkaTopic is not null)
        {
            var info = new TaskInfo(
                entity.Id, oldStatus, newStatus: dto.Status, updatedAt: DateTime.Now
            );
            var message = JsonSerializer.Serialize(info);
            
            await _producer.ProduceAsync(_kafkaTopic, message);
        }
        
        return Ok();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await _repository.GetAsync(id);

        if (entity is null)
        {
            _logger.LogError("Task is not found");
            return NotFound();
        }

        var isEntityRemoved = await _repository.TryRemoveAsync(id);

        if (isEntityRemoved is false)
        {
            _logger.LogError("Failed to remove Task");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (_kafkaTopic is not null)
        {
            var info = new TaskInfo(entity.Id, oldStatus: entity.Status, newStatus: null, updatedAt: DateTime.Now);
            var message = JsonSerializer.Serialize(info);
            await _producer.ProduceAsync(_kafkaTopic, message);
        }
        
        return Ok();
    }

    private bool IsValid(TaskDto task)
    {
        // todo валидация
        return true;
    }
}