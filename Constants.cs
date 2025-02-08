namespace GalacticaBotAPI;

public static class Constants
{
    public static string DiscordClientId { get; private set; }
    public static string DiscordClientSecret { get; private set; }
    public static string GalacticaBotToken { get; private set; }
    public static string GalacticaBotApiBaseUrl { get; private set; }
    public const string DiscordApiBaseUrl = "https://discord.com/api/v10/";

    public static void LoadConfiguration(IConfiguration configuration)
    {
        DiscordClientId = configuration["Discord:ClientId"];
        DiscordClientSecret = configuration["Discord:ClientSecret"];
        GalacticaBotApiBaseUrl = configuration["GalacticaBot:ApiBaseUrl"];
        GalacticaBotToken = configuration["GalacticaBot:Token"];
        ValidateConstants();
    }

    private static void ValidateConstants()
    {
        var properties = typeof(Constants).GetProperties(
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
        );
        foreach (var property in properties)
        {
            var value = property.GetValue(null) as string;
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"{property.Name} is not configured.");
            }
        }
    }
}
