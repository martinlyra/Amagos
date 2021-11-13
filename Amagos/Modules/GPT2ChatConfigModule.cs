using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos.Modules
{
    [Group("gpt2")]
    [Summary("Configure how chatty this bot should be, uses GPT2-based Python package aitextgen")]
    class GPT2ChatConfigModule : ModuleBase<SocketCommandContext>
    {
        public ServerConfigService ServerConfigService { get; set; }

        public class GPT2ChatModuleConfig : ModuleConfig
        {
            public float ChatChance { 
                get => getConfigEntry("ChatChance", 0.0f);
                set => setConfigEntry("ChatChance", value);
            }

            public bool Enabled
            {
                get => getConfigEntry("IsEnabled", false);
                set => setConfigEntry("IsEnabled", value);
            }
        }

        [Command("chance")]
        [Summary("Set the chance this bot will quip unprovoked")]
        public Task SetChanceAsync(
                [Summary("The chance, between 0.0 and 1.0")]
                float chance
            )
        {
            var newValue = Math.Max(0.0f, Math.Min(1.0f, chance));
            var modifyTask = ServerConfigService.ModifyConfig(Context.Guild.Id,
                (config) =>
                {
                    var c = config.fetchModuleConfig<GPT2ChatModuleConfig>();
                    c.ChatChance = newValue;
                }
                );
            var replyTask = ReplyAsync(
                $"Got it! Set the response chance to {newValue}!",
                messageReference: DiscordUtilities.ContextReply(Context));
            return Task.WhenAll(modifyTask, replyTask);
        }

        [Command("enable")]
        [Summary("Enables the bot's chatty function")]
        public Task EnableAsync()
        {
            var modifyTask = ServerConfigService.ModifyConfig(Context.Guild.Id,
                (config) =>
                {
                    var c = config.fetchModuleConfig<GPT2ChatModuleConfig>();
                    c.Enabled = true;
                }
                );
            var replyTask = ReplyAsync(
                $"Got it! I will be chatty!",
                messageReference: DiscordUtilities.ContextReply(Context));
            return Task.WhenAll(modifyTask, replyTask);
        }

        [Command("disable")]
        [Summary("Enables the bot's chatty function")]
        public Task DisableAsync()
        {
            var modifyTask = ServerConfigService.ModifyConfig(Context.Guild.Id,
                (config) =>
                {
                    var c = config.fetchModuleConfig<GPT2ChatModuleConfig>();
                    c.Enabled = true;
                }
                );
            var replyTask = ReplyAsync(
                $"Got it! I will not be chatty at all!",
                messageReference: DiscordUtilities.ContextReply(Context));
            return Task.WhenAll(modifyTask, replyTask);
        }
    }
}
