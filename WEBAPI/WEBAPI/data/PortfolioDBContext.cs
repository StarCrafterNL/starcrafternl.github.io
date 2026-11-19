using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WEBAPI.Data.Entities;

namespace WEBAPI.Data;

public class PortfolioDBContext : IdentityDbContext<IdentityUser>
{
    public PortfolioDBContext(DbContextOptions<PortfolioDBContext> options) : base(options)
    {
    }

    public DbSet<Projects> Projects => Set<Projects>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
}