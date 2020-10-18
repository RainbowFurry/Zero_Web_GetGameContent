using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using TestsGeneralInfo;
using Zero_Web_GetGameContent.GeneralInfo;
using Zero_Web_GetGameContent.Manager;

namespace Tests
{
    internal class Program
    {

        public static IWebDriver driver = new ChromeDriver();

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

                MongoDBManager.CreateConnection();
                StartStoreCreateEntrys();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void StartStoreCreateEntrys()
        {
            LoadGameFile loadGameFile = new LoadGameFile();
            loadGameFile.LoadGamesFromFile();

            foreach (string StoreItemURL in loadGameFile.StoreItemURLs)
            {
                if (StoreItemURL.StartsWith("https://store.steampowered.com/"))
                {
                    Steam.StartSteamTest(StoreItemURL);
                }
                else if (StoreItemURL.StartsWith("https://www.microsoft.com"))
                {
                    MicrosoftStore.StartMicrosoftStoreTest(StoreItemURL);
                }
            }
        }

    }
}
