using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    class DiscordUtilities
    {
        internal static string ClickableChannel(IChannel channel)
        {
            return $"<#{channel.Id}>";
        }

        internal static MessageReference ContextReply(SocketCommandContext context)
        {
            return new MessageReference(context.Message.Id);
        }
    }
}
