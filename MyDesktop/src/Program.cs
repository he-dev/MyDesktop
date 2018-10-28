using System;
using System.Drawing;
using System.IO;
using Microsoft.Extensions.Configuration;

// Configuration in ASP.NET Core
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/index?view=aspnetcore-2.1&tabs=basicconfiguration

namespace MyDesktop
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("MyDesktop-v1.1");

            // Make sure it's really the assembly's location. Otherwise it'll break when run as a service.
            var location = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            Directory.SetCurrentDirectory(location);

            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            SetBackgroundColor(configuration["BackgroundColor"]);
            SetWallpaper(configuration["WallpaperPath"]);


#if !DEBUG
            Console.WriteLine("Enjoy ;-)");
            Console.WriteLine("[Press any key to exit]");
            Console.ReadKey();
#endif
        }

        private static void SetBackgroundColor(string color)
        {
            Console.WriteLine($"Background color: ({color})");
            var elements = new[] { User32.COLOR_DESKTOP };
            var colors = new[] { User32.ParseWin32color(color) };
            User32.SetSysColors(elements.Length, elements, colors);
        }

        private static void SetWallpaper(string path)
        {
            Console.WriteLine($"Wallpaper: '{path}'");
            User32.SystemParametersInfo(User32.SPI_SETDESKWALLPAPER, 0, path, SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
        }
    }    
}
