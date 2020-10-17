using OpenQA.Selenium;
using System;
using System.Threading;
using Tests;
using Zero_Web_GetGameContent.Model;

namespace Zero_Web_GetGameContent.GeneralInfo
{
    internal class MicrosoftStore
    {

        private static IWebDriver driver = Program.driver;
        private static StoreItem storeItem;
        private static LanguageContent languageContent;

        public static void StartMicrosoftStoreTest(string searchURL)
        {

            storeItem = new StoreItem();
            languageContent = new LanguageContent();
            storeItem.LanguageContents = new LanguageContent[10];

            //ID
            storeItem.ID = Guid.NewGuid().ToString();

            Console.WriteLine("\n\n");

            //Search Game
            SearchGame(searchURL);

            //Game Info
            GetGameInfo();

            //Get FSK Info
            GetFSKInfo();

            //Game System Requirement
            GetSystemInfo();

            //Page Info From
            Console.WriteLine("MicrosoftStore");
            storeItem.InfoFrom = "MicrosoftStore";

            //Category REFERENCE!!!!!
            storeItem.Category = "Games";

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
        }


        private static void GetSystemInfo()
        {

            if (driver.FindElement(By.Id("ProductPrice_productPrice_PriceContainer")).FindElement(By.TagName("span")).Text != "Kostenlos")
            {
                driver.FindElement(By.Id("BuyBoxMessages_systemMessages_SystemRequirementsMessage_Link")).Click();
                Thread.Sleep(2000);

                Console.WriteLine("Minimal System:");
                Console.WriteLine("Betriebssystem: " + driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).Text);
                Console.WriteLine("Prozessor: " + driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[7].FindElement(By.TagName("td")).Text);
                Console.WriteLine("Arbeitsspeicher: " + driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[5].FindElement(By.TagName("td")).Text);
                Console.WriteLine("Grafik: " + driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[8].FindElement(By.TagName("td")).Text);
                Console.WriteLine("DirecttX: " + driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[4].FindElement(By.TagName("td")).Text);

                SystemInfo systemInfos = new SystemInfo();

                systemInfos.OS = driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).Text;
                systemInfos.CPU = driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[7].FindElement(By.TagName("td")).Text;
                systemInfos.RAM = driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[5].FindElement(By.TagName("td")).Text;
                systemInfos.GPU = driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[8].FindElement(By.TagName("td")).Text;
                systemInfos.DirectX = driver.FindElement(By.ClassName("m-system-requirements")).FindElement(By.ClassName("c-table")).FindElements(By.TagName("tr"))[4].FindElement(By.TagName("td")).Text;

                storeItem.SystemInfoMin = systemInfos;
            }
        }

        private static void GetGameInfo()
        {
            Console.WriteLine("Name: " + driver.FindElement(By.Id("DynamicHeading_productTitle")).Text);//Name
            storeItem.GameName = driver.FindElement(By.Id("DynamicHeading_productTitle")).Text;

            //Price
            Price price = new Price();
            if (!Functions.elementExists("ProductPrice_productPrice_PriceContainer"))
            {
                if (driver.FindElement(By.Id("ProductPrice_productPrice_PriceContainer")).FindElement(By.TagName("span")).Text == "Free")
                {
                    Console.WriteLine("Preis: " + "Free");

                    price.Free = true;
                }
                else
                {
                    Console.WriteLine("Preis: " + driver.FindElement(By.Id("ProductPrice_productPrice_PriceContainer")).FindElements(By.TagName("span"))[0].Text);

                    price.Free = false;
                    price.GamePrice = driver.FindElement(By.Id("ProductPrice_productPrice_PriceContainer")).FindElements(By.TagName("span"))[0].Text;
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
            storeItem.Price = price;

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
            languageContent.ShortDescription = Functions.FindText("pi-product-description-text");
            Console.WriteLine("Release: " + driver.FindElement(By.Id("releaseDate-toggle-target")).FindElement(By.TagName("span")).Text);//Release
            storeItem.Release = driver.FindElement(By.Id("releaseDate-toggle-target")).FindElement(By.TagName("span")).Text;

            //Genre
            string[] genre;
            Console.WriteLine("Genre: " + driver.FindElement(By.Id("kategorie-toggle-target")).FindElement(By.TagName("a")).Text);
            genre = driver.FindElement(By.Id("kategorie-toggle-target")).FindElement(By.TagName("a")).Text.Split('&');
            languageContent.Genre = genre;

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

    }
}
