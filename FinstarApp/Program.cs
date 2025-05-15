using FinstarApp.Domain.Core;
using FinstarApp.Domain.Interfaces;
using FinstarApp.Infrastructure.Data;
using FinstarApp.Infrastructure.Data.Configurations;
using FinstarApp.Kafka;
using Microsoft.EntityFrameworkCore;
using AppContext = FinstarApp.Infrastructure.Data.AppContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEntityTypeConfiguration<TaskEntity>, TaskConfiguration>();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DbContext, AppContext>(options => options.UseNpgsql(connection));

builder.Services.AddSingleton<IRepository<TaskEntity>, Repository<TaskEntity>>();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

builder.Services.AddControllers();


var app = builder.Build();
app.MapControllers();