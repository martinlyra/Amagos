using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    public class ServerConfigService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly DiscordSocketClient socketClient;
        private readonly LoggerService logger;

        private readonly Dictionary<ulong, ServerConfig> serverConfigDictionary;

        public ServerConfigService(IServiceProvider serviceProvider, DiscordSocketClient socketClient, LoggerService logger)
        {
            this.serviceProvider = serviceProvider;
            this.socketClient = socketClient;
            this.logger = logger;

            logger.Log(new LogMessage(LogSeverity.Info, "ServerConfigService", "Service up and running!"));

            serverConfigDictionary = new Dictionary<ulong, ServerConfig>();

            socketClient.GuildAvailable += OnGuildAvailable;
            socketClient.JoinedGuild += OnGuildJoined;
        }

        public ServerConfig GetConfig(SocketGuild guild) => GetConfig(guild.Id);
        public ServerConfig GetConfig(ulong serverId) => serverConfigDictionary[serverId];

        public async Task ModifyConfig(ulong serverId, Action<ServerConfig> modifier)
        {
            var serverConfig = serverConfigDictionary[serverId];
            modifier(serverConfig);
            await SaveConfig(serverId, serverConfig);
        }

        public Task SaveConfig(SocketGuild guild, ServerConfig serverConfig, string path = null, string file = null) => SaveConfig(guild.Id, serverConfig, path, file);
        public async Task SaveConfig(ulong serverId, ServerConfig serverConfig, string path = null, string file = null)
        {
            if(path is null)
                path = $"Config/Guilds/{serverId}";
            if(file is null)
                file = "settings.json";

            var jsonText = JsonConvert.SerializeObject(serverConfig, Formatting.Indented);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            using (StreamWriter writer = File.CreateText($"{path}/{file}"))
               await writer.WriteLineAsync(jsonText);
        }

        private async Task OnGuildJoined(SocketGuild arg)
        {
            await LoadGuildConfig(arg);
        }

        private async Task OnGuildAvailable(SocketGuild arg)
        {
            await LoadGuildConfig(arg);
        }

        private async Task LoadGuildConfig(SocketGuild guild)
        {
            var path = $"Config/Guilds/{guild.Id}";
            var file = "settings.json";

            ServerConfig serverConfig;

            if (Directory.Exists(path) && File.Exists($"{path}/{file}"))
            {
                var jsonText = File.ReadAllText($"{path}/{file}");
                serverConfig = JsonConvert.DeserializeObject<ServerConfig>(jsonText);

                await logger.Log(new LogMessage(LogSeverity.Info, "ServerConfigService", $"Loaded JSON for guild-ID {guild.Id}"));
            }
            else
            {
                serverConfig = new ServerConfig();

                await Task.WhenAll(
                    SaveConfig(guild, serverConfig, path, file),
                    logger.Log(new LogMessage(LogSeverity.Info, "ServerConfigService", $"Created a new JSON for guild-ID {guild.Id}, as one was missing"))
                    );
            }

            serverConfigDictionary.Add(guild.Id, serverConfig);
        }
    }
}
