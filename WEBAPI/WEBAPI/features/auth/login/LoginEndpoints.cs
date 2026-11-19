using Microsoft.AspNetCore.Identity;

namespace WEBAPI.Features.Auth;

public record LoginRequest(string Email, string Wachtwoord);

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            SignInManager<IdentityUser> signInManager) =>
        {
            var resultaat = await signInManager.PasswordSignInAsync(
                userName: request.Email,
                password: request.Wachtwoord,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!resultaat.Succeeded)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new { Bericht = "Succesvol ingelogd." });
        });
    }
}