using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GamboCity_DiscordBot.Status;

public class DiscordStatus : IDisposable {
    private readonly HttpClient httpclient = new();
    private readonly DiscordSocketClient client;
    private readonly IConfiguration config;
    private readonly ILogger<DiscordStatus> logger;

    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();

    public DiscordStatus(DiscordSocketClient client, IConfiguration config, ILogger<DiscordStatus> logger) {
        this.client = client;
        this.config = config;
        this.logger = logger;

        _timer = new(
            TimeSpan.FromSeconds(
                Convert.ToInt32(
                    config["UPDATE_INTERVAL"])));

        client.Ready += OnClientReady;
    }

    private Task OnClientReady() {
        Start();

        return Task.CompletedTask;
    }

    #region IDisposable Support
    private void Dispose(bool disposing) {
        if (disposing) {
            _ = StopAsync();
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DiscordStatus() {
        Dispose(false);
    }
    #endregion

    #region Timer
    public void Start() {
        logger.LogInformation("Starting DiscordStatus");

        UpdatePlayerStats().Wait();

        _timerTask = DoWorkAsync();
    }

    private async Task DoWorkAsync() {
        try {
            while (await _timer.WaitForNextTickAsync(_cts.Token)) {
                await UpdatePlayerStats();
            }
        } catch(OperationCanceledException) {
        }
    }

    private async Task StopAsync() {
        logger.LogInformation("Stopping DiscordStatus");
        if (_timerTask is null)
            return;

        _cts.Cancel();
        await _timerTask;
        _cts.Dispose();
    }
    #endregion

    #region Discord Status
    private async Task UpdatePlayerStats() {
        try {
            JObject responseJObject = JObject.Parse(await httpclient.GetStringAsync(config["FIVEM_URL"]));

            if (ExtractInfo(responseJObject, out JToken? players, out JToken? maxPlayers)) {
                Game activity = new(
                    string.Format(Language.Language.PRESENCE_TEXT, players, maxPlayers),
                    ActivityType.Watching);

                await client.SetActivityAsync(activity);
                logger.LogInformation($"Setting activity to: {activity}");
            }
        } catch (HttpRequestException ex) {
            NotifyHttpRequestException(ex);
        }
    }

    private bool ExtractInfo(JObject responseJObject, out JToken? players, out JToken? maxPlayers) {
        players = responseJObject["clients"];
        maxPlayers = responseJObject["sv_maxclients"];

        bool encounteredError = false;

        if (players is null) {
            encounteredError = true;
            logger.LogError("Error while updating stats: players is null");
        }
        if (maxPlayers is null) {
            encounteredError = true;
            logger.LogError("Error while updating stats: maxPlayers is null");
        }

        if (!encounteredError)
            return true;

        if (config["DEBUG"] == "1")
            logger.LogError($"Http response received: {responseJObject}");
        else
            logger.LogError("To view the http response responsible for this error set DEBUG=1");

        return false;
    }

    private void NotifyHttpRequestException(HttpRequestException ex) {
        logger.LogError($"Error while updating stats: {ex.Message}");
        logger.LogError($"Your url \"{config["FIVEM_URL"]}\" is likely faulty.");
    }
    #endregion
}