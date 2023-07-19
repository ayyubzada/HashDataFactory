using DataGeneratorApi.Services;
using DataGeneratorApi.Services.Abstractions;
using DataPersistenceLayer.Repositories;
using DataPersistenceLayer.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var rabbitMqSection = builder.Configuration.GetSection(RabbitMqConfiguration.ConfigName);
builder.Services.Configure<RabbitMqConfiguration>(rabbitMqSection);

builder.Services.AddScoped<IHashRecordRepository, HashRecordRepository>();
builder.Services.AddScoped<IHashService, HashService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
