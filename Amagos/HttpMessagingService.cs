using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    class HttpMessagingService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly LoggerService logger;
        private readonly HttpClient httpClient;

        public HttpMessagingService(
            IServiceProvider serviceProvider,
            LoggerService logger
            ) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.httpClient = new HttpClient();
        }

        public async Task<String> PostMessageAsync(object message)
        {
            try
            {
                string json = JsonConvert.SerializeObject(message);
                HttpContent postData = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:6666", postData);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();

                await logger.Log(new LogMessage(
                    LogSeverity.Info,
                    "HttpMessagingService",
                    $"Got reponse {response.StatusCode}, content: {content}"
                    ));
                return content;
            }
            catch (HttpRequestException e)
            {
                await logger.Log(new LogMessage(
                    LogSeverity.Error,
                    "HttpMessagingService",
                    $"Caught exception while sending: {e.Message}"
                ));
            }
            return null;
        }
    }
}
