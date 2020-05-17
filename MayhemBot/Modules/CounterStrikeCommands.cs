using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using MayhemDiscord.Bot.Attributes;
using MayhemDiscord.Bot.Models;
using MayhemDiscord.QueryMasterCore;
using MayhemDiscord.QueryMasterCore.GameServer;
using MayhemDiscordBot.Models;
using Renci.SshNet;

namespace MayhemDiscord.Bot.Modules
{
    public class CounterStrikeCommands : ModuleBase<SocketCommandContext>
    {
        private readonly MayhemConfiguration _config;

        public CounterStrikeCommands(MayhemConfiguration config)
        {
            _config = config;
        }

        [Group("cs"), Name("CounterStrike")]
        [RequireRole("CsAdmin")]
        [RequireContext(ContextType.Guild)]
        public class CS : ModuleBase
        {
            private readonly MayhemConfiguration _config;

            public CS(MayhemConfiguration config)
            {
                _config = config;
            }

            [Command("rcon")]
            [Summary("Perform a CS:GO command")]
            public async Task Rcon([Remainder] string command)
            {
                TryRunRconCommand(command, out string response);
                await ReplyAsync(response);
            }

            private bool TryRunRconCommand(string command, out string response)
            {
                using (var server = ServerQuery.GetServerInstance(EngineType.Source, _config.CounterStrike.IP,
                    _config.CounterStrike.Port, false, 1000, 1000, 1, false))
                {

                    var serverInfo = server.GetInfo();
                    if (serverInfo == null)
                    {

                        response = "Server is down.";
                        return false;
                    }

                    server.GetControl(_config.CounterStrike.RconPassword);

                    var result = server.Rcon.SendCommand(command);
                    if (result == null)
                    {
                        response = $"Ran command '{command}'";
                        return true;
                    }
                    else
                    {
                        response = $"Ran command '{command}' with result '{result}'";
                        return true;
                    }
                }
            }

            [Command("status")]
            [Summary("Get status from the server")]
            public async Task Status()
            {
                using (var server = ServerQuery.GetServerInstance(EngineType.Source, _config.CounterStrike.IP,
                    _config.CounterStrike.Port, false, 1000, 1000, 1, false))
                {

                    var serverInfo = server.GetInfo();
                    if (serverInfo == null)
                    {
                        await ReplyAsync("Server is down.");
                        return;
                    }

                    await ReplyAsync($"{serverInfo.Name} {serverInfo.Players}/{serverInfo.MaxPlayers} players");
                }
            }

            [Command("gamemodes")]
            [Summary("Get all gamemodes")]
            public async Task GameModes()
            {
                using (var client = CmSsh.CreateSshClient(_config))
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

            /*
            [Command("gamemode")]
            [Summary("Set gamemode")]
            public async Task GameMode(string input)
            {
                using (var client = CmSsh.CreateSshClient(_config))
                {
                    client.Connect();
                    var cmd = client.RunCommand(
                        $"test -e /srv/steam/hlserver/csgo/cfg/gamemode_{input}.cfg && echo file exists || echo file not found");
                    if (cmd.Result.Equals("file not found\n"))
                    {
                        await ReplyAsync($"No gamemode found for {input}");
                        return;
                    }

                    await Rcon($"exec gamemode_{input.ToLower()}");

                }
            }
    */
             
            [Command("restart")]
            [Summary("Restarts the server")]
            public async Task RestartServer()
            {
                using (var client = CmSsh.CreateSshClient(_config))
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

            [Command("gamemode")]
            [Summary("Set gamemode")]
            public async Task GameMode(string input)
            {
                MemoryStream ms = new MemoryStream();
                using (var client = CmSsh.CreateSftpClient(_config))
                {
                    try
                    {
                        
                        client.Connect();
                        var lines = client.ReadAllLines($"/srv/steam/hlserver/csgo/cfg/gamemode_{input}.cfg");
                        foreach (var line in lines)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (TryRunRconCommand(line, out string response))
                                {
                                    break;
                                }
                                await Task.Delay(500);
                            }
                        }
                        client.Disconnect();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            public async Task SshCommand(string message)
            {
                using (var client = CmSsh.CreateSshClient(_config))
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
        }
    }
}
