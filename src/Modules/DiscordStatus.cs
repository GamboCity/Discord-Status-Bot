using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;

namespace GamboCity_DiscordBot.src.Modules {
    internal class DiscordStatus(DiscordSocketClient client, IConfiguration config, ILogger<DiscordStatus> logger) {
        private readonly HttpClient httpclient = new();

        public Task InitializeAsync() {
            client.Ready += UpdatePlayerStats;

            return Task.CompletedTask;
        }

        private async Task UpdatePlayerStats() {
            while (true) {
                try {
                    JObject responseJObject = JObject.Parse(await httpclient.GetStringAsync(config["FIVEM_URL"]));

                    if (ExtractInfo(responseJObject, out JToken? players, out JToken? maxPlayers)) {
                        Game activity = new(
                        string.Format(Language.Language.PRESENCE_TEXT, players, maxPlayers),
                        ActivityType.Watching);

                        await client.SetActivityAsync(activity);
                    }
                } catch (HttpRequestException ex) {
                    NotifyHttpRequestException(ex);
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(
                        Convert.ToInt32(
                            config["UPDATE_INTERVAL"])));
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

            if(!encounteredError) 
                return true;

            if(config["DEBUG"] == "1")
                logger.LogError($"Http response received: {responseJObject}");
            else
                logger.LogError("To view the http response responsible for this error set DEBUG=1");

            return false;
        }

        private void NotifyHttpRequestException(HttpRequestException ex) {
            logger.LogError($"Error while updating stats: {ex.Message}");
            logger.LogError($"Your url \"{config["FIVEM_URL"]}\" is likely faulty.");
            //if (!HttpStatusCode.BadGateway.Equals(ex.StatusCode))
            //    //throw;
        }
    }
}
