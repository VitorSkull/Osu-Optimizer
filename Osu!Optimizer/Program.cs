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
            int priority = 0;
            int mode = 0;
            int test = 0;
            int count = 15;
            while (true)
            {
                Console.WriteLine("1 - Auto");
                Console.WriteLine("2 - Manual");
                if (!int.TryParse(Console.ReadLine(), out mode) || mode < 1 || mode > 2)
                {
                    Console.Clear();
                    Console.WriteLine("Select a valid option.");
                    continue;
                }
                break;
            }

            while (true)
            {
                if (mode == 1)
                {
                    string osuProcess = "osu!";
                    string discordProcess = "Discord";

                    Process[] processOsu = Process.GetProcessesByName(osuProcess);
                    Process[] processDiscord = Process.GetProcessesByName(discordProcess);
                    int avaliableThreads = Environment.ProcessorCount;
                    int halfCores = avaliableThreads / 2;

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
                        Console.WriteLine($"You need to open osu! and Discord. Trying again in {count} seconds.");
                        count += 15;
                        Thread.Sleep(count * 1000);
                        continue;
                    }

                    for (int i = 0; i < avaliableThreads; i++)
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
                    if (priority == 0)
                    {
                        Console.WriteLine("Priority Level: ");
                        Console.WriteLine("1 - Normal");
                        Console.WriteLine("2 - Below Normal");
                        priority = int.Parse(Console.ReadLine());
                    }

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
                                if (priority == 1)
                                {
                                    mainDiscordProcess.PriorityClass = ProcessPriorityClass.Normal;
                                }
                                if (priority == 2)
                                {
                                    mainDiscordProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                                }
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
                if (mode == 2)
                {

                    string osuProcess = "osu!";
                    string discordProcess = "Discord";

                    Process[] processOsu = Process.GetProcessesByName(osuProcess);
                    Process[] processDiscord = Process.GetProcessesByName(discordProcess);
                    Process mainDiscordProcess = processDiscord
                        .OrderByDescending(p => p.WorkingSet64)
                        .FirstOrDefault();

                    int avaliableThreads = Environment.ProcessorCount;
                    int box = avaliableThreads;


                    int osuCores = 0;
                    int discordCores = 0;

                    long afinityOsu = 0;
                    long afinityDiscord = 0;
                    if (test == 0)
                    {
                        Console.WriteLine("1 - Optimize osu!");
                        Console.WriteLine("2 - Optimize Discord");
                        Console.WriteLine("3 - Optimize both");
                        if (!int.TryParse(Console.ReadLine(), out test) || test <= 0 || test > 3)
                        {
                            Console.Clear();
                            Console.WriteLine("Select a valid option.");
                        }
                    }

                    if (test == 1)
                    {
                        if (processOsu.Length == 0)
                        {
                            Console.WriteLine($"You need to open osu!. Trying again in {count} seconds.");
                            Thread.Sleep(count * 1000);
                            count += count;
                            continue;
                        }
                        Console.WriteLine($"You have {avaliableThreads} remaining");
                        while (box == avaliableThreads)
                        {

                            Console.WriteLine("How many threads for osu?");
                            osuCores = int.Parse(Console.ReadLine());
                            box -= osuCores;

                            if (box == avaliableThreads)
                            {
                                Console.WriteLine($"You need to use at least 1 thread.");
                            }
                        }


                        for (int i = 0; i < osuCores; i++)
                        {
                            afinityOsu |= 1L << i;
                        }

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
                        Thread.Sleep(5 * 60 * 1000);
                    }

                    if (test == 2)
                    {
                        if (processDiscord.Length == 0)
                        {
                            Console.WriteLine($"You need to open Discord!. Trying again in {count} seconds.");
                            Thread.Sleep(count * 1000);
                            count += count;
                            continue;
                        }

                        Console.WriteLine($"You have {avaliableThreads} remaining");
                        while (box == avaliableThreads)
                        {

                            Console.WriteLine("How many threads for Discord?");
                            discordCores = int.Parse(Console.ReadLine());
                            box -= discordCores;

                            if (box == avaliableThreads)
                            {
                                Console.WriteLine($"You need to use at least 1 thread.");
                            }

                            for (int i = 0; i < discordCores; i++)
                            {
                                afinityDiscord |= 1L << i;
                            }
                        }

                        if (priority == 0)
                        {
                            Console.WriteLine("Priority Level: ");
                            Console.WriteLine("1 - Normal");
                            Console.WriteLine("2 - Below Normal");
                            priority = int.Parse(Console.ReadLine());
                        }

                        foreach (Process discord in processDiscord)
                        {
                            try
                            {
                                if (discord.WorkingSet64 >= mainDiscordProcess.WorkingSet64)
                                {
                                    mainDiscordProcess.ProcessorAffinity = (IntPtr)afinityDiscord;
                                    if (priority == 1)
                                    {
                                        mainDiscordProcess.PriorityClass = ProcessPriorityClass.Normal;
                                    }
                                    if (priority == 2)
                                    {
                                        mainDiscordProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                                    }
                                    Console.WriteLine($"Best afinity defined for Discord (PID: {discord.Id}).");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\nError to define afinity for Discord, your discord is probably closed.\nError message({ex.Message})");
                            }
                        }
                        Thread.Sleep(60 * 1000);
                    }
                    if (test == 3)
                    {
                        if (processOsu.Length == 0 || processDiscord.Length == 0)
                        {
                            Console.WriteLine($"You need to open osu! and Discord. Trying again in {count} seconds.");
                            Thread.Sleep(count * 1000);
                            count += count;
                            continue;
                        }
                        Console.WriteLine($"You have {avaliableThreads} remaining");
                        while (box > 0)
                        {

                            Console.WriteLine("How many threads for osu?");
                            osuCores = int.Parse(Console.ReadLine());
                            box -= osuCores;

                            Console.WriteLine("How many threads for Discord?");
                            discordCores = int.Parse(Console.ReadLine());
                            box -= discordCores;

                            if (box > 0)
                            {
                                Console.WriteLine($"{box} threads remaining, you need to use all threads.");
                                box = avaliableThreads;
                            }
                        }


                        for (int i = 0; i < avaliableThreads; i++)
                        {
                            if (i < osuCores)
                            {
                                afinityOsu |= 1L << i;
                            }
                            else
                            {
                                afinityDiscord |= 1L << i;
                            }
                        }

                        if (priority == 0)
                        {
                            Console.WriteLine("Priority Level: ");
                            Console.WriteLine("1 - Normal");
                            Console.WriteLine("2 - Below Normal");
                            priority = int.Parse(Console.ReadLine());
                        }

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
                                    if (priority == 1)
                                    {
                                        mainDiscordProcess.PriorityClass = ProcessPriorityClass.Normal;
                                    }
                                    if (priority == 2)
                                    {
                                        mainDiscordProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                                    }
                                    Console.WriteLine($"Best afinity defined for Discord (PID: {discord.Id}).");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\nError to define afinity for Discord, your discord is probably closed.\nError message({ex.Message})");
                            }
                        }
                        //Console.WriteLine(osuCores);
                        //Console.WriteLine(discordCores);
                        Thread.Sleep(5 * 60 * 1000);
                    }
                }
            }
        }
    }
}
