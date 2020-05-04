using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using MayhemDiscord.Bot.Interfaces;
using MayhemDiscord.Bot.Models;
using MayhemDiscord.QueryMasterCore;
using MayhemDiscord.QueryMasterCore.GameServer;
using MayhemDiscordBot.Models;
using Renci.SshNet;

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

                await ReplyAsync("Gamemode found!");
            }
        }


        public async Task GameModes()
        {
            using (var client = CmSsh.CreateClient(_config))
            {
                string runCommand = "ls /srv/steam/hlserver/csgo/cfg";
                string seperator = "\n";
                string fileStartsWith = "gamemode_";
                string fileEndsWith = ".cfg";
                string joinSeperator = ", ";

                client.Connect();
                var cmd = client.RunCommand(runCommand);
                var result = cmd.Result
                    .Split(seperator)
                    .Where(file => file.StartsWith(fileStartsWith) && file.EndsWith(fileEndsWith))
                    .Select(file => file.Substring(fileStartsWith.Length, file.Length - fileStartsWith.Length - fileEndsWith.Length))
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
    }
}
