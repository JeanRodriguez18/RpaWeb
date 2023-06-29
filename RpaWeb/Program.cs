using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using RpaWeb;


// Ingrese un tema a Investigar
Console.WriteLine("Ingrese un tema a investigar:");
string TemaInvestigar = Console.ReadLine();

if (string.IsNullOrWhiteSpace(TemaInvestigar))
{
    Console.WriteLine("No ha Ingresado un tema valido. Presione cualquier tecla para salir de la aplicacion.");
    Console.ReadKey();
    Environment.Exit(0);
}

BuscarTemaAinvestigar(Browser.Chrome, TemaInvestigar);
BuscarTemaAinvestigar(Browser.Edge, TemaInvestigar);
BuscarTemaAinvestigar(Browser.Safari, TemaInvestigar);
BuscarTemaAinvestigar(Browser.Firefox, TemaInvestigar);

Console.WriteLine("Presione cualquier tecla para salir de la aplicacion.");
Console.ReadKey();



static void BuscarTemaAinvestigar(Browser browser, string TemaAInvestigar)
{
    IWebDriver driver;

    try
    {
        driver = GetDriver(browser);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"No se pudo abrir {browser} porque no esta instalado. {ex.Message}");
        return;
    }

    try
    {
        driver.Navigate().GoToUrl("https://www.google.com");

        IWebElement searchInput = driver.FindElement(By.Name("q"));
        searchInput.SendKeys($"{TemaAInvestigar} filetype:pdf");
        searchInput.Submit();

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        IWebElement searchResults = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("search")));

    }
    finally
    {
        driver.Manage().Window.Minimize();
    }
}

static IWebDriver GetDriver(Browser browser)
{
    switch (browser)
    {
        case Browser.Chrome:
            try
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\Rpa\Resultados");
                chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                return new ChromeDriver(chromeOptions);
            }
            catch (Exception ex)
            {
                throw new DriverNotFoundException("Chrome driver no se ha encontrado. Asegurate de que este instalado.", ex);
            }

        case Browser.Edge:
            try
            {
                EdgeOptions edgeOptions = new EdgeOptions();
                return new EdgeDriver(edgeOptions);
            }
            catch (Exception ex)
            {
                throw new DriverNotFoundException("Microsoft Edge no se ha encontrado. Asegurate de que este instalado.", ex);
            }


        case Browser.Safari:
            try
            {
                return new SafariDriver();
            }
            catch (Exception ex)
            {
                throw new DriverNotFoundException("Safari browser not found. Please make sure it is installed.", ex);
            }


        case Browser.Firefox:
            try
            {
                FirefoxOptions firefoxOptions = new FirefoxOptions();
                return new FirefoxDriver(firefoxOptions);
            }
            catch (Exception ex)
            {
                throw new DriverNotFoundException("Firefox no se ha encontrado. Asegurese de que este instalado.", ex);
            }
        default:
            throw new ArgumentException("Navegador Invalido.");
    }
}

enum Browser
{
    Chrome,
    Edge,
    Safari,
    Firefox
}