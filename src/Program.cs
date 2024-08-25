using Discord;
using Discord.WebSocket;
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

            if (!CheckEnvironmentValid())
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

        private bool CheckEnvironmentValid() {
            // Check if the DISCORD_TOKEN environment variable is set correctly
            string? discordToken = Configuration["DISCORD_TOKEN"];
            if (discordToken is null) {
                Console.WriteLine("DISCORD_TOKEN environment variable is not set.");
                return false;
            }
            if (discordToken == "REPLACE_ME") {
                Console.WriteLine("DISCORD_TOKEN environment variable is still default.");
                return false;
            }

            // Check if the FIVEM_URL environment variable is set correctly
            string? fivemUrl = Configuration["FIVEM_URL"];
            if (fivemUrl is null) {
                Console.WriteLine("FIVEM_URL environment variable is not set.");
                return false;
            }
            if (fivemUrl == "https://REPLACE_ME/dynamic.json") {
                Console.WriteLine("FIVEM_URL environment variable is still default.");
                return false;

            }

            // Check if the UPDATE_INTERVAL environment variable is set correctly
            string? updateInterval = Configuration["UPDATE_INTERVAL"];
            if (updateInterval is null) {
                Console.WriteLine("UPDATE_INTERVAL environment variable is not set.");
                return false;
            }
            if (!int.TryParse(updateInterval, out int updateIntervalNum)) {
                Console.WriteLine("UPDATE_INTERVAL environment variable is not a valid number.");
                return false;
            }
            if (!(updateIntervalNum >= 1)) {
                Console.WriteLine("UPDATE_INTERVAL environment variable must be greater than or equal to 1.");
                return false;
            }

            return true;
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
