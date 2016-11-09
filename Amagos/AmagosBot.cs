using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    internal class AmagosBot : IDisposable
    {
        Discord.DiscordClient _client;

        readonly Dictionary<string, ServerConfig> _serverConfigs = new Dictionary<string, ServerConfig>();

        readonly List<BotModule> _botModules = new List<BotModule>();

        string _botToken = "";

        string _botCommandPrefix = "<";

        public AmagosBot()
        {
            _botToken = File.ReadAllText("token");
        }

        async void Initialize()
        {
            _client = new Discord.DiscordClient();

            await _client.Connect(token: _botToken, tokenType: Discord.TokenType.Bot);

            AttachEventHandlers();

            InitializeModules();
        }

        void AttachEventHandlers()
        {
            _client.JoinedServer += OnJoinedServer;
            _client.LeftServer += OnLeftServer;
            _client.MessageReceived += OnMessageReceived;
            _client.UserJoined += OnUserJoined;
            _client.UserLeft += OnUserLeft;
            _client.MessageUpdated += OnMessageUpdated;
            _client.UserBanned += OnUserBanned;
            _client.UserUnbanned += OnUserUnbanned;

            
        }

        void InitializeModules()
        {
            _botModules.Add(new Module.Notification());
        }

        private void OnUserUnbanned(object sender, Discord.UserEventArgs e)
        {
            foreach (var m in _botModules)
                m.OnUserUnbanned(sender, e);
        }

        private void OnUserBanned(object sender, Discord.UserEventArgs e)
        {
            foreach (var m in _botModules)
                m.OnUserBanned(sender, e);
        }

        private void OnMessageUpdated(object sender, Discord.MessageUpdatedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnUserLeft(object sender, Discord.UserEventArgs e)
        {
            foreach (var m in _botModules)
                m.OnUserLeft(sender, e);
        }

        private void OnUserJoined(object sender, Discord.UserEventArgs e)
        {
            foreach (var m in _botModules)
                m.OnUserJoined(sender, e);
        }

        async private void OnMessageReceived(object sender, Discord.MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Text.StartsWith(_botCommandPrefix) 
                || message.IsMentioningMe())
            {
                if (message.Text.Contains("serverstatus"))
                {
                    var data = await Module.ServerStatus.FetchServerData();
                    if (data.Version == null)
                        await e.Channel.SendMessage("Failed to retrieve server data. Currently offline, perhaps?");
                    else
                    {
                        var response = "**Server is up**; " + data.Version + ", with "
                            + data.PlayerCount + " players on the gamemode " + "\"" + data.Mode
                            + "\"";
                        Console.WriteLine(response);
                        await e.Channel.SendMessage(response);
                    }
                }
            }

        }

        private void OnLeftServer(object sender, Discord.ServerEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnJoinedServer(object sender, Discord.ServerEventArgs e)
        {
            //throw new NotImplementedException();
        }

        internal void RunOrDie()
        {
            Initialize();

            while (true)
            {

            }
        }

        

        public void Dispose()
        {
            _client.Disconnect();
        }
    }
}
