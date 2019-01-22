using System;
using System.Collections.Generic;


namespace MayhemBot.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Shuffle list based on Fisher–Yates shuffle algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <see cref="https://stackoverflow.com/questions/49570175/simple-way-to-randomly-shuffle-list"></see>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;

            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }
        }
    }
}
