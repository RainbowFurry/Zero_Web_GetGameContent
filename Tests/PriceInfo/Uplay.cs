using OpenQA.Selenium;
using System;
using System.Threading;
using Zero_Web_GetGameContent.Model;

namespace Tests.GeneralInfo
{
    internal class Uplay
    {

        //PREIS WIRD NICHT GEHOLT

        private static IWebDriver driver = Program.driver;
        private static StoreItem storeItem;
        private static LanguageContent languageContent;

        public static void StartUplayTest(string searchURL)
        {

            storeItem = new StoreItem();
            languageContent = new LanguageContent();
            storeItem.LanguageContents = new LanguageContent[10];

            //ID
            storeItem.ID = "";

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);

            //Game Info
            GetGameInfo();

            //Page Info From
            Console.WriteLine("Uplay");
            storeItem.InfoFrom = "Uplay";

            storeItem.LanguageContents[0] = languageContent;
            Console.WriteLine("\n\n");

            //Create DB Entry
            //MongoDBManager.CreateEntry("StoreItemTEMP", storeItem.ToBsonDocument());

            Thread.Sleep(2000);

        }

        private static void SearchGame(String Game)
        {
            driver.Navigate().GoToUrl(Game);
            Thread.Sleep(3000);

            if (Functions.elementExists("age-input"))
            {
                driver.FindElement(By.ClassName("age-input")).Click();
                driver.FindElement(By.ClassName("age-input")).SendKeys("22" + Keys.Enter);
                Thread.Sleep(2000);
            }

        }

        private static void GetGameInfo()
        {
            Console.WriteLine("Name: " + Functions.FindText("product-name"));//Name
            storeItem.GameName = Functions.FindText("product-name");

            //Price
            Price price = new Price();
            if (!Functions.elementExists("deal-percentage"))
            {
                Console.WriteLine("Preis: " + Functions.FindText("price-sales"));

                price.Free = false;
                price.GamePrice = Functions.FindText("price-sales");
            }
            else
            {
                Console.WriteLine("Price Old: " + Functions.FindText("price-item"));
                Console.WriteLine("Price New: " + Functions.FindText("standard-price"));
                Console.WriteLine("Reduce: " + Functions.FindText("deal-percentage"));

                price.Free = false;
                price.PriceOld = Functions.FindText("price-item");
                price.PriceNew = Functions.FindText("standard-price");
                price.Reduced = Functions.FindText("deal-percentage");
            }
            storeItem.Price = price;

        }

    }
}
