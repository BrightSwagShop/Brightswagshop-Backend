using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace BrightestSwagShop.Tests_Selenium.Helpers;

// Statische klasse zodat ze overal in de tests gebruikt kan worden
public static class ReportManager
{
    // Bevat het volledige testrapport
    public static ExtentReports Extent;

    // Bevat de huidige test die uitgevoerd wordt
    public static ExtentTest Test;

    // Initialiseert het rapport voordat de tests starten
    public static void InitReport()
    {
          Console.WriteLine(
        Environment.CurrentDirectory
    );
        // Maakt een HTML-rapport aan met de naam TestReport.html
        var spark =
            new ExtentSparkReporter(
                "TestReport.html"
            );

        // Maakt een nieuw ExtentReports-object aan
        Extent = new ExtentReports();

        // Koppelt de HTML-reporter aan ExtentReports
        // Hierdoor weet ExtentReports waar het rapport moet worden opgeslagen
        Extent.AttachReporter(
            spark
        );
    }

    // Schrijft alle verzamelde testresultaten weg naar het HTML-bestand
    // Zonder Flush() wordt het rapport niet opgeslagen of bijgewerkt
    public static void Flush()
    {
        Extent.Flush();
    }
}