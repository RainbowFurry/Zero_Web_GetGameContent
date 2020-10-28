using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using TestsGeneralInfo;
using Zero_Web_GetGameContent.AutoGrab;
using Zero_Web_GetGameContent.GeneralInfo;
using Zero_Web_GetGameContent.Manager;

namespace Tests
{
    internal class Program
    {

        public static IWebDriver driver = new ChromeDriver();

        private static LoadGameFile loadGameFile;

        private static void Main(string[] args)
        {

            try
            {

                //Epic.StartEpicTest("https://www.epicgames.com/store/en-US/product/i-am-dead/home");
                //Epic.StartEpicTest("https://www.epicgames.com/store/en-US/product/killing-floor-2/home");

                //MMoga.StartMMogaTest("https://www.mmoga.de/Steam-Games/Age-of-Empires-III-Definitive-Edition-Steam-Key.html");
                //MMoga.StartMMogaTest("https://www.mmoga.de/Steam-Games/HITMAN-2.html");

                //Uplay.StartUplayTest("https://store.ubi.com/de/tom-clancy-s-ghost-recon-breakpoint-ultimate-edition/5cc81f626b54a4cd3c0e9d37.html");
                //Uplay.StartUplayTest("https://store.ubi.com/de/assassin-s-creed-valhalla/5e849c6c5cdf9a21c0b4e731.html");

                //InstandGaming.StartInstandGamingTest("https://www.instant-gaming.com/en/840-buy-game-gogcom-cyberpunk-2077/");
                //InstandGaming.StartInstandGamingTest("https://www.instant-gaming.com/en/5935-buy-game-steam-bayonetta-3/");

                //GoG.StartGoGTest("https://www.gog.com/game/cyberpunk_2077");
                //GoG.StartGoGTest("https://www.gog.com/game/the_signifier");

                loadGameFile = new LoadGameFile();
                loadGameFile.LoadGamesFromFile();

                MongoDBManager.CreateConnection();

                StartStoreCreateEntrys();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                foreach (string url in loadGameFile.StoreItemURLs)
                {
                    File.AppendAllText(Environment.CurrentDirectory + "ErrorGames.txt", url);
                }

            }
        }

        private static void StartStoreCreateEntrys()
        {
            foreach (string StoreItemURL in loadGameFile.StoreItemURLs)
            {
                if (StoreItemURL.StartsWith("https://store.steampowered.com/"))
                {
                    SteamGame(StoreItemURL);
                }
                else if (StoreItemURL.StartsWith("https://www.microsoft.com"))
                {
                    MicrosoftGame(StoreItemURL);
                }
                else if (StoreItemURL.StartsWith("https://www.xbox.com/"))
                {
                    Xbox.StartMicrosoftStoreAutoGrab(StoreItemURL);//MS AUTOGET GAMES
                }
                //loadGameFile.StoreItemURLs.Remove(StoreItemURL);
            }
        }

        private static void SteamGame(string StoreItemURL)
        {
            if (StoreItemURL.StartsWith("https://store.steampowered.com/") && !StoreItemURL.Contains("search/?filter=") && !StoreItemURL.Contains("/genre/") && !StoreItemURL.Contains("/tags/") && !StoreItemURL.Contains("/specials"))
            {
                Steam.StartSteamTest(StoreItemURL);//STEAM STORE ITEM
            }
            else if (StoreItemURL.StartsWith("https://store.steampowered.com/") && StoreItemURL.Contains("search/?filter="))
            {
                SteamGames.StartSteamAutoGrab(StoreItemURL);//Steam AUTOGET GAMES
            }
            else if (StoreItemURL.StartsWith("https://store.steampowered.com/") && StoreItemURL.Contains("/genre/"))
            {
                SteamGames.StartSteamAutoGrab(StoreItemURL);//Steam AUTOGET GAMES
            }
            else if (StoreItemURL.StartsWith("https://store.steampowered.com/") && StoreItemURL.Contains("/tags/"))
            {
                SteamGames.StartSteamAutoGrab(StoreItemURL);//Steam AUTOGET GAMES
            }
            else if (StoreItemURL.StartsWith("https://store.steampowered.com/") && StoreItemURL.Contains("/specials"))
            {
                SteamGames.StartSteamAutoGrab(StoreItemURL);//Steam AUTOGET GAMES
            }
        }

        private static void MicrosoftGame(string StoreItemURL)
        {
            if (StoreItemURL.StartsWith("https://www.microsoft.com"))
            {
                MicrosoftStore.StartMicrosoftStoreTest(StoreItemURL);//MS STORE ITEM
            }
        }

    }
}
