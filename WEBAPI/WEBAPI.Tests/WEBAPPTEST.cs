using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using WEBAPI.Data;

namespace WEBAPI.Tests;

public class WEBAPPTEST : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Verwijder de SQL Server DbContext-registraties
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<PortfolioDBContext>) ||
                            d.ServiceType == typeof(IDbContextOptionsConfiguration<PortfolioDBContext>))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Voeg een geïsoleerde InMemory database toe per test-run
            services.AddDbContext<PortfolioDBContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }
}