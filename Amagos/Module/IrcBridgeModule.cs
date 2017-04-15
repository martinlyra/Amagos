using Amagos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChatSharp;
using Discord;
using ChatSharp.Events;

namespace Amagos.Module
{
    class IrcBridgeModule : BotModule
    {
        private class IrcBridgeConfiguration
        {
            private ulong _linkedDiscordServerId;
            private string _linkedDiscordChannelName;
            private string _linkedIrcChannelName;

            public IrcBridgeConfiguration(
                ulong discordServerId,
                string discordChannelName,
                string ircChannelName)
            {
                _linkedDiscordServerId = discordServerId;
                _linkedDiscordChannelName = discordChannelName;
                _linkedIrcChannelName = ircChannelName;
            }

            public ulong LinkedDiscordServerId
            {
                get { return _linkedDiscordServerId; }
                set { _linkedDiscordServerId = value; }
            }
            public string LinkedDiscordChannelName
            {
                get { return _linkedDiscordChannelName; }
                set { _linkedDiscordChannelName = value; }
            }
            public string LinkedIrcChannelName
            {
                get { return _linkedIrcChannelName; }
                set { _linkedIrcChannelName = value; }
            }
        }


        private IrcClient _client;

        private Server _targetDiscordServer;

        private string _targetIrcChannelName = "#sporecreate";

        private string _bridgeName = "Kopparberg";

        public override void Initialize()
        {
            _client = new IrcClient("irc.freenode.net", new IrcUser(
                "Kopparberg",
                "YABBa",
                "",
                "An IRC-Discord bridge bot"));

            _client.ConnectionComplete += OnIrcConnectionComplete;
            _client.ChannelMessageRecieved += OnIrcChannelMessageReceived;

            ConnectBridgeAsync();
        }

        private void ConnectBridgeAsync()
        {
            Console.WriteLine("[IRC-Bridge]: Connecting to IRC network...");
            _client.ConnectAsync();
        }

        private void OnIrcConnectionComplete(object sender, EventArgs e)
        {
            Console.WriteLine("[IRC-Bridge]: Connection complete!");
            _client.JoinChannel(_targetIrcChannelName);
        }

        private void OnIrcChannelMessageReceived(object sender, PrivateMessageEventArgs e)
        {
            if (!e.PrivateMessage.IsChannelMessage || _targetDiscordServer == null)
                return;

            var user = e.PrivateMessage.User;

            if (_client.User.Hostname == user.Hostname)
                return;

            var channel = _targetDiscordServer.AllChannels.First();
            channel.SendMessage($"<{e.PrivateMessage.User.Nick}>: {e.PrivateMessage.Message}");
        }

        public override void OnMessageReceived(object sender, MessageEventArgs e)
        {
            var user = e.User;

            if (Client.DiscordClient.CurrentUser.Id == user.Id)
                return;

            if (_targetDiscordServer == null)
                _targetDiscordServer = e.Server;

            var channel = _client.Channels[_targetIrcChannelName];

            if (channel == null)
                return;

            var senderUser = e.User;
            var message = $"**<{senderUser.Name}>:** {e.Message.Text}";
            channel.SendMessage(message);
        }
    }
}
