using Discord.Commands;
using MayhemDiscord.Bot.Attributes;
using MayhemDiscord.Bot.Models;
using MayhemDiscord.QueryMasterCore;
using MayhemDiscord.QueryMasterCore.GameServer;
using MayhemDiscordBot.Models;
using Renci.SshNet;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MayhemDiscordBot.Modules
{
    public class CounterStrikeCommands : ModuleBase<SocketCommandContext>
    {
        private readonly MayhemConfiguration _config;

        public CounterStrikeCommands(MayhemConfiguration config)
        {
            _config = config;
        }

        [Command("cs")]
        [RequireRole("CsAdmin")]
        [Summary("Perform a CS:GO command")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")]
            string input)
        {


            string cmd = input;
            string message = null;
            int firstSpaceIndex = input.IndexOf(" ");
            if (firstSpaceIndex > -1)
            {
                cmd = input.Substring(0, firstSpaceIndex);
                message = input.Substring(firstSpaceIndex, input.Length - firstSpaceIndex).Trim();
            }

            switch (cmd.ToLower())
            {
                case "rcon":
                    await RconCommand(message);
                    return;
                case "restart":
                    await RestartServer();
                    return;                
                case "ssh":
                    await SshCommand(message);
                    return;
                case "gamemode":
                    await GameMode(message);
                    return;
                case "gamemodes":
                    await GameModes();
                    return;
            }
        }

        public async Task RconCommand(string message)
        {

            using (var server = ServerQuery.GetServerInstance(EngineType.Source, _config.CounterStrike.IP, _config.CounterStrike.Port, false, 1000, 1000, 1, false))
            {
                var serverInfo = server.GetInfo();
                if (serverInfo == null)
                {
                    await ReplyAsync("Server is down.");
                    return;
                }

                server.GetControl(_config.CounterStrike.RconPassword);

                var result = server.Rcon.SendCommand(message);
                if (result == null)
                {
                    await ReplyAsync($"Ran command '{message}'");
                }
                else
                {
                    await ReplyAsync($"Ran command '{message}' with result '{result}'");
                }
            }
            await ReplyAsync(message);
        }

        public async Task GameMode(string input)
        {
            using (var client = CmSsh.CreateClient(_config))
            {
                client.Connect();
                var cmd = client.RunCommand($"test -e /srv/steam/hlserver/csgo/cfg/gamemode_{input}.cfg && echo file exists || echo file not found");
                if (cmd.Result.Equals("file not found\n"))
                {
                    await ReplyAsync($"No gamemode found for {input}");
                    return;
                }

                await RconCommand($"exec gamemode_{input.ToLower()}");
                
            }
        }

        public async Task GameMode2(string input)
        {
            using (var client = CmSsh.CreateClient(_config))
            {
                client.Connect();
                var cmd = client.RunCommand($"test -e /srv/steam/startconfigurations/{input.ToUpper()} && echo file exists || echo file not found");
                if (cmd.Result.Equals("file not found\n"))
                {
                    await ReplyAsync($"No gamemode found for {input}");
                    return;
                }

                client.RunCommand($"ln -f /srv/steam/startconfigurations/{input.ToUpper()} /srv/steam/CSGO_CONFIGURATION");
                await ReplyAsync($"Gamemode changed to {input}");
            }
        }

        public async Task GameModes()
        {
            using (var client = CmSsh.CreateClient(_config))
            {
                string runCommand = @"ls /srv/steam/hlserver/csgo/cfg/gamemode*.cfg -1 | grep -oP 'gamemode_\K\w+'";
                string seperator = "\n";
                string fileStartsWith = "gamemode_";
                string fileEndsWith = ".cfg";
                string joinSeperator = ", ";

                client.Connect();
                var cmd = client.RunCommand(runCommand);
                var result = cmd.Result
                    .Split(seperator)
                    .ToArray();

                await ReplyAsync(String.Join(joinSeperator, result));
            }
        }

        public async Task GameModes2()
        {
            using (var client = CmSsh.CreateClient(_config))
            {
                string runCommand = "ls /srv/steam/startconfigurations";
                string seperator = "\n";
                string joinSeperator = ", ";

                client.Connect();
                var cmd = client.RunCommand(runCommand);
                var result = cmd.Result
                    .Split(seperator)
                    .Select(file => file.ToLower())
                    .ToArray();

                await ReplyAsync(String.Join(joinSeperator, result));
            }
        }

        public async Task SshCommand(string message)
        {
            using (var client = CmSsh.CreateClient(_config))
            {
                try
                {
                    client.Connect();
                    var cmd = client.RunCommand(message);
                    await ReplyAsync(cmd.Result);
                    client.Disconnect();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task RestartServer()
        {
            using (var client = CmSsh.CreateClient(_config))
            {
                try
                {
                    SshCommand cmd = null;
                    client.Connect();
                    cmd = client.RunCommand("sudo /bin/systemctl stop csgo");
                    if (cmd.ExitStatus != 0)
                    {
                        await ReplyAsync(cmd.Error);
                        return;
                    }

                    cmd = client.RunCommand("sudo /bin/systemctl start csgo");
                    if (cmd.ExitStatus != 0)
                    {
                        await ReplyAsync(cmd.Error);
                        return;
                    }

                    client.Disconnect();
                    await ReplyAsync("Restarting server");
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
