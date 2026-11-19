using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WEBAPI.Tests;

public class AUTHTEST
{
    // Hulpmethode voor andere tests om snel als Gast, Editor of Admin in te loggen
    public static async Task<HttpClient> MaakIngelogdeClientAsync(
        WEBAPPTEST factory,
        string email,
        string rol)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });

        string wachtwoord = "TestWachtwoord123!";

        var regResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Wachtwoord = wachtwoord
        });
        regResponse.EnsureSuccessStatusCode();

        if (rol != "Gast")
        {
            using var scope = factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);
            await userManager.RemoveFromRoleAsync(user!, "Gast");
            await userManager.AddToRoleAsync(user!, rol);
        }

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Wachtwoord = wachtwoord
        });
        loginResponse.EnsureSuccessStatusCode();

        return client;
    }

    [Fact]
    public async Task Register_NieuweGebruiker_KrijgtStandaardRolGast()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var client = factory.CreateClient();
        var email = "nieuwgast@test.nl";

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Wachtwoord = "Welkom123!"
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var gebruiker = await userManager.FindByEmailAsync(email);
        Assert.NotNull(gebruiker);

        var rollen = await userManager.GetRolesAsync(gebruiker);
        Assert.Contains("Gast", rollen);
        Assert.DoesNotContain("Editor", rollen);
        Assert.DoesNotContain("Admin", rollen);
    }

    [Fact]
    public async Task Login_GeldigeGegevens_GeeftAuthenticatieCookieTerug()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var client = factory.CreateClient();
        var email = "loginuser@test.nl";
        var wachtwoord = "Welkom123!";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Wachtwoord = wachtwoord
        });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Wachtwoord = wachtwoord
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
        var cookies = response.Headers.GetValues("Set-Cookie");
        Assert.Contains(cookies, c => c.Contains(".AspNetCore.Identity.Application"));
    }

    [Fact]
    public async Task Login_OngeldigWachtwoord_Geeft401Unauthorized()
    {
        // Arrange
        using var factory = new WEBAPPTEST();
        var client = factory.CreateClient();
        var email = "foutlogin@test.nl";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Wachtwoord = "Welkom123!"
        });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Wachtwoord = "VerkeerdWachtwoord123!"
        });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}