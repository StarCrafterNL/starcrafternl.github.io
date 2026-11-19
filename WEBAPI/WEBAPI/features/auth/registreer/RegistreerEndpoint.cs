using Microsoft.AspNetCore.Identity;

namespace WEBAPI.Features.Auth;

public record RegistreerRequest(string Email, string Wachtwoord);

public static class RegistreerEndpoint
{
    public static void MapRegistreerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            RegistreerRequest request,
            UserManager<IdentityUser> userManager) =>
        {
            var gebruiker = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var resultaat = await userManager.CreateAsync(gebruiker, request.Wachtwoord);

            if (!resultaat.Succeeded)
            {
                return Results.BadRequest(resultaat.Errors);
            }

            // Wijs elke nieuwe geregistreerde gebruiker automatisch de rol 'Gast' toe
            await userManager.AddToRoleAsync(gebruiker, "Gast");

            return Results.Ok(new { Bericht = "Registratie geslaagd. Rol 'Gast' toegekend." });
        });
    }
}