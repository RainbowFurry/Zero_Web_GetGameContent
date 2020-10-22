using MongoDB.Bson;
using OpenQA.Selenium;
using System;
using System.Threading;
using Tests;
using Zero_Web_GetGameContent.Manager;
using Zero_Web_GetGameContent.Model;

namespace Zero_Web_GetGameContent.GeneralInfo
{
    internal class MicrosoftDLC
    {

        public static string GetMicroSoftDLC(string dlcURL, StoreItem mainGame)
        {
            return StartMicrosoftTest(dlcURL, mainGame);
        }

        private static IWebDriver driver = Program.driver;
        private static StoreItem storeItem;
        private static ItemPrice price;
        private static StoreItemPrice storeItemPrice;
        private static StoreItemLanguage storeItemLanguage;
        private static Language language;

        private static string StartMicrosoftTest(string searchURL, StoreItem mainGame)
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

            //Get Images
            GetDisplayImages();

            //Get FSK Info
            GetFSKInfo();

            //Game System Requirement
            GetSystemInfo(mainGame);

            //Page Info From
            Console.WriteLine("Microsoft");
            storeItem.InfoFrom = "Microsoft";

            //Category
            storeItem.Category = "Games";

            Console.WriteLine("\n\n");

            //Create DB Entry
            if (price.GamePrice != null || price.PriceNew != null)
            {
                MongoDBManager.CreateEntry("StoreItemDLCTEMP", storeItem.ToBsonDocument());
                storeItemLanguage.language = new Language[] { language };
                storeItemLanguage.PageURL = searchURL;
                storeItemLanguage.PageName = "Microsoft";
                MongoDBManager.CreateEntry("StoreItemDLCLanguageTEMP", storeItemLanguage.ToBsonDocument());
                price.PageURL = searchURL;
                price.PageName = "Microsoft";
                storeItemPrice.price = new ItemPrice[] { price };
                MongoDBManager.CreateEntry("StoreItemDLCPriceTEMP", storeItemPrice.ToBsonDocument());

                Thread.Sleep(2000);
                return storeItem.ID;
            }
            Thread.Sleep(2000);
            return null;

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

        private static void GetGameInfo()
        {
            Console.WriteLine("Name: " + driver.FindElement(By.Id("DynamicHeading_productTitle")).Text);//Name
            storeItem.GameName = driver.FindElement(By.Id("DynamicHeading_productTitle")).Text;

            if (driver.FindElements(By.ClassName("m-product-detail-description")).Count > 0)
            {
                Console.WriteLine(driver.FindElement(By.ClassName("m-product-detail-description")).Text);
                language.LongDescription = driver.FindElement(By.ClassName("m-product-detail-description")).Text;
            }

            //Price
            if (!Functions.elementExists("ProductPrice_productPrice_PriceContainer"))
            {
                if (driver.FindElement(By.Id("ProductPrice_productPrice_PriceContainer")).FindElement(By.TagName("span")).Text == "Free")
                {
                    Console.WriteLine("Preis: " + "Free");

                    price.Free = true;
                }
                else
                {
                    if (driver.FindElements(By.Id("ProductPrice_productPrice_PriceContainer")).Count > 0)
                    {
                        Console.WriteLine("Preis: " + driver.FindElement(By.Id("ProductPrice_productPrice_PriceContainer")).FindElements(By.TagName("span"))[0].Text);

                        price.Free = false;
                        price.GamePrice = driver.FindElement(By.Id("ProductPrice_productPrice_PriceContainer")).FindElements(By.TagName("span"))[0].Text;
                    }
                }
            }
            else
            {
                Console.WriteLine("Price Old: " + driver.FindElement(By.ClassName("ProductPrice_productPrice_PriceContainer")).FindElement(By.TagName("s")).Text);
                Console.WriteLine("Price New: " + driver.FindElement(By.ClassName("price-disclaimer")).FindElement(By.TagName("span")).Text);
                Console.WriteLine("Reduce: " + driver.FindElement(By.ClassName("caption")).FindElements(By.TagName("span"))[0].Text.Replace("Rabatt", ""));

                price.Free = false;
                price.PriceOld = driver.FindElement(By.ClassName("ProductPrice_productPrice_PriceContainer")).FindElement(By.TagName("s")).Text;
                price.PriceNew = driver.FindElement(By.ClassName("price-disclaimer")).FindElement(By.TagName("span")).Text;
                price.Reduced = driver.FindElement(By.ClassName("caption")).FindElements(By.TagName("span"))[0].Text.Replace("Rabatt", "");
            }

            //Image
            if (Functions.elementExistsID("dynamicImage_image_picture"))
            {
                Console.WriteLine("\nGame Image: " + driver.FindElement(By.Id("dynamicImage_image_picture")).FindElement(By.TagName("img")).GetAttribute("src"));
                storeItem.GameImage = driver.FindElement(By.Id("dynamicImage_image_picture")).FindElement(By.TagName("img")).GetAttribute("src");
            }
            else
            {
                Console.WriteLine("\nGame Image: " + driver.FindElement(By.Id("dynamicImage_productImage_picture")).FindElement(By.TagName("img")).GetAttribute("src"));
                storeItem.GameImage = driver.FindElement(By.Id("dynamicImage_productImage_picture")).FindElement(By.TagName("img")).GetAttribute("src");
            }

            Console.WriteLine("Description: " + Functions.FindText("pi-product-description-text"));//Short Description
            language.ShortDescription = Functions.FindText("pi-product-description-text");

            Console.WriteLine("Release: " + driver.FindElement(By.Id("releaseDate-toggle-target")).FindElement(By.TagName("span")).Text);//Release
            storeItem.Release = driver.FindElement(By.Id("releaseDate-toggle-target")).FindElement(By.TagName("span")).Text;

            //Genre
            string[] genre;
            Console.WriteLine("Genre: " + driver.FindElement(By.Id("kategorie-toggle-target")).FindElement(By.TagName("a")).Text);
            genre = driver.FindElement(By.Id("kategorie-toggle-target")).FindElement(By.TagName("a")).Text.Split('&');
            language.Genre = genre;

        }

        private static void GetFSKInfo()
        {
            if (Functions.elementExists("c-age-rating"))
            {
                IWebElement element = Functions.FindElement("c-age-rating").FindElements(By.TagName("a"))[0];

                if (element.Text.Contains("16"))
                {
                    Console.WriteLine("FSK: 16");
                    storeItem.FSK = "16";
                }
                else if (element.Text.Contains("18"))
                {
                    Console.WriteLine("FSK: 18");
                    storeItem.FSK = "18";
                }
                else if (element.Text.Contains("12"))
                {
                    Console.WriteLine("FSK: 12");
                    storeItem.FSK = "12";
                }
                else if (element.Text.Contains("6"))
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

            DisplayImages[] displayImagesList;
            DisplayImages displayImage;
            int counter = 0;

            //Images
            DisplayImages[] displayImages = new DisplayImages[0];
            if (Functions.elementExists("module-responsive-screenshots") && Functions.elementExists("cli_screenshot_gallery"))
            {
                displayImages = new DisplayImages[driver.FindElement(By.ClassName("module-responsive-screenshots")).FindElement(By.ClassName("m-product-placement")).FindElement(By.ClassName("cli_screenshot_gallery")).FindElements(By.TagName("li")).Count];

                foreach (IWebElement element in driver.FindElement(By.ClassName("module-responsive-screenshots")).FindElement(By.ClassName("m-product-placement")).FindElement(By.ClassName("cli_screenshot_gallery")).FindElements(By.TagName("li")))
                {
                    displayImage = new DisplayImages();
                    displayImage.Video = false;
                    string url = element.FindElement(By.TagName("a")).GetAttribute("href");
                    Console.WriteLine(url);
                    displayImage.URL = url;
                    displayImages[counter] = displayImage;
                    counter++;
                }
            }

            //Videos
            DisplayImages[] displayVideos = new DisplayImages[0];
            counter = 0;
            if (Functions.elementExists("m-media-gallery"))
            {
                displayVideos = new DisplayImages[driver.FindElement(By.ClassName("m-media-gallery")).FindElement(By.ClassName("c-carousel")).FindElements(By.TagName("li")).Count];

                foreach (IWebElement element in driver.FindElement(By.ClassName("m-media-gallery")).FindElement(By.ClassName("c-carousel")).FindElements(By.TagName("li")))
                {
                    displayImage = new DisplayImages();
                    displayImage.Video = true;
                    string url = element.FindElement(By.TagName("a")).GetAttribute("href");
                    Console.WriteLine(url);
                    displayImage.URL = url;
                    displayVideos[counter] = displayImage;
                    counter++;
                }
            }

            if (displayVideos.Length > 0)
            {
                counter = 0;
                displayImagesList = new DisplayImages[displayImages.Length + displayVideos.Length];
                foreach (DisplayImages img in displayImages)
                {
                    displayImagesList[counter] = img;
                    counter++;
                }
                foreach (DisplayImages video in displayVideos)
                {
                    displayImagesList[counter] = video;
                    counter++;
                }
                storeItem.GameImages = displayImagesList;
            }
            else
            {
                counter = 0;
                displayImagesList = new DisplayImages[displayImages.Length];
                foreach (DisplayImages img in displayImages)
                {
                    displayImagesList[counter] = img;
                    counter++;
                }
                storeItem.GameImages = displayImagesList;
            }

        }

    }
}
