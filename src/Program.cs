using Discord;
using Discord.WebSocket;
using GamboCity_DiscordBot.Config;
using GamboCity_DiscordBot.src.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace GamboCity_DiscordBot.src {
    public class Program {
        private static readonly DiscordSocketConfig SocketConfig = new() {
            GatewayIntents = GatewayIntents.AllUnprivileged & GatewayIntents.GuildScheduledEvents & GatewayIntents.GuildInvites,
        };

        private readonly IConfiguration Configuration;

        private readonly IServiceProvider Services;

        public Program() {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            if (!ConfigHelper.IsEnvironmentValid(Configuration))
                Environment.Exit(1);

            Services = new ServiceCollection()
                .AddSingleton(Configuration)
                .AddSingleton(SocketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<DiscordStatus>()
                .BuildServiceProvider();
        }

        public static async Task Main(string[] _) {
            Program program = new();

            SetLanguage(program.Configuration["LANGUAGE"]!);

            await program.InitializeDiscordClient();

            await Task.Delay(Timeout.Infinite);
        }

        private static void SetLanguage(string language) {
            CultureInfo cultureInfo = new(language);

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        private async Task InitializeDiscordClient() {
            DiscordSocketClient client = Services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;

            await InitializeServices();

            await client.LoginAsync(TokenType.Bot, Configuration["DISCORD_TOKEN"]);
            await client.StartAsync();
        }

        private Task LogAsync(LogMessage message) {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private async Task InitializeServices() {
            await Services.GetRequiredService<DiscordStatus>()
                .InitializeAsync();
        }
    }
}
