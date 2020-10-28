using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
using System.Threading;
using Tests;

namespace Zero_Web_GetGameContent.AutoGrab
{
    public class SteamGames
    {

        private static IWebDriver driver = Program.driver;
        private static string path = Environment.CurrentDirectory + "NewGamesToLoadSteam.txt";

        //https://store.steampowered.com/search/?filter=topsellers
        public static void StartSteamAutoGrab(string searchURL)
        {

            //Search Link
            SearchGames(searchURL);

            //Get Games
            GetGames(searchURL);

        }

        private static void SearchGames(string searchURL)
        {
            driver.Navigate().GoToUrl(searchURL);
            Thread.Sleep(3000);
        }

        private static void GetGames(string searchURL)
        {

            if (!File.Exists(path))
            {
                File.Create(path);
            }

            Thread.Sleep(5000);

            if (driver.FindElements(By.ClassName("paged_items_paging_pagelink")).Count <= 0)
            {
                int maxPageSize = 150;
                int currentGame = 0;

                //Foreach max Page
                for (int i = 0; i < maxPageSize - 1; i++)
                {
                    Thread.Sleep(2000);
                    IWebElement scroll = driver.FindElement(By.Id("search_resultsRows")).FindElements(By.TagName("a"))[driver.FindElement(By.Id("search_resultsRows")).FindElements(By.TagName("a")).Count - 1];
                    int loop = 0;

                    foreach (IWebElement element in driver.FindElement(By.Id("search_resultsRows")).FindElements(By.TagName("a")))
                    {
                        if (loop >= currentGame)
                        {
                            File.AppendAllText(path, element.GetAttribute("href") + "\n");
                            currentGame++;
                        }
                        loop++;
                    }

                    Actions actions = new Actions(driver);
                    actions.MoveToElement(scroll);
                    actions.Perform();
                    Thread.Sleep(2000);

                }
            }
            else
            {

                //Current User
                if (searchURL.Contains("ConcurrentUsers"))
                {
                    string stringMaxPageSize = driver.FindElement(By.Id("ConcurrentUsers_ctn")).FindElement(By.Id("ConcurrentUsers_links")).FindElements(By.ClassName("paged_items_paging_pagelink"))[driver.FindElement(By.Id("ConcurrentUsers_ctn")).FindElement(By.Id("ConcurrentUsers_links")).FindElements(By.ClassName("paged_items_paging_pagelink")).Count - 1].Text;
                    int maxPageSize = Convert.ToInt32(stringMaxPageSize);

                    //Foreach max Page
                    for (int i = 0; i < maxPageSize - 1; i++)
                    {
                        //Foreach Items
                        foreach (IWebElement element in driver.FindElement(By.ClassName("tab_content_ctn")).FindElements(By.TagName("div"))[3].FindElements(By.TagName("a")))
                        {
                            if (!element.FindElement(By.ClassName("tab_item_name")).Text.Contains("Demo") && !element.FindElement(By.ClassName("tab_item_name")).Text.Contains("Addon") && !element.FindElement(By.ClassName("tab_item_name")).Text.Contains("DLC") && !element.FindElement(By.ClassName("tab_item_name")).Text.Contains("Pack") && element.FindElements(By.ClassName("discount_final_price")).Count > 0)
                                File.AppendAllText(path, element.GetAttribute("href") + "\n");
                        }

                        Thread.Sleep(2000);
                        driver.FindElement(By.Id("ConcurrentUsers_btn_next")).Click();

                    }
                }
                else if (searchURL.Contains("TopSellers"))//TopSeller
                {
                    string stringMaxPageSize = driver.FindElement(By.Id("TopSellers_ctn")).FindElement(By.Id("TopSellers_links")).FindElements(By.ClassName("paged_items_paging_pagelink"))[driver.FindElement(By.Id("TopSellers_ctn")).FindElement(By.Id("TopSellers_links")).FindElements(By.ClassName("paged_items_paging_pagelink")).Count - 1].Text;
                    int maxPageSize = Convert.ToInt32(stringMaxPageSize);

                    //Foreach max Page
                    for (int i = 0; i < maxPageSize - 1; i++)
                    {
                        //Foreach Items
                        foreach (IWebElement element in driver.FindElement(By.ClassName("tab_content_ctn")).FindElements(By.TagName("div"))[3].FindElements(By.TagName("a")))
                        {
                            if (!element.FindElement(By.ClassName("tab_item_name")).Text.Contains("Demo") && !element.FindElement(By.ClassName("tab_item_name")).Text.Contains("Addon") && !element.FindElement(By.ClassName("tab_item_name")).Text.Contains("DLC") && !element.FindElement(By.ClassName("tab_item_name")).Text.Contains("Pack") && element.FindElements(By.ClassName("discount_final_price")).Count > 0)
                                File.AppendAllText(path, element.GetAttribute("href") + "\n");
                        }

                        Thread.Sleep(2000);
                        driver.FindElement(By.Id("TopSellers_btn_next")).Click();
                    }

                }

            }

        }

    }
}
