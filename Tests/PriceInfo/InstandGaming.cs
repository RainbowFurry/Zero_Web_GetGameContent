using OpenQA.Selenium;
using System;
using System.Threading;
using Tests;
using Zero_Web_GetGameContent.Model;

namespace Zero_Web_GetGameContent.GeneralInfo
{
    internal class InstandGaming
    {

        private static IWebDriver driver = Program.driver;
        private static ItemPrice price;

        public static void StartInstandGamingTest(string searchURL)
        {

            price = new ItemPrice();

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);

            //Game Info
            GetGameInfo();

            //Page Info From
            Console.WriteLine("InstandGaming");

            Console.WriteLine("\n\n");

            //Create DB Entry
            //MongoDBManager.CreateEntry("StoreItemTEMP", storeItem.ToBsonDocument());

            Thread.Sleep(2000);

        }

        private static void SearchGame(String Game)
        {
            driver.Navigate().GoToUrl(Game);
            Thread.Sleep(3000);
        }

        private static void GetGameInfo()
        {
            Console.WriteLine("Name: " + driver.FindElement(By.ClassName("title")).FindElement(By.TagName("h1")).Text);//Name
            //storeItem.GameName = driver.FindElement(By.ClassName("title")).FindElement(By.TagName("h1")).Text;

            //Price
            if (driver.FindElement(By.ClassName("price")).Text == "N/A")
            {
                Console.WriteLine("Preis: " + "-");

                price.Free = false;
                price.GamePrice = "-";
            }
            else
            {
                Console.WriteLine("Price Old: " + Functions.FindText("retail"));
                Console.WriteLine("Price New: " + Functions.FindText("price"));
                Console.WriteLine("Reduce: " + Functions.FindText("discount"));

                price.Free = false;
                price.PriceOld = Functions.FindText("retail");
                price.PriceNew = Functions.FindText("price");
                price.Reduced = Functions.FindText("discount");
            }

        }

    }
}
