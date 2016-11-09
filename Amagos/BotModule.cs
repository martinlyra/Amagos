using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    internal abstract class BotModule
    {
        public virtual void OnMessageUpdated(object sender, Discord.MessageUpdatedEventArgs e) { }
        public virtual void OnReceivedMessage(object sender, Discord.MessageEventArgs e) { }

        public virtual void OnUserJoined(object sender, Discord.UserEventArgs e) { }
        public virtual void OnUserLeft(object sender, Discord.UserEventArgs e) { }

        public virtual void OnUserBanned(object sender, Discord.UserEventArgs e) { }
        public virtual void OnUserUnbanned(object sender, Discord.UserEventArgs e) { }
    }
}
