using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    class Gpt2MessagingService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly DiscordSocketClient socketClient;
        private readonly LoggerService logger;

        public Gpt2MessagingService(
            IServiceProvider serviceProvider, 
            DiscordSocketClient socketClient, 
            LoggerService logger)
        {
            this.serviceProvider = serviceProvider;
            this.socketClient = socketClient;
            this.logger = logger;
        }

        private ServerConfigService configService;
        ServerConfigService ServerConfig
        {
            get {
                if (configService == null)
                    configService = serviceProvider.GetRequiredService<ServerConfigService>();
                return configService;
            }
        }

        private HttpMessagingService httpMessaging;
        HttpMessagingService HttpMessaging
        {
            get
            {
                if (httpMessaging == null)
                    httpMessaging = serviceProvider.GetRequiredService<HttpMessagingService>();
                return httpMessaging;
            }
        }

        public void Initialize()
        {
            socketClient.MessageReceived += HandleMessageReceivedAsync;
        }

        private async Task HandleMessageReceivedAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            string jsonResponse = await httpMessaging.PostMessageAsync(message);

            await logger.Log(
                new Discord.LogMessage(
                    Discord.LogSeverity.Info, 
                    "Gpt2MessagingService", 
                    jsonResponse));

            var replyTask = message.Channel.SendMessageAsync("Capybara");

            await replyTask;
        }
    }
}
