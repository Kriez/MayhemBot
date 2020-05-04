using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MayhemDiscord.Bot.Interfaces;
using MayhemDiscordBot.Models;
using Renci.SshNet;

namespace MayhemDiscord.Bot.Models
{
    public static class CmSsh 
    {
        public static SshClient CreateClient(MayhemConfiguration config)
        {
            if (!File.Exists(config.Ssh.KeyAuth))
            {
                throw new Exception("Key does not exists");
            }
            PrivateKeyFile privateKeyStream = new PrivateKeyFile(config.Ssh.KeyAuth);

            var connectionInfo = new PrivateKeyConnectionInfo(config.Ssh.IP,
                config.Ssh.Username,
                privateKeyStream);

            return new SshClient(connectionInfo);
        }
    }
}
