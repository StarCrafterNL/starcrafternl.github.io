using Microsoft.EntityFrameworkCore;
using WEBAPI.Data;
using WEBAPI.Data.Entities;

namespace WEBAPI.Features.Blogposts;

public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/blogposts").WithTags("Blogposts");

        // 1. GET alle blogposts
        group.MapGet("/", async (PortfolioDBContext db) =>
        {
            var blogposts = await db.BlogPosts  
                .Select(b => new BlogpostDto
                {
                    Id = b.Id,
                    Titel = b.Titel,
                    Inhoud = b.Inhoud,
                    Publicatiedatum = b.Publicatiedatum
                })
                .ToListAsync();

            return Results.Ok(blogposts);
        });

        // 2. GET één blogpost op Id
        group.MapGet("/{id:int}", async (int id, PortfolioDBContext db) =>
        {
            var blogpost = await db.BlogPosts.FindAsync(id);

            if (blogpost is null)
            {
                return Results.NotFound();
            }

            var dto = new BlogpostDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                Publicatiedatum = blogpost.Publicatiedatum
            };

            return Results.Ok(dto);
        });

        // 3. POST nieuwe blogpost
        group.MapPost("/", async (CreateBlogpostDto dto, PortfolioDBContext db) =>
        {
            var blogpost = new BlogPost
            {
                Titel = dto.Titel,
                Inhoud = dto.Inhoud,
                Publicatiedatum = dto.Publicatiedatum
            };

            db.BlogPosts.Add(blogpost);
            await db.SaveChangesAsync();

            var resultDto = new BlogpostDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                Publicatiedatum = blogpost.Publicatiedatum
            };

            return Results.Created($"/api/blogposts/{blogpost.Id}", resultDto);
        });

        // 4. PUT blogpost op Id
        group.MapPut("/{id:int}", async (int id, UpdateBlogpostDto dto, PortfolioDBContext db) =>
        {
            var blogpost = await db.BlogPosts.FindAsync(id);

            if (blogpost is null)
            {
                return Results.NotFound();
            }

            blogpost.Titel = dto.Titel;
            blogpost.Inhoud = dto.Inhoud;
            blogpost.Publicatiedatum = dto.Publicatiedatum;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // 5. DELETE blogpost op Id
        group.MapDelete("/{id:int}", async (int id, PortfolioDBContext db) =>
        {
            var blogpost = await db.BlogPosts.FindAsync(id);

            if (blogpost is null)
            {
                return Results.NotFound();
            }

            db.BlogPosts.Remove(blogpost);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}