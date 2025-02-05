namespace GalacticaBotAPI.Features.Auth.Routes;

public static class Auth
{
    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("auth");
        return auth;
    }
}
