using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Amagos.Modules
{
    [Group("configure")]
    [Summary("Configure how this bot should behave on this Discord Guild")]
    public class GuildConfigurationModule : ModuleBase<SocketCommandContext>
    {
        public ServerConfigService ServerConfigService { get; set; }

        [Command("add")]
        [Summary("Add a channel to the list of allowed channels this bot shall only read and reply in")]
        public Task AddChannelAsync(
            [Summary("What command, module, behaviour, or setting to change")]
            IChannel channel
            )
        {
            if (channel is null)
            {
                return ReplyAsync(
                    "Please give me a valid channel, for I got none.",
                    messageReference: DiscordUtilities.ContextReply(Context)
                    );
            }

            var cid = channel.Id;

            var modifyTask = ServerConfigService.ModifyConfig(Context.Guild.Id, (config) =>
            {
                config.PublicCommandChannels.Add(cid);
            }
            );

            var replyTask = ReplyAsync(
                $"Got it! Added channel {DiscordUtilities.ClickableChannel(channel)} to the list.", 
                messageReference: DiscordUtilities.ContextReply(Context));

            return Task.WhenAll(modifyTask, replyTask);
        }

        [Command("remove")]
        [Summary("Remove one channel from the list if it exists in the list")]
        public Task RemoveChannelAsync(IChannel channel)
        {
            if (channel is null)
            {
                return Task.CompletedTask;
            }

            var cid = channel.Id;
            if (!ServerConfigService.GetConfig(Context.Guild).PublicCommandChannels.Contains(cid))
            {
                return ReplyAsync(
                    "This channel is not in the list!",
                    messageReference: DiscordUtilities.ContextReply(Context)
                    );
            }

            var modifyTask = ServerConfigService.ModifyConfig(Context.Guild.Id, (config) =>
            {
                config.PublicCommandChannels.Remove(cid);
            }
            );
            var loggerTask = ReplyAsync(
                $"Got it! The channel {DiscordUtilities.ClickableChannel(channel)} has been removed from the list!",
                messageReference: DiscordUtilities.ContextReply(Context)
                );

            return Task.WhenAll(modifyTask, loggerTask);
        }

        [Command("view")]
        [Summary("View the list of channels in which this bot is only allowed to listen to for most commands")]
        public Task ViewChannelListAsync()
        {
            var config = ServerConfigService.GetConfig(Context.Guild);

            if (config.PublicCommandChannels.Count < 1)
            {
                ReplyAsync(
                    "There are currently no listed channels", 
                    messageReference: DiscordUtilities.ContextReply(Context)
                    );
            }
            else if (config.PublicCommandChannels.Count == 1)
            {
                var channel = Context.Guild.GetChannel(config.PublicCommandChannels[0]);
                ReplyAsync(
                    $"The only channel I am listening to is: {DiscordUtilities.ClickableChannel(channel)}", 
                    messageReference: DiscordUtilities.ContextReply(Context)
                    );
            }
            else
            {
                var message = "These are the channels I am listening to:\n";
                foreach (ulong id in config.PublicCommandChannels)
                {
                    var channel = Context.Guild.GetChannel(id);
                    message += $"- {DiscordUtilities.ClickableChannel(channel)}\n";
                }
                ReplyAsync(
                    message, 
                    messageReference: DiscordUtilities.ContextReply(Context)
                    );
            }
            return Task.CompletedTask;
        }
    }
}
