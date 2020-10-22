using OpenQA.Selenium;
using System;
using System.IO;
using System.Threading;
using Tests;

namespace Zero_Web_GetGameContent.AutoGrab
{
    public class Xbox
    {

        private static IWebDriver driver = Program.driver;
        private static string path = Environment.CurrentDirectory + "NewGamesToLoadMicroSoft.txt";

        //https://www.xbox.com/de-DE/games/all-games?xr=shellnav
        public static void StartMicrosoftStoreAutoGrab(string searchURL)
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

            int maxPageSize = driver.FindElement(By.ClassName("m-pagination")).FindElements(By.ClassName("paginatenum")).Count;
            Console.WriteLine(maxPageSize);

            //Foreach max Page
            for (int i = 0; i < maxPageSize; i++)
            {
                //Foreach Items
                foreach (IWebElement element in driver.FindElement(By.ClassName("gameDivsWrapper")).FindElements(By.ClassName("m-product-placement-item")))
                {
                    File.AppendAllText(path, element.FindElement(By.TagName("a")).GetAttribute("href") + "\n");
                }

                Thread.Sleep(2000);
                driver.FindElement(By.ClassName("paginatenext")).FindElement(By.TagName("a")).Click();

            }

        }

    }
}
