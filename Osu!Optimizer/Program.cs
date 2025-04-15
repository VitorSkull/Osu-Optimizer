using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace OsuOptimizer
{
    internal class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                string osuProcess = "osu!";
                string discordProcess = "Discord";

                Process[] processOsu = Process.GetProcessesByName(osuProcess);
                Process[] processDiscord = Process.GetProcessesByName(discordProcess);
                int avaliableCores = Environment.ProcessorCount;
                int halfCores = avaliableCores / 2;

                long afinityOsu = 0;
                long afinityDiscord = 0;

                Process mainDiscordProcess = processDiscord
                    .OrderByDescending(p => p.WorkingSet64)
                    .FirstOrDefault();

                    //Console.WriteLine(avaliableCores);
                    //Console.WriteLine(processDiscord);
                    //Console.WriteLine(processOsu);

                if (processOsu.Length == 0 || processDiscord.Length == 0)
                {
                    Console.WriteLine("You need to open osu!.");
                    Thread.Sleep(60 * 1000);
                    continue;
                }

                for (int i = 0; i < avaliableCores; i++)
                {
                    if (i < halfCores)
                    {
                        afinityOsu |= 1L << i;
                    }
                    else
                    {
                        afinityDiscord |= 1L << i;
                    }
                }
                //Console.WriteLine($"Afinidade osu!:      {Convert.ToString(afinityOsu, 2).PadLeft(avaliableCores, '0')}");
                //Console.WriteLine($"Afinidade Discord:   {Convert.ToString(afinityDiscord, 2).PadLeft(avaliableCores, '0')}");

                foreach (Process osu in processOsu)
                {
                    try
                    {
                        osu.ProcessorAffinity = (IntPtr)afinityOsu;
                        Console.WriteLine($"Best afinity defined for osu! (PID: {osu.Id})");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError to define afinity for osu!, your game is probably closed.\nError message({ex.Message})");
                    }
                }

                foreach (Process discord in processDiscord)
                {
                    try
                    {
                        if (discord.WorkingSet64 >= mainDiscordProcess.WorkingSet64)
                        {
                            mainDiscordProcess.ProcessorAffinity = (IntPtr)afinityDiscord;
                            mainDiscordProcess.PriorityClass = ProcessPriorityClass.Normal;
                            Console.WriteLine($"Best afinity defined for Discord (PID: {discord.Id}).");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError to define afinity for Discord, your discord is probably closed.\nError message({ex.Message})");
                    }
                }
                Thread.Sleep(5 * 60 * 1000);
            }
        }
    }
}
