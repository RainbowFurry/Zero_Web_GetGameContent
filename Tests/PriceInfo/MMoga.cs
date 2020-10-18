using OpenQA.Selenium;
using System;
using System.Threading;
using Zero_Web_GetGameContent.Model;

namespace Tests.GeneralInfo
{
    internal class MMoga
    {

        private static IWebDriver driver = Program.driver;
        private static ItemPrice price;

        public static void StartMMogaTest(string searchURL)
        {

            price = new ItemPrice();

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);

            //Game Info
            GetGameInfo();

            //Page Info From
            Console.WriteLine("MMoga");

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
            Console.WriteLine("Name: " + Functions.FindText("title"));//Name
            //storeItem.GameName = Functions.FindText("title");

            //Price
            Console.WriteLine("Price Old: " + Functions.FindElement("proMoney").FindElement(By.TagName("del")).Text.Replace("UVP", "").Replace("?", ""));
            Console.WriteLine("Price New: " + Functions.FindElement("proMoney").FindElement(By.ClassName("now")).Text.Replace("?", ""));
            Console.WriteLine("Reduce: " + "");

            price.Free = false;
            price.PriceOld = Functions.FindElement("proMoney").FindElement(By.TagName("del")).Text.Replace("UVP", "").Replace("?", "");
            price.PriceNew = Functions.FindElement("proMoney").FindElement(By.ClassName("now")).Text.Replace("?", "");
            price.Reduced = "";

        }

    }
}
