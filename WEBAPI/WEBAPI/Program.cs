using Microsoft.EntityFrameworkCore;
using WEBAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext registreren met de SQL Server connection string via Dependency Injection
builder.Services.AddDbContext<PortfolioDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Hello World endpoint
app.MapGet("/", () => "Hello World!")
    .WithName("GetHelloWorld");

app.Run();