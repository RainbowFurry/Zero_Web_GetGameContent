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

                //StartStoreCreateEntrys();

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
