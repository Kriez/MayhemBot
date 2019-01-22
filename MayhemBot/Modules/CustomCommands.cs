using Discord;
using Discord.Commands;
using MayhemBot.Extensions;
using MayhemBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MayhemBot.Modules
{
    public class CustomCommands : ModuleBase
    {
        private MayhemConfiguration _config;
        public CustomCommands(MayhemConfiguration config)
        {
            _config = config;
        }

        [Command("randomize", RunMode = RunMode.Async)]
        [Summary("Randomize teams")]
        public async Task RandomizeTeamsCommand([Remainder] string args = "")
        {
            var defaultVoiceChannel = await Context.Guild.GetVoiceChannelAsync(_config.VoiceChannels.General);

            var users = (await defaultVoiceChannel.GetUsersAsync().Flatten()).ToList();

            var userCount = users.Count();

            users.Shuffle();

            foreach (var user in users)
            {
                await user.ModifyAsync(usr => usr.ChannelId = _config.VoiceChannels.Red);
            }
            
        }

        [Command("reset", RunMode = RunMode.Async)]
        [Summary("Reset teams")]
        public async Task ResetCommand()
        {
            var redVoiceChannel = await Context.Guild.GetVoiceChannelAsync(_config.VoiceChannels.Blue);
            var blueVoiceChannel = await Context.Guild.GetVoiceChannelAsync(_config.VoiceChannels.Red);

            List<IGuildUser> users = new List<IGuildUser>();
            users.AddRange((await redVoiceChannel.GetUsersAsync().Flatten()).ToList());
            users.AddRange((await blueVoiceChannel.GetUsersAsync().Flatten()).ToList());

            foreach (var user in users)
            {
                await user.ModifyAsync(usr => usr.ChannelId = _config.VoiceChannels.General);
            }
        }
    }
}
