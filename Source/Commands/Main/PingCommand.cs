using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class PingCommand : BaseCommandModule
    {
        [Command("ping")]
        [Description("Gets the bots latency to Discord")]
        [Category(Category.Main)]
        public async Task Ping(CommandContext commandContext)
        {
            DiscordEmbedBuilder discordEmbedBuilder = new DiscordEmbedBuilder();
            discordEmbedBuilder.WithColor(DiscordColor.Gold);
            discordEmbedBuilder.WithTitle($"🏓 Pong! **{Bot.client.Ping}ms**");
            await commandContext.ReplyAsync("", discordEmbedBuilder.Build());
        }
    }
}