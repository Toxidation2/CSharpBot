using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using DSharpPlus.Entities;
using Serilog;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace WinBot.Util
{
    public static class MiscUtil
    {
        public static List<ulong> LoadBlacklist()
        {
            if (!File.Exists("blacklist.json"))
            {
                File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(new List<ulong>()));
                return new List<ulong>();
            }
            return JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText("blacklist.json"));
        }

        public static void SaveBlacklist()
        {
            File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(Bot.blacklistedUsers, Formatting.Indented));
        }
    }
}

namespace DSharpPlus.CommandsNext
{
    public static class DSharpImprovements
    {
        public static async Task<DiscordMessage> SendFileAsync(this DiscordChannel channel, string fileName)
        {
            if(!File.Exists(fileName)) {
                Log.Warning($"File does not exist! (SendFileAsync @ {channel.Name})");
                return null;
            }

            FileStream fStream = new FileStream(fileName, FileMode.Open);
            DiscordMessage msg = await new DiscordMessageBuilder().WithFile(fileName, fStream).SendAsync(channel);
            fStream.Close();

            return msg;
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, string Content)
        {
            return await Context.Channel.SendMessageAsync(Content);
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, string Content, DiscordEmbed Embed)
        {
            return await Context.Channel.SendMessageAsync(Content, Embed);
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, DiscordEmbed Embed)
        {
            return await Context.Channel.SendMessageAsync("", Embed);
        }
    }
}