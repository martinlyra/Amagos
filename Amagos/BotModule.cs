using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    internal abstract class BotModule
    {
        private AmagosBot _client;

        public void SetClient(AmagosBot client)
        {
            _client = client;
        }

        public virtual void Initialize() { }

        public virtual void OnMessageUpdated(object sender, Discord.MessageUpdatedEventArgs e) { }
        public virtual void OnMessageReceived(object sender, Discord.MessageEventArgs e) { }
        public virtual void OnCommandReceived(object sender, Discord.MessageEventArgs e) { }

        public virtual void OnUserJoined(object sender, Discord.UserEventArgs e) { }
        public virtual void OnUserLeft(object sender, Discord.UserEventArgs e) { }

        public virtual void OnUserBanned(object sender, Discord.UserEventArgs e) { }
        public virtual void OnUserUnbanned(object sender, Discord.UserEventArgs e) { }

        public virtual void OnJoinedServer(object sender, Discord.ServerEventArgs e) { }
        public virtual void OnLeftServer(object sender, Discord.ServerEventArgs e) { }

        public AmagosBot Client {
            get { return _client; }
            private set { _client = value; }
        }
    }
}
