﻿using OpenQA.Selenium;
using System;
using System.Threading;
using Zero_Web_GetGameContent.Model;

namespace Tests.GeneralInfo
{
    internal class GoG
    {

        private static IWebDriver driver = Program.driver;
        private static ItemPrice price;

        public static void StartGoGTest(string searchURL)
        {

            price = new ItemPrice();

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);

            //Game Info
            GetGameInfo();

            //Page Info From
            Console.WriteLine("GoG");

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
            Console.WriteLine("Name: " + Functions.FindText("productcard-basics__title"));//Name
            //storeItem.GameName = Functions.FindText("productcard-basics__title");

            //Price
            if (!Functions.elementExists("product-actions-price__discount"))
            {
                Console.WriteLine("Preis: " + Functions.FindText("product-actions-price__final-amount"));

                price.Free = false;
                price.GamePrice = Functions.FindText("product-actions-price__final-amount");
            }
            else
            {
                Console.WriteLine("Price Old: " + Functions.FindText("product-actions-price__base-amount"));
                Console.WriteLine("Price New: " + Functions.FindText("product-actions-price__final-amount"));
                Console.WriteLine("Reduce: " + Functions.FindText("product-actions-price__discount"));

                price.Free = false;
                price.PriceOld = Functions.FindText("product-actions-price__base-amount");
                price.PriceNew = Functions.FindText("product-actions-price__final-amount");
                price.Reduced = Functions.FindText("product-actions-price__discount");
            }

        }

    }
}
