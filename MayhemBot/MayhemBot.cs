using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MayhemBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlexDiscordBot.Services;
using System;
using System.Threading.Tasks;


namespace PlexDiscordBot
{
    public class MayhemBot
    {
        private MayhemConfiguration _config;

        public async Task StartAsync()
        {
            var _builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(path: "config.json");
            _config = new MayhemConfiguration(_builder.Build());

            var services = new ServiceCollection()
               .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
               {
                   LogLevel = LogSeverity.Verbose,
                   MessageCacheSize = 1000
               }))
               .AddSingleton(_config)
               .AddSingleton(new CommandService(new CommandServiceConfig
               {
                   DefaultRunMode = RunMode.Async,
                   LogLevel = LogSeverity.Verbose,
                   CaseSensitiveCommands = false,
                   ThrowOnError = false
               }))
               .AddSingleton<LoggingService>()
               .AddSingleton<CommandHandler>()
               .AddSingleton<StartupService>();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<DiscordSocketClient>().Log += Log;
            serviceProvider.GetRequiredService<CommandHandler>();
            serviceProvider.GetRequiredService<LoggingService>();

            await serviceProvider.GetRequiredService<StartupService>().StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
