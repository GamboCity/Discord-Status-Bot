using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GamboCity_DiscordBot.Config;

public class ConfigHelper(IConfiguration configuration, ILogger<ConfigHelper> logger) {
    public bool IsEnvironmentValid() {
        // Check if the DISCORD_TOKEN environment variable is set correctly
        string? discordToken = configuration["DISCORD_TOKEN"];
        if (discordToken is null) {
            logger.LogError("DISCORD_TOKEN environment variable is not set.");
            return false;
        }
        if (discordToken == "REPLACE_ME") {
            logger.LogError("DISCORD_TOKEN environment variable is still default.");
            return false;
        }

        // Check if the FIVEM_URL environment variable is set correctly
        string? fivemUrl = configuration["FIVEM_URL"];
        if (fivemUrl is null) {
            logger.LogError("FIVEM_URL environment variable is not set.");
            return false;
        }
        if (fivemUrl == "https://REPLACE_ME/dynamic.json") {
            logger.LogError("FIVEM_URL environment variable is still default.");
            return false;

        }

        // Check if the UPDATE_INTERVAL environment variable is set correctly
        string? updateInterval = configuration["UPDATE_INTERVAL"];
        if (updateInterval is null) {
            logger.LogError("UPDATE_INTERVAL environment variable is not set.");
            return false;
        }
        if (!int.TryParse(updateInterval, out int updateIntervalNum)) {
            logger.LogError("UPDATE_INTERVAL environment variable is not a valid number.");
            return false;
        }
        if (updateIntervalNum < 1) {
            logger.LogError("UPDATE_INTERVAL environment variable must be greater than or equal to 1.");
            return false;
        }

        return true;
    }
}
