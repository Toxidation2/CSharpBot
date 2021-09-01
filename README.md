# CSharpBot
CSharpBot is a Discord bot.

## Usage/Build instructions
1. Clone the repository: ``https://github.com/Toxidation2/CSharpBot.git`` and move into the project directory: ``cd CSharpBot``
2. Build the source code: ``dotnet build -c Release -r win-x64``
3. Change into the build directory: ``cd bin\Release\net5.0\win-x64\``
4. Run the bot: ``CSharpBot``. This will generate a blank configuration file for you.
5. Edit the ``config.json``  file and add your token into the token field
6. Run the bot once more, as before. Once it has started up (It'll output "Ready" to the terminal), you should be good to go into Discord and use it.

*If you're on Linux there's some changes you need to make to the build process but I'm sure you can figure that out, as you do.*
