﻿using System;
namespace MayhemDiscordBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                new MayhemBot().StartAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
