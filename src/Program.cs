using Discord;
using Discord.WebSocket;
using GamboCity_DiscordBot.Config;
using GamboCity_DiscordBot.Language;
using GamboCity_DiscordBot.Status;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GamboCity_DiscordBot;

public class Program {
    private static readonly DiscordSocketConfig SocketConfig = new() {
        GatewayIntents = GatewayIntents.AllUnprivileged & GatewayIntents.GuildScheduledEvents & GatewayIntents.GuildInvites,
    };

    private readonly IConfiguration Configuration;
    private readonly IServiceProvider Services;
    private readonly ILogger logger;

    public Program() {
        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        Services = new ServiceCollection()
            .AddLogging(configure => {
                configure.AddConsole();
            })
            .AddSingleton(Configuration)
            .AddSingleton(SocketConfig)
            .AddSingleton<ConfigHelper>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<DiscordStatus>()
            .BuildServiceProvider();

        logger = Services.GetRequiredService<ILogger<Program>>();

        DiscordSocketClient client = Services.GetRequiredService<DiscordSocketClient>();
        client.Log += LogAsync;
    }

    public static async Task Main(string[] _) {
        Program program = new();

        ConfigHelper configHelper = program.Services.GetRequiredService<ConfigHelper>();
        if (!configHelper.IsEnvironmentValid())
            Environment.Exit(1);

        LanguageHelper.SetLanguageFromConfiguration(program.Configuration);

        await program.Run();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task Run() {
        DiscordSocketClient client = Services.GetRequiredService<DiscordSocketClient>();

        DiscordStatus discordStatus = Services.GetRequiredService<DiscordStatus>();

        await client.LoginAsync(TokenType.Bot, Configuration["DISCORD_TOKEN"]);
        await client.StartAsync();
    }

    private Task LogAsync(LogMessage message) {
        logger.LogInformation(message.ToString());
        return Task.CompletedTask;
    }
}
