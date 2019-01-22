using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MayhemBot.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PlexDiscordBot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly MayhemConfiguration _config;

        public StartupService(DiscordSocketClient discord, CommandService commands, MayhemConfiguration config)
        {
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            if (string.IsNullOrWhiteSpace(_config.Token))
            {
                throw new Exception("Token missing from config.json! Please enter your token there (root directory)");
            }

            await _discord.LoginAsync(TokenType.Bot, _config.Token);
            await _discord.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}