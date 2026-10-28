using Microsoft.EntityFrameworkCore;
using WEBAPI.Data;
using WEBAPI.Data.Entities;

namespace WEBAPI.Features.Projects;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/projects").WithTags("Projects");

        // 1. GET alle projecten
        group.MapGet("/", async (PortfolioDBContext db) =>
        {
            var projects = await db.Projects
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Titel = p.Titel,
                    Beschrijving = p.Beschrijving,
                    Categorie = p.Categorie,
                    GitHubUrl = p.GitHubUrl,
                    Datum = p.Datum
                })
                .ToListAsync();

            return Results.Ok(projects);
        });

        // 2. GET één project op Id
        group.MapGet("/{id:int}", async (int id, PortfolioDBContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
            {
                return Results.NotFound();
            }

            var dto = new ProjectDto
            {
                Id = project.Id,
                Titel = project.Titel,
                Beschrijving = project.Beschrijving,
                Categorie = project.Categorie,
                GitHubUrl = project.GitHubUrl,
                Datum = project.Datum
            };

            return Results.Ok(dto);
        });

        // 3. POST nieuw project (aanmaken)
        group.MapPost("/", async (CreateProjectDto dto, PortfolioDBContext db) =>
        {
            var project = db.Projects.Add(new()
            {
                Titel = dto.Titel,
                Beschrijving = dto.Beschrijving,
                Categorie = dto.Categorie,
                GitHubUrl = dto.GitHubUrl,
                Datum = dto.Datum
            }).Entity;

            await db.SaveChangesAsync();

            var resultDto = new ProjectDto
            {
                Id = project.Id,
                Titel = project.Titel,
                Beschrijving = project.Beschrijving,
                Categorie = project.Categorie,
                GitHubUrl = project.GitHubUrl,
                Datum = project.Datum
            };

            return Results.Created($"/api/projects/{project.Id}", resultDto);
        });

        // 4. PUT project op Id (wijzigen)
        group.MapPut("/{id:int}", async (int id, UpdateProjectDto dto, PortfolioDBContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
            {
                return Results.NotFound();
            }

            project.Titel = dto.Titel;
            project.Beschrijving = dto.Beschrijving;
            project.Categorie = dto.Categorie;
            project.GitHubUrl = dto.GitHubUrl;
            project.Datum = dto.Datum;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // 5. DELETE project op Id (verwijderen)
        group.MapDelete("/{id:int}", async (int id, PortfolioDBContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
            {
                return Results.NotFound();
            }

            db.Projects.Remove(project);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}