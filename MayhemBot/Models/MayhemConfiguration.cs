using Microsoft.Extensions.Configuration;
using System;

namespace MayhemDiscordBot.Models
{
    public class MayhemConfiguration
    {
        public readonly string Token;
        public readonly char Prefix;

        public readonly VoiceChannelConfiguration VoiceChannels;

        public MayhemConfiguration(IConfigurationRoot config)
        {
            Token = config["Token"];
            Prefix = Char.Parse(config["Prefix"]);

            VoiceChannels = new VoiceChannelConfiguration(config.GetSection("VoiceChannels"));
        }
    }
    public class VoiceChannelConfiguration
    {
        public readonly ulong General;
        public readonly ulong Red;
        public readonly ulong Blue;

        public VoiceChannelConfiguration(IConfigurationSection config)
        {
            General = ulong.Parse(config["General"]);
            Red = ulong.Parse(config["Red"]);
            Blue = ulong.Parse(config["Blue"]);
        }
    }
}
