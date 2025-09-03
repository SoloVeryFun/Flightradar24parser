using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        var options = new ChromeOptions();
        
        //Headless mode:
        //options.AddArgument("--window-size=1200,800");
        options.AddArgument("--headless");

        //Disable cmd logs
        options.AddArgument("--log-level=3"); // Только ошибки
        options.AddArgument("--disable-logging");
        options.AddArgument("--silent");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-speech-api");
        options.AddArgument("--disable-notifications");
        options.AddArgument("--disable-gcm-registration");

        IWebDriver driver = new ChromeDriver(options);

        try
        {
            driver.Navigate().GoToUrl("https://www.flightradar24.com/airport/evn/arrivals");
            //driver.Navigate().GoToUrl("https://www.example.com/");

        }
        catch (WebDriverException)
        {
            Console.WriteLine("Error with response from site, try to run the application later");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            driver.Quit();
        }


        string title = driver.Title;
        Console.WriteLine($"Page Title: {title}");


        var allflights = GetAllFlights(driver);
        var requiredflights = requiredFlights(allflights);
        var flightinformation = flightInformation(requiredflights);

        print(flightinformation);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        driver.Quit();
        }

    //Getting flight information
    static List<string> flightInformation(List<IWebElement> requiredflights)
    {
        List<string> flightinfo = new List<string>();

        foreach (var flight in requiredflights)
        {
            IWebElement today = flight.FindElement(By.XPath("ancestor::div[9]"));
            IWebElement parentSection = flight.FindElement(By.XPath("ancestor::div[5]"));

            string date = today.FindElement(By.CssSelector("h3.inline-flex")).Text;
            string cityName = parentSection.FindElement(By.CssSelector("span.mr-px")).Text;

            flightinfo.Add($"{date} : {cityName}");
        }

        return flightinfo;
    }

    //Filtering flights by time, NOT FINISHED
    static List<IWebElement> requiredFlights(List<IWebElement> allflights)
    {
        int hour;
        //int minute;
        string ampm;
        List<IWebElement> requiredflights = new List<IWebElement>();

        foreach (IWebElement flight in allflights)
        {
            var spans = flight.FindElements(By.TagName("span"));

            hour = Convert.ToInt32(spans[0].Text);
            ampm = spans[4].Text;

            if ((hour >= 10 && hour < 11) && ampm == "PM") requiredflights.Add(flight);
            
        }

        return requiredflights;
    }

    //get all scheduled flights for 2 days
    static List<IWebElement> GetAllFlights(IWebDriver driver)
    {
        WebDriverWait waitMainDiv = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

        List<IWebElement> allflights = new List<IWebElement>();

        try
        {
            var mainDiv = waitMainDiv.Until(driver => driver.FindElements(By.CssSelector("div.font-semibold.text-gray-1300")).Count > 0);

            if (mainDiv)
            {
                var elements = driver.FindElements(By.CssSelector("div.font-semibold.text-gray-1300"));
                allflights = elements.ToList();
            }
            else
            {
                Console.WriteLine("Error with response from site, try to run the application later");
                
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                driver.Quit();
            }
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine("Error with response from site, try to run the application later");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            
            driver.Quit();
        }
        return allflights;
    }

    static void print(List<IWebElement> allflights)
    {
        if (allflights.Count > 0)
        {
            foreach (var flight in allflights)
            {
                Console.WriteLine(flight.Text);
            }
        }
        else Console.WriteLine("No flights found.");
    }

    static void print(List<string> texts)
    {
        if (texts.Count > 0)
        {
            foreach (var text in texts)
            {
                Console.WriteLine(text);
            }
        }
        else Console.WriteLine("Information not founded");
    }
}