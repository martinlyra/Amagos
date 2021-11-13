using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Amagos
{
    internal class Initializer
    {
        private readonly CommandService commandService;
        private readonly DiscordSocketClient socketClient;

        internal Initializer(
            DiscordSocketClient socketClient,
            CommandService commandService
            )
        {
            this.socketClient = socketClient ?? new DiscordSocketClient();
            this.commandService = commandService ?? new CommandService();
        }

        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton<LoggerService>()
            .AddSingleton<HttpMessagingService>()

            .AddSingleton(commandService)
            .AddSingleton(socketClient)

            .AddSingleton<CommandHandler>()
            .AddSingleton<ServerConfigService>()

            .BuildServiceProvider();
    }
}
