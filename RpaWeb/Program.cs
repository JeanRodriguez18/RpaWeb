using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V112.Console;
using OpenQA.Selenium.DevTools.V112.Debugger;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;

// Configuración de Selenium
ChromeOptions options = new ChromeOptions();
options.AddUserProfilePreference("download.default_directory", @"C:\Rpa\Resultados");
options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);

IWebDriver driver = new ChromeDriver(options);
WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

try
{
    // Abrir el buscador de Google
    driver.Navigate().GoToUrl("https://www.google.com");

    // Buscar el tema de investigación en el buscador
    Console.WriteLine("Ingrese el tema a investigar:");
    string temaDeInvestigacion = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(temaDeInvestigacion))
    {
        Console.WriteLine("No ha ingresado un tema válido. Presione cualquier tecla para cerrar la aplicación.");
        Console.ReadKey();
        return;
    }

    IWebElement searchInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Name("q")));
    searchInput.SendKeys(temaDeInvestigacion);
    searchInput.Submit();

    // Esperar a que aparezcan los resultados de búsqueda
    IWebElement searchResults = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("search")));

    // Obtener los enlaces principales de los resultados de búsqueda
    IReadOnlyCollection<IWebElement> mainLinks = searchResults.FindElements(By.CssSelector("div.g:not(.g-blk) a:not(.fl)"));

    // Crear el directorio para almacenar los resultados
    string resultadosFolderPath = @"C:\Rpa\Resultados\Google Chrome";
    Directory.CreateDirectory(resultadosFolderPath);

    // Tomar una captura de pantalla de la primera página de cada enlace principal
    var iterador = 1;
    foreach (IWebElement link in mainLinks)
    {
        string url = link.GetAttribute("href");

        if (!url.Contains("google") && !url.Contains("youtube") && !url.Contains("googleusercontent"))
        {
            // Abrir el enlace en una nueva pestaña
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);

            try
            {
                driver.Navigate().GoToUrl(url);
                // Esperar un máximo de 60 segundos para que la página se cargue completamente
                WebDriverWait pageLoadWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                pageLoadWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (WebDriverException)
            {
                Console.WriteLine("La página ha tardado demasiado en cargar. Regresando a los resultados de búsqueda.");
                driver.Close();
                driver.SwitchTo().Window(driver.WindowHandles[0]);
                continue;
            }

            // Tomar la captura de pantalla de la página actual
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            // Obtener el título de la página para utilizarlo como nombre del archivo
            string pageTitle = driver.Title;
            string fileName = Path.Combine(resultadosFolderPath, $"resultado{iterador}.png");

            // Guardar la captura de pantalla
            screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);

            Console.WriteLine($"Captura de pantalla guardada: {pageTitle}");
            iterador++;
            // Cerrar la pestaña actual y volver a la página de resultados de búsqueda
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);
        }
        
    }
}
finally
{
    // Cerrar el navegador
    driver.Quit();
}