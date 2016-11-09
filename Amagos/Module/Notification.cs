using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Amagos.Module
{
    internal sealed class Notification : BotModule
    {

        string _targetChannelName = "general";

        public override void OnUserJoined(object sender, UserEventArgs e)
        {
            var joiner_name = e.User.Name;
            var message = joiner_name + " has joined the server.";
            e.Server.FindChannels(_targetChannelName).First().SendMessage(message);
            Console.WriteLine(message);  
        }

        public override void OnUserLeft(object sender, UserEventArgs e)
        {
            var leaver_name = e.User.Name;
            var message = leaver_name + " has left the server.";
            e.Server.FindChannels(_targetChannelName).First().SendMessage(message);
            Console.WriteLine(message);   
        }
    }
}
