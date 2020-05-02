using Microsoft.Extensions.Configuration;
using System;

namespace MayhemDiscordBot.Models
{
    public class MayhemConfiguration
    {
        public readonly string Token;
        public readonly string Prefix;
        public readonly ulong Guild;

        public readonly VoiceChannelConfiguration VoiceChannels;
        public readonly TextChannelConfiguration TextChannels;
        public readonly CounterStrikeConfiguration CounterStrike;

        public MayhemConfiguration(IConfigurationRoot config)
        {
            Token = config["Token"];
            Prefix = config["Prefix"];
            Guild = ulong.Parse(config["Guild"]);

            VoiceChannels = new VoiceChannelConfiguration(config.GetSection("VoiceChannels"));
            TextChannels = new TextChannelConfiguration(config.GetSection("TextChannels"));
            CounterStrike = new CounterStrikeConfiguration(config.GetSection("CounterStrike"));
        }
    }

    public class CounterStrikeConfiguration
    {
        public readonly string IP;
        public readonly ushort Port;
        public readonly string RconPassword;

        public CounterStrikeConfiguration(IConfigurationSection config)
        {
            IP = config["IP"];
            Port = ushort.Parse(config["Port"]);
            RconPassword = config["RconPassword"];

        }
    }

    public class TextChannelConfiguration
    {
        public readonly ulong Log;
        public TextChannelConfiguration(IConfigurationSection config)
        {
            Log = ulong.Parse(config["Log"]);
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
