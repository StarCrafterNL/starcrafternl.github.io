using Microsoft.EntityFrameworkCore;
using WEBAPI.Data;
using WEBAPI.Features.Projects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PortfolioDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapProjectEndpoints();

app.Run();