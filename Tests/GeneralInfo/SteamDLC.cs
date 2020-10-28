using MongoDB.Bson;
using OpenQA.Selenium;
using System;
using System.Threading;
using Tests;
using Zero_Web_GetGameContent.Manager;
using Zero_Web_GetGameContent.Model;

namespace Zero_Web_GetGameContent.GeneralInfo
{
    public class SteamDLC
    {

        public static string GetSteamDLC(string dlcURL, StoreItem mainGame)
        {
            return StartSteamTest(dlcURL, mainGame);
        }

        private static readonly IWebDriver driver = Program.driver;
        private static StoreItem storeItem;
        private static ItemPrice price;
        private static StoreItemPrice storeItemPrice;
        private static StoreItemLanguage storeItemLanguage;
        private static Language language;

        private static string StartSteamTest(string searchURL, StoreItem mainGame)
        {

            storeItem = new StoreItem();
            storeItemPrice = new StoreItemPrice();
            price = new ItemPrice();
            storeItemLanguage = new StoreItemLanguage();
            language = new Language();

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


            if (!MongoDBManager.DocumentExists("StoreItemDLCTEMP", storeItem))
            {
                //Game Tags
                GetGameTags();

                //Get GameImages
                GetDisplayImages();

                //Get FSK Info
                GetFSKInfo();

                //Game System Requirement
                GetSystemInfo(mainGame);

                //Page Info From
                Console.WriteLine("Steam");
                storeItem.InfoFrom = "Steam";

                //Category
                storeItem.Category = "Games";

                Console.WriteLine("\n\n");

                //Create DB Entry
                MongoDBManager.CreateEntry("StoreItemDLCTEMP", storeItem.ToBsonDocument());
                storeItemLanguage.language = new Language[] { language };
                storeItemLanguage.PageURL = searchURL;
                storeItemLanguage.PageName = "Steam";
                MongoDBManager.CreateEntry("StoreItemDLCLanguageTEMP", storeItemLanguage.ToBsonDocument());
                price.PageURL = searchURL;
                price.PageName = "Steam";
                storeItemPrice.price = new ItemPrice[] { price };
                MongoDBManager.CreateEntry("StoreItemDLCPriceTEMP", storeItemPrice.ToBsonDocument());

                Thread.Sleep(2000);
                return storeItem.ID;
            }
            return storeItem.ID;
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



        private static void GetSystemInfo(StoreItem mainGame)
        {
            storeItem.SystemInfoMin = mainGame.SystemInfoMin;
            storeItem.SystemInfoMax = mainGame.SystemInfoMax;
        }

        private static void GetGameTags()
        {
            IWebElement elements = Functions.FindElement("glance_tags");
            int index = 0;
            string[] gameTag = new string[elements.FindElements(By.ClassName("app_tag")).Count];

            foreach (IWebElement element in elements.FindElements(By.ClassName("app_tag")))
            {
                //if (!String.IsNullOrEmpty(element.Text) && !element.Text.Contains("+"))
                //{
                //    Console.WriteLine("Game Tag: " + element.Text);
                //    gameTag[index] = element.Text;
                //}
                //index++;
                if (element.Text.Contains("+"))
                {
                    element.Click();
                    Thread.Sleep(2000);

                    gameTag = new string[driver.FindElement(By.ClassName("app_tags")).FindElements(By.ClassName("app_tag_control")).Count];

                    foreach (IWebElement el in driver.FindElement(By.ClassName("app_tags")).FindElements(By.ClassName("app_tag_control")))
                    {
                        Console.WriteLine("Game Tag: " + el.FindElement(By.TagName("a")).Text);
                        gameTag[index] = el.FindElement(By.TagName("a")).Text.Replace("Game Tag:", "");
                        index++;
                    }

                    //Close
                    driver.FindElement(By.ClassName("btn_grey_steamui")).Click();
                    Thread.Sleep(2000);

                }
            }
            language.GameTags = gameTag;
        }

        private static void GetGameInfo()
        {
            Console.WriteLine("Name: " + Functions.FindText("apphub_AppName"));//Name
            storeItem.GameName = Functions.FindText("apphub_AppName");

            Console.WriteLine(driver.FindElement(By.ClassName("game_area_description")).Text);
            language.LongDescription = driver.FindElement(By.ClassName("game_area_description")).Text;

            //Price
            if (Functions.elementExists("game_purchase_price"))
            {
                if (Functions.FindText("game_purchase_price") == "Free")
                {
                    Console.WriteLine("Preis: " + "Free");

                    price.Free = true;
                }
                else
                {
                    Console.WriteLine("Preis: " + Functions.FindText("game_purchase_price"));

                    price.Free = false;
                    price.GamePrice = Functions.FindText("game_purchase_price");
                }
            }
            else
            {
                if (Functions.elementExists("discount_original_price"))
                {
                    Console.WriteLine("Price Old: " + Functions.FindText("discount_original_price"));
                    Console.WriteLine("Price New: " + Functions.FindText("discount_final_price"));
                    Console.WriteLine("Reduce: " + Functions.FindText("discount_pct"));

                    price.Free = false;
                    price.PriceOld = Functions.FindText("discount_original_price");
                    price.PriceNew = Functions.FindText("discount_final_price");
                    price.Reduced = Functions.FindText("discount_pct");
                }
            }

            Console.WriteLine("\nGame Image: " + driver.FindElement(By.ClassName("game_header_image_full")).GetAttribute("src"));
            storeItem.GameImage = driver.FindElement(By.ClassName("game_header_image_full")).GetAttribute("src");
            Console.WriteLine("Release: " + Functions.FindText("date"));//Release
            storeItem.Release = Functions.FindText("date");

            if (driver.FindElements(By.ClassName("game_description_snippet")).Count > 0)
            {
                Console.WriteLine("Description: " + Functions.FindText("game_description_snippet"));//Short Description
                language.ShortDescription = Functions.FindText("game_description_snippet");
            }
            else
            {
                Console.WriteLine("Description: " + Functions.FindText("game_area_description"));//Short Description
                language.ShortDescription = Functions.FindText("game_area_description");
            }

            //Developer
            IWebElement elementDevelopers = driver.FindElement(By.Id("developers_list"));
            int devCunter = 0;
            string[] dev = new string[elementDevelopers.FindElements(By.TagName("a")).Count];
            string[] devLink = new string[elementDevelopers.FindElements(By.TagName("a")).Count];
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

        private static void GetDisplayImages()
        {

            DisplayImages[] displayImages = new DisplayImages[driver.FindElement(By.Id("highlight_strip_scroll")).FindElements(By.ClassName("highlight_strip_item")).Count];
            DisplayImages displayImage;
            int counter = 0;

            foreach (IWebElement element in driver.FindElement(By.Id("highlight_strip_scroll")).FindElements(By.ClassName("highlight_strip_item")))
            {
                displayImage = new DisplayImages();
                string elementdisplayImage = element.FindElement(By.TagName("img")).GetAttribute("src");
                Console.WriteLine(elementdisplayImage);
                displayImage.URL = elementdisplayImage;

                if (elementdisplayImage.Contains("movie"))
                {
                    displayImage.Video = true;
                }
                else
                {
                    displayImage.Video = false;
                }
                displayImages[counter] = displayImage;
                counter++;

            }
            storeItem.GameImages = displayImages;
            //add to db

        }

    }
}
