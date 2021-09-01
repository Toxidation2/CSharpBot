using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Newtonsoft.Json;

using Serilog;

using Microsoft.Extensions.Logging;

using WinBot.Util;

namespace WinBot
{
    class Bot
    {
        static void Main(string[] args) => new Bot().RunBot().GetAwaiter().GetResult();

        public static DiscordClient client;
        public static CommandsNextExtension commands;
        public static BotConfig config;
        public static List<ulong> blacklistedUsers = new List<ulong>();

        public async Task RunBot()
        {
            if (!File.Exists("blacklist.json"))
                File.WriteAllText("blacklist.json", "[]");
            blacklistedUsers = JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText("blacklist.json"));

            if (File.Exists("config.json"))
                config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));
            else
            {
                config = new BotConfig();
                config.token = "TOKEN";
                config.status = " ";
                config.prefix = ".";

                File.WriteAllText("config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                Console.WriteLine("No configuration file found. A template config has been written to config.json");
                return;
            }

            client = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { config.prefix },
                EnableDefaultHelp = false,
                EnableDms = false,
                UseDefaultCommandHandler = false
            });

            commands.CommandErrored += async (CommandsNextExtension cnext, CommandErrorEventArgs e) =>
            {
                string msg = e.Exception.Message;
                if (msg == "One or more pre-execution checks failed.")
                    msg += " This is likely a permissions issue.";
                
                await e.Context.ReplyAsync($"There was an error executing your command!\nMessage: `{msg}`");
            };
            client.Ready += async (DiscordClient client, ReadyEventArgs e) => {
                await client.UpdateStatusAsync(new DiscordActivity() { Name = config.status });
            };
            client.MessageCreated += CommandHandler;
            
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            await client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task CommandHandler(DiscordClient client, MessageCreateEventArgs e)
        {
            DiscordMessage msg = e.Message;
            
            if (blacklistedUsers.Contains(msg.Author.Id) || e.Author.IsBot)
                return;

            int start = msg.GetStringPrefixLength(config.prefix);
            if (start == -1) return;

            string prefix = msg.Content.Substring(0, start);
            string cmdString = msg.Content.Substring(start);

            if (cmdString.Contains(" && ")) {
                string[] commands = cmdString.Split(" && ");
                if (commands.Length > 2) return;
                for(int i = 0; i < commands.Length; i++) {
                    DoCommand(commands[i], prefix, msg);
                }
                return;
            }

            Command cmd = commands.FindCommand(cmdString, out var args);
            if (cmd == null) return;
            CommandContext ctx = commands.CreateContext(msg, prefix, cmd, args);
            _ = Task.Run(async () => await commands.ExecuteCommandAsync(ctx).ConfigureAwait(false));
        }

        private void DoCommand(string commandString, string prefix, DiscordMessage msg) {
            Command cmd = commands.FindCommand(commandString, out var args);
            if (cmd == null) return;
            CommandContext ctx = commands.CreateFakeContext(msg.Author, msg.Channel, commandString, prefix, cmd, args);
            _ = Task.Run(async () => await commands.ExecuteCommandAsync(ctx).ConfigureAwait(false));
        }
    }

    class BotConfig
    {
        public string token { get; set; }
        public string status { get; set; }
        public string prefix { get; set; } = ".";
    }
}