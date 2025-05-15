using FinstarApp.KafkaConsumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();

app.Run();