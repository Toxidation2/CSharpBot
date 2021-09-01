using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class HelpCommand : BaseCommandModule
    {
        [Command("help")]
        [Description("Lists commands or gets info on a specific command")]
        [Usage("[command]")]
        [Category(Category.Main)]
        public async Task Help(CommandContext Context, [RemainingText] string command = null)
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithFooter($"Type \"{Bot.config.prefix}help [command]\" to get more info on a command");

            if (command == null)
            {
                eb.WithTitle($"{Bot.client.CurrentUser.Username} Commands");
                eb.AddField("**Main**", GetCommands(Category.Main), false);
            }
            else
            {
                string usage = GetCommandUsage(command);
                if (usage != null)
                {
                    string upperCommandName = command[0].ToString().ToUpper() + command.Remove(0, 1);
                    eb.WithTitle($"{upperCommandName} Command");
                    eb.WithDescription($"{usage}");
                }
                else
                {
                    await Context.ReplyAsync("That command doesn't seem to exist.");
                    return;
                }
            }

            await Context.ReplyAsync("", eb.Build());
        }

        static string GetCommands(Category searchCategory)
        {
            string finalString = "";
            foreach (Command command in Bot.commands.RegisteredCommands.Values)
            {
                Category category = ((CategoryAttribute)command.CustomAttributes.FirstOrDefault(x => x.GetType() == typeof(CategoryAttribute))).Category;
                if (category != searchCategory)
                    continue;

                if (!string.IsNullOrWhiteSpace(finalString)) finalString += $" | `{command.Name}`";
                else finalString = $"`{command.Name}`";
            }
            return finalString;
        }

        public static string GetCommandUsage(string commandName)
        {
            Command command = Bot.commands.FindCommand(commandName.ToLower(), out string args);
            if (command == null)
                return null;
            UsageAttribute usage = (UsageAttribute)command.CustomAttributes.FirstOrDefault(x => x.GetType() == typeof(UsageAttribute));
            
            string desc = $"{command.Description}\n\n**Usage:** {Bot.config.prefix}{commandName}";
            if (usage != null)
                desc += $" {usage.Usage}";

            return desc;
        }
    }
}