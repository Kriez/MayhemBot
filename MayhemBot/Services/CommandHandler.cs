using Discord.Commands;
using Discord.WebSocket;
using MayhemBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace PlexDiscordBot.Services
{
    public class CommandHandler
    {

        private CommandService _commands;
        private DiscordSocketClient _client;
        private readonly IServiceProvider _provider;
        private readonly MayhemConfiguration _config;

        public CommandHandler(IServiceProvider provider, MayhemConfiguration config)
        {
            _config = config;
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _commands = _provider.GetService<CommandService>();
            _client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage parameterMessage)
        {
            // Don't handle the command if it is a system message
            var message = parameterMessage as SocketUserMessage;
            if (message == null) return;

            // Mark where the prefix ends and the command begins
            int argPos = 0;

            // Create a Command Context
            var context = new SocketCommandContext(_client, message);

            char prefix = _config.Prefix;

            // Determine if the message has a valid prefix, adjust argPos
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos))) return;

            // Execute the Command, store the result            
            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            //await LogCommandUsage(context, result);
            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (result.ErrorReason != "Unknown command.")
                {
                    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
                }
                Console.WriteLine($"{message.Content} -> {result.ErrorReason}");
            }
        }      
    }
}
