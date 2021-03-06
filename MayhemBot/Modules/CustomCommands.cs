﻿using Discord;
using Discord.Commands;
using MayhemDiscordBot.Extensions;
using MayhemDiscordBot.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayhemDiscordBot.Modules
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
            var users = (await defaultVoiceChannel.GetUsersAsync().FlattenAsync()).ToList();

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
            users.AddRange((await redVoiceChannel.GetUsersAsync().FlattenAsync()).ToList());
            users.AddRange((await blueVoiceChannel.GetUsersAsync().FlattenAsync()).ToList());

            foreach (var user in users)
            {
                await user.ModifyAsync(usr => usr.ChannelId = _config.VoiceChannels.General);
            }
        }
    }
}
