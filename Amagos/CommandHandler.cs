using Amagos.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commandService;
        private readonly LoggerService logger;
        private readonly IServiceProvider services;

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, LoggerService logger)
        {
            commandService = commands;
            this.services = services;
            this.client = client;
            this.logger = logger;
        }

        public async Task InitializeAsync()
        {
            await commandService.AddModuleAsync<GuildConfigurationModule>(services);

            await logger.Log(new LogMessage(
                LogSeverity.Info,
                "CommadHandler",
                $"Available commands: {string.Join(", ", commandService.Commands.Select((commandInfo) => commandInfo.Name))}"
                ));
            

            client.MessageReceived += HandleCommandAsync;
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            if (msg.Author.Id == client.CurrentUser.Id || msg.Author.IsBot) return;

            int pos = 0;
            if (msg.HasCharPrefix('!', ref pos) || msg.HasMentionPrefix(client.CurrentUser, ref pos))
            {
                var context = new SocketCommandContext(client, msg);

                var result = await commandService.ExecuteAsync(context, pos, services);

                await logger.Log(new LogMessage(
                    LogSeverity.Info,
                    "CommandHandler",
                    $"Received: {context.Message}"
                    ));
                await logger.Log(new LogMessage(
                    LogSeverity.Info,
                    "CommandHandler",
                    $"Result: {result}"
                    ));

                // Uncomment the following lines if you want the bot
                // to send a message if it failed.
                // This does not catch errors from commands with 'RunMode.Async',
                // subscribe a handler for '_commands.CommandExecuted' to see those.
                //if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                //    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
