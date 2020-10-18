using OpenQA.Selenium;
using System;
using System.Threading;
using Zero_Web_GetGameContent.Model;

namespace Tests.GeneralInfo
{
    internal class Epic
    {

        //NOCHMAL TESTEN

        private static IWebDriver driver = Program.driver;
        private static ItemPrice price;
        //private static LanguageContent languageContent;

        public static void StartEpicTest(string searchURL)
        {

            price = new ItemPrice();
            //languageContent = new LanguageContent();

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);

            //Game Info
            GetGameInfo();

            //Page Info From
            Console.WriteLine("Epic");

            Console.WriteLine("\n\n");

            //Create DB Entry
            //PageName
            //PageLink
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

            if (!Functions.elementExists("css-1cxdtsr-NavigationVertical-styles__subNavLabel"))
            {
                Console.WriteLine("Name: " + Functions.FindElement("css-1he0b79-NavItemHeading__heading").FindElement(By.TagName("span")).Text);//Name
                //storeItem.GameName = Functions.FindElement("css-1he0b79-NavItemHeading__heading").FindElement(By.TagName("span")).Text;
            }
            else
            {
                Console.WriteLine("Name: " + Functions.FindElement("css-1cxdtsr-NavigationVertical-styles__subNavLabel").FindElement(By.TagName("span")).Text);//Name
                //storeItem.GameName = Functions.FindElement("css-1cxdtsr-NavigationVertical-styles__subNavLabel").FindElement(By.TagName("span")).Text;
            }

            //Price
            if (!Functions.elementExists("css-tezx95-PurchaseTag__tag-PurchaseTag__shouldUseTheme-PurchaseTag__main"))
            {
                Console.WriteLine("Preis: " + Functions.FindText("css-csbeaa"));

                price.Free = false;
                price.GamePrice = Functions.FindText("css-csbeaa");
            }
            else
            {
                Console.WriteLine("Price Old: " + Functions.FindText("css-csbeaa"));
                Console.WriteLine("Price New: " + Functions.FindText("css-1u3oa55-Price__discount"));
                Console.WriteLine("Reduce: " + Functions.FindText("css-tezx95-PurchaseTag__tag-PurchaseTag__shouldUseTheme-PurchaseTag__main"));

                price.Free = false;
                price.PriceOld = Functions.FindText("css-csbeaa");
                price.PriceNew = Functions.FindText("css-1u3oa55-Price__discount");
                price.Reduced = Functions.FindText("css-tezx95-PurchaseTag__tag-PurchaseTag__shouldUseTheme-PurchaseTag__main");
            }

        }

    }
}
