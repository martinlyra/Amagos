using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

        public async Task InitializeAsync()
        {
            socketClient.MessageReceived += HandleMessageReceivedAsync;

            await logger.Log(
            new Discord.LogMessage(
                Discord.LogSeverity.Info,
                "Gpt2MessagingService",
                "Ready!"));
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

        private async Task HandleMessageReceivedAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (arg.Author.IsBot) return;

            var request = new CgiRequest();
            request.RequestType = CgiRequestType.Gpt2Generate;
            request.Content = message.Content;

            string jsonResponse = await HttpMessaging.PostMessageAsync(request);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

            string result = json["result"];

            await logger.Log(
                new Discord.LogMessage(
                    Discord.LogSeverity.Verbose, 
                    "Gpt2MessagingService", 
                    jsonResponse));

            var replyTask = message.Channel.SendMessageAsync(result);

            await replyTask;
        }
    }
}
