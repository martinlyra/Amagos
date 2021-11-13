using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Amagos
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public struct TokenPad {
            public string TokenValue { internal get; set; }
        }

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly LoggerService _logger;
        private readonly IServiceProvider _services;

        private Program()
        {
            var config = new Configuration()
                .Require("Config/token.json");

            config.CheckFiles();

            _client = new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 30
                }
                );

            _commands = new CommandService(
                new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Info,
                });

            _services = new Initializer(_client, _commands).BuildServiceProvider();

            _logger = _services.GetRequiredService<LoggerService>();

            _client.Log += _logger.Log;
            _commands.Log += _logger.Log;
        }

        private async Task MainAsync()
        {
            // Dry-fire the service to have it up and ready for events
            _services.GetRequiredService<ServerConfigService>();

            await _services.GetRequiredService<CommandHandler>().InitializeAsync();

            string token = JsonConvert.DeserializeObject<TokenPad>(
                File.ReadAllText("Config/token.json"))
                .TokenValue;

            var http = _services.GetRequiredService<HttpMessagingService>();
            http.PostMessageAsync("Hello world!");
            /*
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            */
            await Task.Delay(Timeout.Infinite);
        }
    }
}
