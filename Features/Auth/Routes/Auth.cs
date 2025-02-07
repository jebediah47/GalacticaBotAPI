using Microsoft.AspNetCore.Authentication;

namespace GalacticaBotAPI.Features.Auth.Routes;

public static class Auth
{
    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("auth");
        auth.MapGet(
            "login",
            () => Results.Challenge(new AuthenticationProperties { RedirectUri = "/" })
        );
        return auth;
    }
}
