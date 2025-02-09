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

        auth.MapGet(
            "denied",
            () =>
                Results.Problem(
                    title: "Access Denied",
                    detail: "Only the bot owner is permitted to log in.",
                    statusCode: 403
                )
        );

        return auth;
    }
}
