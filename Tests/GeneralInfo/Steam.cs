using MongoDB.Bson;
using OpenQA.Selenium;
using System;
using System.Threading;
using Tests;
using Zero_Web_GetGameContent.GeneralInfo;
using Zero_Web_GetGameContent.Manager;
using Zero_Web_GetGameContent.Model;

namespace TestsGeneralInfo
{
    internal class Steam
    {

        private static readonly IWebDriver driver = Program.driver;
        private static StoreItem storeItem;
        private static ItemPrice price;
        private static StoreItemPrice storeItemPrice;
        private static StoreItemLanguage storeItemLanguage;
        private static Language language;
        private static StoreItemDLC storeItemDLC;

        public static void StartSteamTest(string searchURL)
        {

            storeItem = new StoreItem();
            storeItemPrice = new StoreItemPrice();
            price = new ItemPrice();
            storeItemLanguage = new StoreItemLanguage();
            language = new Language();
            storeItemDLC = new StoreItemDLC();

            //ID
            storeItem.ID = Guid.NewGuid().ToString();
            storeItemPrice.ID = storeItem.ID;
            storeItemLanguage.ID = storeItem.ID;
            storeItemDLC.GameID = storeItem.ID;

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);
            //FSK Check
            CheckFSK();

            if (driver.FindElements(By.ClassName("apphub_AppName")).Count > 0)
            {
                if (!driver.FindElement(By.ClassName("apphub_AppName")).Text.Contains("Add-On") && !driver.FindElement(By.ClassName("apphub_AppName")).Text.Contains("Soundtrack"))
                {

                    //Game Info
                    GetGameInfo();

                    if (!MongoDBManager.DocumentExists("StoreItemTEMP", storeItem))
                    {

                        //Game Tags
                        GetGameTags();

                        //Get Images
                        GetDisplayImages();

                        //Get FSK Info
                        GetFSKInfo();

                        //Game System Requirement
                        GetSystemInfo();

                        //Page Info From
                        Console.WriteLine("Steam");
                        storeItem.InfoFrom = "Steam";

                        //Category
                        storeItem.Category = "Games";

                        Console.WriteLine("\n\n");

                        //Create DB Entry
                        MongoDBManager.CreateEntry("StoreItemTEMP", storeItem.ToBsonDocument());
                        storeItemLanguage.language = new Language[] { language };
                        storeItemLanguage.PageURL = searchURL;
                        storeItemLanguage.PageName = "Steam";
                        MongoDBManager.CreateEntry("StoreItemLanguageTEMP", storeItemLanguage.ToBsonDocument());
                        //MongoDBManager.CreateEntry("StoreItemBundleTEMP", storeItem.ToBsonDocument());
                        price.PageURL = searchURL;
                        price.PageName = "Steam";
                        storeItemPrice.price = new ItemPrice[] { price };
                        MongoDBManager.CreateEntry("StoreItemPriceTEMP", storeItemPrice.ToBsonDocument());

                        //GET DLC
                        GetDLC();
                        if (storeItemDLC.DLCID != null)
                            MongoDBManager.CreateEntry("StoreItemDLCLinkerTEMP", storeItemDLC.ToBsonDocument());
                        Thread.Sleep(2000);
                    }
                }
            }
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
            SystemInfo systemInfosMin = new SystemInfo();
            SystemInfo systemInfosMax = new SystemInfo();

            if (Functions.elementExists("game_area_sys_req_leftCol") && driver.FindElements(By.ClassName("game_area_sys_req")).Count >= 6)
            {
                IWebElement webElement = Functions.FindElement("game_area_sys_req_leftCol");
                Console.WriteLine("Minimal System:");
                Console.WriteLine("Betriebssystem: " + webElement.FindElements(By.TagName("li"))[0].Text);
                Console.WriteLine("Prozessor: " + webElement.FindElements(By.TagName("li"))[1].Text);
                Console.WriteLine("Arbeitsspeicher: " + webElement.FindElements(By.TagName("li"))[2].Text);
                Console.WriteLine("Grafik: " + webElement.FindElements(By.TagName("li"))[3].Text);
                Console.WriteLine("DirectX: " + webElement.FindElements(By.TagName("li"))[4].Text);
                Console.WriteLine("Storage: " + webElement.FindElements(By.TagName("li"))[5].Text);

                systemInfosMin.OS = webElement.FindElements(By.TagName("li"))[0].Text;
                systemInfosMin.CPU = webElement.FindElements(By.TagName("li"))[1].Text;
                systemInfosMin.RAM = webElement.FindElements(By.TagName("li"))[2].Text;
                systemInfosMin.GPU = webElement.FindElements(By.TagName("li"))[3].Text;
                systemInfosMin.DirectX = webElement.FindElements(By.TagName("li"))[4].Text;
                systemInfosMin.Storage = webElement.FindElements(By.TagName("li"))[5].Text;

                webElement = Functions.FindElement("game_area_sys_req_rightCol");
                Console.WriteLine("Maximal System:");
                Console.WriteLine("Betriebssystem: " + webElement.FindElements(By.TagName("li"))[0].Text);
                Console.WriteLine("Prozessor: " + webElement.FindElements(By.TagName("li"))[1].Text);
                Console.WriteLine("Arbeitsspeicher: " + webElement.FindElements(By.TagName("li"))[2].Text);
                Console.WriteLine("Grafik: " + webElement.FindElements(By.TagName("li"))[3].Text);
                Console.WriteLine("DirectX: " + webElement.FindElements(By.TagName("li"))[4].Text);
                Console.WriteLine("Storage: " + webElement.FindElements(By.TagName("li"))[5].Text);

                systemInfosMax.OS = webElement.FindElements(By.TagName("li"))[0].Text;
                systemInfosMax.CPU = webElement.FindElements(By.TagName("li"))[1].Text;
                systemInfosMax.RAM = webElement.FindElements(By.TagName("li"))[2].Text;
                systemInfosMax.GPU = webElement.FindElements(By.TagName("li"))[3].Text;
                systemInfosMax.DirectX = webElement.FindElements(By.TagName("li"))[4].Text;
                systemInfosMax.Storage = webElement.FindElements(By.TagName("li"))[5].Text;
            }
            else
            {
                if (driver.FindElements(By.ClassName("game_area_sys_req")).Count >= 6)
                {
                    IWebElement webElement = driver.FindElements(By.ClassName("game_area_sys_req"))[0];
                    Console.WriteLine("Minimal System:");
                    Console.WriteLine("Betriebssystem: " + webElement.FindElements(By.TagName("li"))[0].Text);
                    Console.WriteLine("Prozessor: " + webElement.FindElements(By.TagName("li"))[1].Text);
                    Console.WriteLine("Arbeitsspeicher: " + webElement.FindElements(By.TagName("li"))[2].Text);
                    Console.WriteLine("Grafik: " + webElement.FindElements(By.TagName("li"))[3].Text);
                    Console.WriteLine("DirectX: " + webElement.FindElements(By.TagName("li"))[4].Text);
                    Console.WriteLine("Storage: " + webElement.FindElements(By.TagName("li"))[5].Text);

                    systemInfosMin.OS = webElement.FindElements(By.TagName("li"))[0].Text;
                    systemInfosMin.CPU = webElement.FindElements(By.TagName("li"))[1].Text;
                    systemInfosMin.RAM = webElement.FindElements(By.TagName("li"))[2].Text;
                    systemInfosMin.GPU = webElement.FindElements(By.TagName("li"))[3].Text;
                    systemInfosMin.DirectX = webElement.FindElements(By.TagName("li"))[4].Text;
                    systemInfosMin.Storage = webElement.FindElements(By.TagName("li"))[5].Text;

                    systemInfosMax.OS = webElement.FindElements(By.TagName("li"))[0].Text;
                    systemInfosMax.CPU = webElement.FindElements(By.TagName("li"))[1].Text;
                    systemInfosMax.RAM = webElement.FindElements(By.TagName("li"))[2].Text;
                    systemInfosMax.GPU = webElement.FindElements(By.TagName("li"))[3].Text;
                    systemInfosMax.DirectX = webElement.FindElements(By.TagName("li"))[4].Text;
                    systemInfosMax.Storage = webElement.FindElements(By.TagName("li"))[5].Text;
                }
                else
                {
                    IWebElement webElement = driver.FindElements(By.ClassName("game_area_sys_req"))[0];
                    Console.WriteLine("Minimal System:");
                    Console.WriteLine("Betriebssystem: " + webElement.FindElements(By.TagName("li"))[0].Text);
                    Console.WriteLine("Arbeitsspeicher: " + webElement.FindElements(By.TagName("li"))[1].Text);
                    Console.WriteLine("Grafik: " + webElement.FindElements(By.TagName("li"))[2].Text);
                    Console.WriteLine("Storage: " + webElement.FindElements(By.TagName("li"))[3].Text);

                    systemInfosMin.OS = webElement.FindElements(By.TagName("li"))[0].Text;
                    systemInfosMin.RAM = webElement.FindElements(By.TagName("li"))[1].Text;
                    systemInfosMin.GPU = webElement.FindElements(By.TagName("li"))[2].Text;
                    systemInfosMin.Storage = webElement.FindElements(By.TagName("li"))[3].Text;

                    systemInfosMax.OS = webElement.FindElements(By.TagName("li"))[0].Text;
                    systemInfosMax.RAM = webElement.FindElements(By.TagName("li"))[1].Text;
                    systemInfosMax.GPU = webElement.FindElements(By.TagName("li"))[2].Text;
                    systemInfosMax.Storage = webElement.FindElements(By.TagName("li"))[3].Text;
                }
            }

            storeItem.SystemInfoMin = systemInfosMin;
            storeItem.SystemInfoMax = systemInfosMax;

        }

        private static void GetGameTags()
        {
            IWebElement elements = Functions.FindElement("glance_tags");
            int index = 0;
            string[] gameTag = new string[0];

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
                Console.WriteLine("Price Old: " + Functions.FindText("discount_original_price"));
                Console.WriteLine("Price New: " + Functions.FindText("discount_final_price"));
                Console.WriteLine("Reduce: " + Functions.FindText("discount_pct"));

                price.Free = false;
                price.PriceOld = Functions.FindText("discount_original_price");
                price.PriceNew = Functions.FindText("discount_final_price");
                price.Reduced = Functions.FindText("discount_pct");
            }

            Console.WriteLine("\nGame Image: " + driver.FindElement(By.ClassName("game_header_image_full")).GetAttribute("src"));
            storeItem.GameImage = driver.FindElement(By.ClassName("game_header_image_full")).GetAttribute("src");

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

            Console.WriteLine("Release: " + Functions.FindText("date"));//Release
            storeItem.Release = Functions.FindText("date");

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

        private static void GetDLC()
        {
            if (Functions.elementExists("dlc_footer_element"))
            {
                driver.FindElement(By.Id("dlc_footer")).FindElements(By.ClassName("dlc_footer_element"))[0].Click();
                Thread.Sleep(2000);

                string[] dlcURL = new string[driver.FindElement(By.ClassName("gameDlcBlocks")).FindElements(By.TagName("a")).Count + driver.FindElement(By.Id("game_area_dlc_expanded")).FindElements(By.TagName("a")).Count];
                string[] dlc = new string[driver.FindElement(By.ClassName("gameDlcBlocks")).FindElements(By.TagName("a")).Count + driver.FindElement(By.Id("game_area_dlc_expanded")).FindElements(By.TagName("a")).Count];

                int counter1 = 0;
                foreach (IWebElement element in driver.FindElement(By.ClassName("gameDlcBlocks")).FindElements(By.TagName("a")))
                {
                    String dlcID = element.GetAttribute("href");
                    dlcURL[counter1] = dlcID;
                    counter1++;
                }

                foreach (IWebElement element in driver.FindElement(By.Id("game_area_dlc_expanded")).FindElements(By.TagName("a")))
                {
                    String dlcID = element.GetAttribute("href");
                    dlcURL[counter1] = dlcID;
                    counter1++;
                }

                counter1 = 0;
                foreach (string url in dlcURL)
                {
                    if (!url.Contains("Soundtrack") && url.StartsWith("http"))
                    {
                        String dlcID = SteamDLC.GetSteamDLC(url, storeItem);
                        dlc[counter1] = dlcID;
                        counter1++;
                    }
                }

                storeItemDLC.DLCID = dlc;

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
