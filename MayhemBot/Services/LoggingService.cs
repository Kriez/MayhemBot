﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using MayhemDiscordBot.Models;

namespace MayhemDiscordBot.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly MayhemConfiguration _config;

        private string _logDirectory { get; }
        private string _logFile;

        // DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
        public LoggingService(DiscordSocketClient discord, CommandService commands, MayhemConfiguration config)
        {
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            _discord = discord;
            _commands = commands;
            _config = config;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;

            CreateLogFile();
        }

        private void CreateLogFile()
        {
            string logFile = string.Empty;
            logFile = Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd-HH")}.txt");
            if (!Directory.Exists(_logDirectory))     // Create the log directory if it doesn't exist
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(logFile))               // Create today's log file if it doesn't exist
                File.Create(logFile).Dispose();
            _logFile = logFile;


        }

        private async Task OnLogAsync(LogMessage msg)
        {
            //CreateLogFile();
            //string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            //File.AppendAllText(_logFile, logText + "\n");     // Write the log text to a file
            //return Console.Out.WriteLineAsync(logText);       // Write the log text to the console

            if (msg.Severity == LogSeverity.Error || msg.Severity == LogSeverity.Critical)
            {
                await LogToDiscord(msg);
            }
        }

        private async Task LogToDiscord(LogMessage msg)
        {
            string logText = $"{DateTime.UtcNow:hh:mm:ss} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";

            var guild = _discord.GetGuild(_config.Guild);
            var channel = guild?.GetTextChannel(_config.TextChannels.Log);

            if (channel == null)
            {
                return;
            }
            await channel?.SendMessageAsync(logText);
        }

    }
}