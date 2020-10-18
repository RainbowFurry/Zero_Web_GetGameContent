using MongoDB.Bson;
using OpenQA.Selenium;
using System;
using System.Threading;
using Tests;
using Zero_Web_GetGameContent.Manager;
using Zero_Web_GetGameContent.Model;

namespace TestsGeneralInfo
{
    internal class Steam
    {

        private static IWebDriver driver = Program.driver;
        private static StoreItem storeItem;
        private static LanguageContent languageContent;
        private static ItemPrice price;
        private static StoreItemPrice storeItemPrice;
        private static StoreItemLanguage storeItemLanguage;
        private static Language language;

        public static void StartSteamTest(string searchURL)
        {

            storeItem = new StoreItem();
            languageContent = new LanguageContent();
            storeItemPrice = new StoreItemPrice();
            price = new ItemPrice();
            storeItemLanguage = new StoreItemLanguage();
            language = new Language();
            storeItem.LanguageContents = new LanguageContent[10];

            //ID
            storeItem.ID = Guid.NewGuid().ToString();
            storeItemPrice.ID = storeItem.ID;
            storeItemLanguage.ID = storeItem.ID;

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);

            //FSK Check
            CheckFSK();

            //Game Info
            GetGameInfo();

            //Game Tags
            GetGameTags();

            //Get FSK Info
            GetFSKInfo();

            //Game System Requirement
            GetSystemInfo();

            //Page Info From
            Console.WriteLine("Steam");
            storeItem.InfoFrom = "Steam";

            //Category
            storeItem.Category = "Games";

            storeItem.LanguageContents[0] = languageContent;
            Console.WriteLine("\n\n");

            //Create DB Entry
            MongoDBManager.CreateEntry("StoreItemTEMP", storeItem.ToBsonDocument());
            storeItemLanguage.language = new Language[] { language };
            MongoDBManager.CreateEntry("StoreItemLanguageTEMP", storeItem.ToBsonDocument());
            //MongoDBManager.CreateEntry("StoreItemDLCTEMP", storeItem.ToBsonDocument());
            //MongoDBManager.CreateEntry("StoreItemBundleTEMP", storeItem.ToBsonDocument());
            price.PageURL = searchURL;
            price.PageName = "Steam";
            storeItemPrice.price = new ItemPrice[] { price };
            MongoDBManager.CreateEntry("StoreItemPriceTEMP", storeItem.ToBsonDocument());

            Thread.Sleep(2000);

        }

        private static void SearchGame(String Game)
        {
            driver.Navigate().GoToUrl(Game);
            Thread.Sleep(3000);
        }

        private static void CheckFSK()
        {
            if (Functions.elementExists("agegate_birthday_selector"))
            {
                driver.FindElement(By.Name("ageYear")).Click();
                driver.FindElement(By.Name("ageYear")).SendKeys("1977");
                driver.FindElement(By.Name("ageYear")).Click();
                Functions.FindElement("btnv6_blue_hoverfade").Click();
                Thread.Sleep(3000);
            }
        }



        private static void GetSystemInfo()
        {
            IWebElement webElement = Functions.FindElement("bb_ul");
            Console.WriteLine("Minimal System:");
            Console.WriteLine("Betriebssystem: " + webElement.FindElements(By.TagName("li"))[0].Text);
            Console.WriteLine("Prozessor: " + webElement.FindElements(By.TagName("li"))[1].Text);
            Console.WriteLine("Arbeitsspeicher: " + webElement.FindElements(By.TagName("li"))[2].Text);
            Console.WriteLine("Grafik: " + webElement.FindElements(By.TagName("li"))[3].Text);
            Console.WriteLine("DirecttX: " + webElement.FindElements(By.TagName("li"))[4].Text);
            Console.WriteLine("Storage: " + webElement.FindElements(By.TagName("li"))[5].Text);

            SystemInfo systemInfos = new SystemInfo();

            systemInfos.OS = webElement.FindElements(By.TagName("li"))[0].Text;
            systemInfos.CPU = webElement.FindElements(By.TagName("li"))[1].Text;
            systemInfos.RAM = webElement.FindElements(By.TagName("li"))[2].Text;
            systemInfos.GPU = webElement.FindElements(By.TagName("li"))[3].Text;
            systemInfos.DirectX = webElement.FindElements(By.TagName("li"))[4].Text;
            systemInfos.Storage = webElement.FindElements(By.TagName("li"))[5].Text;

            storeItem.SystemInfoMin = systemInfos;

        }

        private static void GetGameTags()
        {
            IWebElement elements = Functions.FindElement("glance_tags");
            int index = 0;
            string[] gameTag = new string[elements.FindElements(By.ClassName("app_tag")).Count];

            foreach (IWebElement element in elements.FindElements(By.ClassName("app_tag")))
            {
                if (!String.IsNullOrEmpty(element.Text) && !element.Text.Contains("+"))
                {
                    Console.WriteLine("Game Tag: " + element.Text);
                    gameTag[index] = element.Text;
                }
                index++;
            }
            language.GameTags = gameTag;
            languageContent.GameTags = gameTag;
        }

        private static void GetGameInfo()
        {
            Console.WriteLine("Name: " + Functions.FindText("apphub_AppName"));//Name
            storeItem.GameName = Functions.FindText("apphub_AppName");

            //Price
            Price priceOld = new Price();
            if (Functions.elementExists("game_purchase_price"))
            {
                if (Functions.FindText("game_purchase_price") == "Free")
                {
                    Console.WriteLine("Preis: " + "Free");

                    priceOld.Free = true;
                    price.Free = true;
                }
                else
                {
                    Console.WriteLine("Preis: " + Functions.FindText("game_purchase_price"));

                    priceOld.Free = false;
                    price.Free = false;
                    priceOld.GamePrice = Functions.FindText("game_purchase_price");
                    price.GamePrice = Functions.FindText("game_purchase_price");
                }
            }
            else
            {
                Console.WriteLine("Price Old: " + Functions.FindText("discount_original_price"));
                Console.WriteLine("Price New: " + Functions.FindText("discount_final_price"));
                Console.WriteLine("Reduce: " + Functions.FindText("discount_pct"));

                priceOld.Free = false;
                priceOld.PriceOld = Functions.FindText("discount_original_price");
                priceOld.PriceNew = Functions.FindText("discount_final_price");
                priceOld.Reduced = Functions.FindText("discount_pct");
                price.Free = false;
                price.PriceOld = Functions.FindText("discount_original_price");
                price.PriceNew = Functions.FindText("discount_final_price");
                price.Reduced = Functions.FindText("discount_pct");
            }
            storeItem.Price = priceOld;

            Console.WriteLine("\nGame Image: " + driver.FindElement(By.ClassName("game_header_image_full")).GetAttribute("src"));
            storeItem.GameImage = driver.FindElement(By.ClassName("game_header_image_full")).GetAttribute("src");
            Console.WriteLine("Description: " + Functions.FindText("game_description_snippet"));//Short Description
            languageContent.ShortDescription = Functions.FindText("game_description_snippet");
            language.ShortDescription = Functions.FindText("game_description_snippet");
            Console.WriteLine("Release: " + Functions.FindText("date"));//Release
            storeItem.Release = Functions.FindText("date");

            //Developer
            IWebElement elementDevelopers = driver.FindElement(By.Id("developers_list"));
            int devCunter = 0;
            string[] dev = new string[30];
            string[] devLink = new string[30];
            foreach (IWebElement elementDeveloper in elementDevelopers.FindElements(By.TagName("a")))
            {
                Console.WriteLine("Developer: " + elementDeveloper.Text);
                dev[devCunter] = elementDeveloper.Text;
                Console.WriteLine("DeveloperLink: " + elementDeveloper.GetAttribute("href"));
                devLink[devCunter] = elementDeveloper.GetAttribute("href");
                devCunter++;
            }
            storeItem.Developer = dev;
            storeItem.DeveloperLink = devLink;

            //Publisher
            IWebElement elementPublishers = driver.FindElement(By.Id("developers_list"));
            foreach (IWebElement elementPublisher in elementPublishers.FindElements(By.TagName("a")))
            {
                Console.WriteLine("Publisher: " + elementPublisher.Text);
                storeItem.Publisher = elementPublisher.Text;
                Console.WriteLine("PublisherLink: " + elementPublisher.GetAttribute("href"));
                storeItem.PublisherLink = elementPublisher.GetAttribute("href");
            }

            //Genre
            IWebElement elementGenres = Functions.FindElement("details_block");
            int genreCounter = 0;
            string[] genre = new string[elementGenres.FindElements(By.TagName("a")).Count];
            foreach (IWebElement elementGenre in elementGenres.FindElements(By.TagName("a")))
            {
                Console.WriteLine("Genre: " + elementGenre.Text);
                genre[genreCounter] = elementGenre.Text;
                genreCounter++;
            }
            language.Genre = genre;
            languageContent.Genre = genre;

            //Early Access
            if (Functions.elementExists("early_access_header"))
            {
                Console.WriteLine("Early Access: true");
                storeItem.EarlyAccess = true;
            }
            else
            {
                Console.WriteLine("Early Access: false");
                storeItem.EarlyAccess = false;
            }

        }

        private static void GetFSKInfo()
        {
            if (Functions.elementExists("game_rating_icon"))
            {
                IWebElement element = Functions.FindElement("game_rating_icon").FindElements(By.TagName("img"))[0];

                if (element.GetAttribute("src").Contains("16.png"))
                {
                    Console.WriteLine("FSK: 16");
                    storeItem.FSK = "16";
                }
                else if (element.GetAttribute("src").Contains("18.png"))
                {
                    Console.WriteLine("FSK: 18");
                    storeItem.FSK = "18";
                }
                else if (element.GetAttribute("src").Contains("12.png"))
                {
                    Console.WriteLine("FSK: 12");
                    storeItem.FSK = "12";
                }
                else if (element.GetAttribute("src").Contains("6.png"))
                {
                    Console.WriteLine("FSK: 6");
                    storeItem.FSK = "6";
                }
                else
                {
                    Console.WriteLine("FSK: 0");
                    storeItem.FSK = "0";
                }
            }

        }

    }
}
