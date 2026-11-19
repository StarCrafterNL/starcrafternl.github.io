using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WEBAPI.Data;
using WEBAPI.Data.Entities;
using WEBAPI.Features.Blogposts;

namespace WEBAPI.Tests;

public class BLOGPOSTTEST
{
    [Fact]
    public async Task GetBlogposts_Publiek_Geeft200EnLijstTerug()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            db.BlogPosts.Add(new BlogPost
            {
                Titel = "Eerste Blog",
                Inhoud = "Inhoud van de blog",
                Publicatiedatum = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<BlogpostDto>>();
        Assert.NotNull(posts);
        Assert.Single(posts);
        Assert.Equal("Eerste Blog", posts[0].Titel);
    }

    [Fact]
    public async Task GetBlogpostById_BestaandEnOnbekendId_Geeft200En404()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        int bestaandId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var post = new BlogPost
            {
                Titel = "Specifieke Blog",
                Inhoud = "Meer tekst",
                Publicatiedatum = DateTime.UtcNow
            };
            db.BlogPosts.Add(post);
            await db.SaveChangesAsync();
            bestaandId = post.Id;
        }
        var client = factory.CreateClient();

        // Act
        var responseBestaand = await client.GetAsync($"/api/blogposts/{bestaandId}");
        var responseOnbekend = await client.GetAsync("/api/blogposts/99999");

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseBestaand.StatusCode);
        var dto = await responseBestaand.Content.ReadFromJsonAsync<BlogpostDto>();
        Assert.Equal("Specifieke Blog", dto!.Titel);

        Assert.Equal(HttpStatusCode.NotFound, responseOnbekend.StatusCode);
    }

    [Fact]
    public async Task PostBlogpost_ZonderLoginEnAlsGast_Geeft401En403()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var anoniemeClient = factory.CreateClient();
        var gastClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "gast@blog.nl", "Gast");

        var nieuweBlog = new
        {
            Titel = "Geweigerde Blog",
            Inhoud = "Mag niet geplaatst worden",
            Publicatiedatum = DateTime.UtcNow
        };

        // Act
        var response401 = await anoniemeClient.PostAsJsonAsync("/api/blogposts", nieuweBlog);
        var response403 = await gastClient.PostAsJsonAsync("/api/blogposts", nieuweBlog);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response401.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response403.StatusCode);
    }

    [Fact]
    public async Task PostBlogpost_AlsEditorEnAdmin_MaaktAanInDatabase()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var editorClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "editor@blog.nl", "Editor");
        var adminClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "admin@blog.nl", "Admin");

        // Act
        var resEditor = await editorClient.PostAsJsonAsync("/api/blogposts", new
        {
            Titel = "Editor Blog",
            Inhoud = "Geschreven door Editor",
            Publicatiedatum = DateTime.UtcNow
        });

        var resAdmin = await adminClient.PostAsJsonAsync("/api/blogposts", new
        {
            Titel = "Admin Blog",
            Inhoud = "Geschreven door Admin",
            Publicatiedatum = DateTime.UtcNow
        });

        // Assert
        Assert.Equal(HttpStatusCode.Created, resEditor.StatusCode);
        Assert.Equal(HttpStatusCode.Created, resAdmin.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
        Assert.Equal(2, db.BlogPosts.Count());
    }

    [Fact]
    public async Task PutEnDeleteBlogpost_AlsEditorEnAdmin_WijzigtEnVerwijdertInDatabase()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        int blogId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var post = new BlogPost
            {
                Titel = "Oude Blogtitel",
                Inhoud = "Oude Inhoud",
                Publicatiedatum = DateTime.UtcNow
            };
            db.BlogPosts.Add(post);
            await db.SaveChangesAsync();
            blogId = post.Id;
        }

        var editorClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "editor2@blog.nl", "Editor");
        var adminClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "admin2@blog.nl", "Admin");

        // Act - PUT als Editor
        var putResponse = await editorClient.PutAsJsonAsync($"/api/blogposts/{blogId}", new
        {
            Titel = "Aangepaste Blogtitel",
            Inhoud = "Aangepaste Inhoud",
            Publicatiedatum = DateTime.UtcNow
        });

        // Assert - PUT effect in database
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var bijgewerkt = await db.BlogPosts.FindAsync(blogId);
            Assert.Equal("Aangepaste Blogtitel", bijgewerkt!.Titel);
        }

        // Act - DELETE als Admin
        var deleteResponse = await adminClient.DeleteAsync($"/api/blogposts/{blogId}");

        // Assert - DELETE effect in database
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDBContext>();
            var verwijderd = await db.BlogPosts.FindAsync(blogId);
            Assert.Null(verwijderd);
        }
    }

    [Fact]
    public async Task PostBlogpost_OngeldigeInvoer_Geeft400BadRequest()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var adminClient = await AUTHTEST.MaakIngelogdeClientAsync(factory, "admin3@blog.nl", "Admin");

        // Act
        var response = await adminClient.PostAsync("/api/blogposts",
            new StringContent("{ ongeldige json }", System.Text.Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}