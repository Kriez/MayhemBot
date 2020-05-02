using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using MayhemDiscord.QueryMasterCore;
using MayhemDiscord.QueryMasterCore.GameServer;
using MayhemDiscordBot.Models;

namespace MayhemDiscordBot.Modules
{
    public class CounterStrikeCommands : ModuleBase<SocketCommandContext>
    {
        private MayhemConfiguration _config;

        public CounterStrikeCommands(MayhemConfiguration config)
        {
            _config = config;
        }

        [Command("cs")]
        [Summary("Perform a CS:GO command")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")]
            string echo)
        {
            var rnd = new Random();
            Server server = null;

            using (server = ServerQuery.GetServerInstance(EngineType.Source, _config.CounterStrike.IP, _config.CounterStrike.Port, false, 1000, 1000, 1, false))
            {
                var serverInfo = server.GetInfo();
                if (serverInfo == null)
                {
                    await ReplyAsync("Server is down.");
                    return;
                }

                server.GetControl(_config.CounterStrike.RconPassword);

                var result = server.Rcon.SendCommand(echo);
                if (result == null)
                {
                    await ReplyAsync($"Ran command '{echo}'");
                }
                else
                {
                    await ReplyAsync($"Ran command '{echo}' with result '{result}'");
                }
            }


            await ReplyAsync(echo);
        }
    }
}
