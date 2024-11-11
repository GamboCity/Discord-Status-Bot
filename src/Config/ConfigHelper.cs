using Microsoft.Extensions.Configuration;

namespace GamboCity_DiscordBot.Config;

public static class ConfigHelper {
    public static bool IsEnvironmentValid(IConfiguration configuration) {
        // Check if the DISCORD_TOKEN environment variable is set correctly
        string? discordToken = configuration["DISCORD_TOKEN"];
        if (discordToken is null) {
            Console.WriteLine("DISCORD_TOKEN environment variable is not set.");
            return false;
        }
        if (discordToken == "REPLACE_ME") {
            Console.WriteLine("DISCORD_TOKEN environment variable is still default.");
            return false;
        }

        // Check if the FIVEM_URL environment variable is set correctly
        string? fivemUrl = configuration["FIVEM_URL"];
        if (fivemUrl is null) {
            Console.WriteLine("FIVEM_URL environment variable is not set.");
            return false;
        }
        if (fivemUrl == "https://REPLACE_ME/dynamic.json") {
            Console.WriteLine("FIVEM_URL environment variable is still default.");
            return false;

        }

        // Check if the UPDATE_INTERVAL environment variable is set correctly
        string? updateInterval = configuration["UPDATE_INTERVAL"];
        if (updateInterval is null) {
            Console.WriteLine("UPDATE_INTERVAL environment variable is not set.");
            return false;
        }
        if (!int.TryParse(updateInterval, out int updateIntervalNum)) {
            Console.WriteLine("UPDATE_INTERVAL environment variable is not a valid number.");
            return false;
        }
        if (updateIntervalNum < 1) {
            Console.WriteLine("UPDATE_INTERVAL environment variable must be greater than or equal to 1.");
            return false;
        }

        return true;
    }
}
