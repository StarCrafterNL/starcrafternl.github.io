using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WEBAPI.Data;
using WEBAPI.Data.Entities;
using WEBAPI.Features.Projects;

namespace WEBAPI.Tests;

public class PROJECTTEST
{
    [Fact]
    public async Task GetProjects_Publiek_Geeft200EnLijstTerug()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            db.Projects.Add(new Projects
            {
                Titel = "Test Project",
                Beschrijving = "Beschrijving",
                Categorie = "Web",
                GitHubUrl = "https://github.com/test",
                Datum = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
        Assert.NotNull(projects);
        Assert.Single(projects);
        Assert.Equal("Test Project", projects[0].Titel);
    }

    [Fact]
    public async Task GetProjectById_BestaandEnOnbekendId_Geeft200En404()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        int bestaandId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var project = new Projects
            {
                Titel = "Specifiek Project",
                Beschrijving = "Details",
                Categorie = "API",
                GitHubUrl = "https://github.com/specifiek",
                Datum = DateTime.UtcNow
            };
            db.Projects.Add(project);
            await db.SaveChangesAsync();
            bestaandId = project.Id;
        }
        var client = factory.CreateClient();

        // Act
        var responseBestaand = await client.GetAsync($"/api/projects/{bestaandId}");
        var responseOnbekend = await client.GetAsync("/api/projects/99999");

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseBestaand.StatusCode);
        var dto = await responseBestaand.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.Equal("Specifiek Project", dto!.Titel);

        Assert.Equal(HttpStatusCode.NotFound, responseOnbekend.StatusCode);
    }

    [Fact]
    public async Task PostProject_ZonderLoginEnAlsGast_Geeft401En403()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var anoniemeClient = factory.CreateClient();
        var gastClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "gast@proj.nl", "Gast");

        var nieuwProject = new
        {
            Titel = "Ongeautoriseerd",
            Beschrijving = "Mag niet",
            Categorie = "Test",
            GitHubUrl = "https://github.com/nee",
            Datum = DateTime.UtcNow
        };

        // Act
        var response401 = await anoniemeClient.PostAsJsonAsync("/api/projects", nieuwProject);
        var response403 = await gastClient.PostAsJsonAsync("/api/projects", nieuwProject);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response401.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response403.StatusCode);
    }

    [Fact]
    public async Task PostProject_AlsEditorEnAdmin_MaaktAanInDatabase()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var editorClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "editor@proj.nl", "Editor");
        var adminClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "admin@proj.nl", "Admin");

        // Act
        var resEditor = await editorClient.PostAsJsonAsync("/api/projects", new
        {
            Titel = "Editor Project",
            Beschrijving = "Gemaakt door Editor",
            Categorie = "Backend",
            GitHubUrl = "https://github.com/editor",
            Datum = DateTime.UtcNow
        });

        var resAdmin = await adminClient.PostAsJsonAsync("/api/projects", new
        {
            Titel = "Admin Project",
            Beschrijving = "Gemaakt door Admin",
            Categorie = "Fullstack",
            GitHubUrl = "https://github.com/admin",
            Datum = DateTime.UtcNow
        });

        // Assert
        Assert.Equal(HttpStatusCode.Created, resEditor.StatusCode);
        Assert.Equal(HttpStatusCode.Created, resAdmin.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
        Assert.Equal(2, db.Projects.Count());
        Assert.Contains(db.Projects, p => p.Titel == "Editor Project");
        Assert.Contains(db.Projects, p => p.Titel == "Admin Project");
    }

    [Fact]
    public async Task PutEnDeleteProject_AlsEditorEnAdmin_WijzigtEnVerwijdertInDatabase()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        int projectId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var project = new Projects
            {
                Titel = "Oude Titel",
                Beschrijving = "Oude Beschrijving",
                Categorie = "Web",
                GitHubUrl = "https://github.com/oud",
                Datum = DateTime.UtcNow
            };
            db.Projects.Add(project);
            await db.SaveChangesAsync();
            projectId = project.Id;
        }

        var editorClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "editor2@proj.nl", "Editor");
        var adminClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "admin2@proj.nl", "Admin");

        // Act - PUT als Editor
        var putResponse = await editorClient.PutAsJsonAsync($"/api/projects/{projectId}", new
        {
            Titel = "Nieuwe Titel",
            Beschrijving = "Nieuwe Beschrijving",
            Categorie = "Web",
            GitHubUrl = "https://github.com/nieuw",
            Datum = DateTime.UtcNow
        });

        // Assert - PUT effect in database
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var bijgewerkt = await db.Projects.FindAsync(projectId);
            Assert.Equal("Nieuwe Titel", bijgewerkt!.Titel);
        }

        // Act - DELETE als Admin
        var deleteResponse = await adminClient.DeleteAsync($"/api/projects/{projectId}");

        // Assert - DELETE effect in database
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var verwijderd = await db.Projects.FindAsync(projectId);
            Assert.Null(verwijderd);
        }
    }

    [Fact]
    public async Task PostProject_OngeldigeInvoer_Geeft400BadRequest()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var editorClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "editor3@proj.nl", "Editor");

        // Act (lege of ongeldige JSON sturen)
        var response = await editorClient.PostAsync("/api/projects",
            new StringContent("{ ongeldige json }", System.Text.Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}