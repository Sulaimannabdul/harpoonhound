using System;
using System.IO;
using System.Net;
using System.Threading;

namespace AccountChecker.Globals
{
    internal class Menu
    {
        public static void LoginMenu()
        {
            // When authenticating users call you auth login here
            MainMenu();
        }

        public static void MainMenu()
        {
            Console.Clear();
            Theme.Art.Display();
            RPC.Handler.UpdatePresence("Main Menu", $"{Tool.Toolname} by {Tool.Author}");
            Console.Title = $"{Tool.Toolname} ~ {Tool.Version} ~ Main Menu";

            Theme.Art.Prefix("1", "Start Checker\n");
            Theme.Art.Prefix("2", "Scrape Proxies\n");
            Theme.Art.Prefix("3", "Exit\n");

            ConsoleKey cki;

            cki = Console.ReadKey(true).Key;

            switch (cki)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    StartMenu();
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    ProxyMenu();
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Environment.Exit(0);
                    break;
            }
        }

        public static void StartMenu()
        {
            Console.Clear();
            Theme.Art.Display();
            RPC.Handler.UpdatePresence("Loading Combolist", $"{Tool.Toolname} by {Tool.Author}");
            Console.Title = $"{Tool.Toolname} ~ {Tool.Version} ~ Load Combos";

            Theme.Art.Prefix(Globals.Tool.Toolname, "Load Your Combolist\n");
            Utils.Import.LoadCombos();

            Console.Clear();
            Theme.Art.Display();
            Theme.Art.Prefix(Globals.Tool.Toolname, "Choose Your Proxytype\n");

            Theme.Art.Prefix("1", "Https\n");
            Theme.Art.Prefix("2", "Socks4\n");
            Theme.Art.Prefix("3", "Socks5\n");
            Theme.Art.Prefix("4", "Proxyless\n");
            Theme.Art.Prefix(">", "");

            int ProxyType = int.Parse(Console.ReadLine());

            switch (ProxyType)
            {
                case 1:
                    Globals.Tool.ProxyType = "Https";
                    break;

                case 2:
                    Globals.Tool.ProxyType = "Socks4";
                    break;

                case 3:
                    Globals.Tool.ProxyType = "Socks5";
                    break;

                case 4:
                    Globals.Tool.ProxyType = "Proxyless";
                    break;
            }

            if (ProxyType != 4)
            {
                Console.Clear();
                Theme.Art.Display();
                RPC.Handler.UpdatePresence("Loading Proxylist", $"{Tool.Toolname} by {Tool.Author}");
                Console.Title = $"{Tool.Toolname} ~ {Tool.Version} ~ Load Proxylist";

                Theme.Art.Prefix(Globals.Tool.Toolname, "Load Your Proxylist");
                Utils.Import.LoadProxies();
            }

            Console.Title = $"{Tool.Toolname} ~ {Tool.Version} ~ Threads Amount";
            Console.Clear();
            Theme.Art.Display();
            RPC.Handler.UpdatePresence("Choosing Threads", $"{Tool.Toolname} by {Tool.Author}");

            Theme.Art.Prefix(Globals.Tool.Toolname, "How Many Threads?\n");
            Theme.Art.Prefix(">", "");

            Globals.Tool.Threads = int.Parse(Console.ReadLine());

            Console.Title = $"{Tool.Toolname} ~ {Tool.Version} ~ Starting Checker";
            Console.Clear();
            Theme.Art.Display();
            RPC.Handler.UpdatePresence("Starting Checker", $"{Tool.Toolname} by {Tool.Author}");

            // Load Threads

            new Thread(new ThreadStart(Utils.Functions.UpdateTitle)).Start();
            new Thread(new ThreadStart(Utils.Functions.CalculateCPM)).Start();
            new Thread(new ThreadStart(Utils.Functions.RPCUpdater)).Start();
            new Thread(new ThreadStart(Utils.Functions.PauseThreads)).Start();

            // Loop Main Checker

            Theme.Art.Prefix("Info", "'P' to Pause | 'R' to Resume\n\n");

            var num = 0;
            while (num < Tool.Threads)
            {
                var Thread = new Thread(new ThreadStart(Utils.Checker.Check));
                Thread.Start();
                Globals.Tool.ActiveThreads.Add(Thread);
                num++;
                Globals.Tool.RunningThreads++;
                if (num >= Tool.Threads) break;
            }
        }

        public static void ProxyMenu()
        {
            Console.Clear();
            Theme.Art.Display();
            RPC.Handler.UpdatePresence("Scraping Proxies", $"{Tool.Toolname} by {Tool.Author}");
            Console.Title = $"{Tool.Toolname} ~ {Tool.Version} ~ Proxy Menu";

            Console.Clear();
            Theme.Art.Display();
            Theme.Art.Prefix(Globals.Tool.Toolname, "Choose Your Proxytype\n");

            Theme.Art.Prefix("1", "Https\n");
            Theme.Art.Prefix("2", "Socks4\n");
            Theme.Art.Prefix("3", "Socks5\n");
            Theme.Art.Prefix(">", "");

            int ProxyType = int.Parse(Console.ReadLine());

            var Proxies = "";
            var Type = "";

            using (WebClient client = new WebClient())
            {
                switch (ProxyType)
                {
                    case 1:
                        Globals.Tool.ProxyType = "Https";
                        Type = "Https";
                        Proxies = client.DownloadString("https://api.proxyscrape.com/v2/?request=displayproxies&protocol=http&timeout=10000&country=all&ssl=all&anonymity=all");
                        break;

                    case 2:
                        Globals.Tool.ProxyType = "Socks4";
                        Type = "Socks4";
                        Proxies = client.DownloadString("https://api.proxyscrape.com/v2/?request=displayproxies&protocol=socks4&timeout=10000&country=all&ssl=all&anonymity=all");
                        break;

                    case 3:
                        Globals.Tool.ProxyType = "Socks5";
                        Type = "Socks5";
                        Proxies = client.DownloadString("https://api.proxyscrape.com/v2/?request=displayproxies&protocol=socks5&timeout=10000&country=all&ssl=all&anonymity=all");
                        break;
                }
            }

            var Proxylist = Proxies.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + $"Proxies"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + $"Proxies");
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"Proxies\\{Type}.txt"))
            {
                var c = File.Create(AppDomain.CurrentDomain.BaseDirectory + $"Proxies\\{Type}.txt");
                c.Close();
            }

            File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + $"Proxies\\{Type}.txt", Proxylist);
            Theme.Art.Prefix("Info", $"Scraped {Proxylist.Length} Proxies!");
            Thread.Sleep(1000);
            MainMenu();
        }
    }
}
