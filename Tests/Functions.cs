using OpenQA.Selenium;
using System;

namespace Tests
{
    internal class Functions
    {

        private static IWebDriver driver = Program.driver;

        public static string FindText(string className)
        {
            return driver.FindElement(By.ClassName(className)).Text;
        }

        public static IWebElement FindElement(string className)
        {
            return driver.FindElement(By.ClassName(className));
        }

        public static IWebElement FindElementID(string className)
        {
            return driver.FindElement(By.Id(className));
        }

        public static IWebElement FindElementTag(string className)
        {
            return driver.FindElement(By.TagName(className));
        }

        public static bool elementExists(String className)
        {
            return driver.FindElements(By.ClassName(className)).Count > 0;
        }

        public static bool elementExistsID(String className)
        {
            return driver.FindElements(By.Id(className)).Count > 0;
        }

    }
}
