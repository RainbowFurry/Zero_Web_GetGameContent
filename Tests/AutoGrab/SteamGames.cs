using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
using System.Threading;
using Tests;

namespace Zero_Web_GetGameContent.AutoGrab
{
    public class SteamGames
    {

        private static IWebDriver driver = Program.driver;
        private static string path = Environment.CurrentDirectory + "NewGamesToLoadSteam.txt";

        //https://store.steampowered.com/search/?filter=topsellers
        public static void StartSteamAutoGrab(string searchURL)
        {

            //Search Link
            SearchGames(searchURL);

            //Get Games
            GetGames();

        }

        private static void SearchGames(string searchURL)
        {
            driver.Navigate().GoToUrl(searchURL);
            Thread.Sleep(3000);
        }

        private static void GetGames()
        {

            if (!File.Exists(path))
            {
                File.Create(path);
            }

            Thread.Sleep(5000);

            int maxPageSize = 50;
            int currentGame = 0;

            //Foreach max Page
            for (int i = 0; i < maxPageSize; i++)
            {
                Thread.Sleep(2000);
                IWebElement scroll = driver.FindElement(By.Id("search_resultsRows")).FindElements(By.TagName("a"))[driver.FindElement(By.Id("search_resultsRows")).FindElements(By.TagName("a")).Count - 1];
                int loop = 0;

                foreach (IWebElement element in driver.FindElement(By.Id("search_resultsRows")).FindElements(By.TagName("a")))
                {
                    if (loop >= currentGame)
                    {
                        File.AppendAllText(path, element.GetAttribute("href") + "\n");
                        currentGame++;
                        loop++;
                    }
                }

                Thread.Sleep(2000);
                Actions actions = new Actions(driver);
                actions.MoveToElement(scroll);
                actions.Perform();
                //SCROLL DOWN 

            }

        }

    }
}
