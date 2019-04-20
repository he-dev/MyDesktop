using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

// Configuration in ASP.NET Core
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/index?view=aspnetcore-2.1&tabs=basicconfiguration

namespace MyDesktop
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"{ProgramInfo.Name}-v{ProgramInfo.Version}");
            
            SetLockScreenWallpaper("");

            // Make sure it's really the assembly's location. Otherwise it'll break when run as a service.
            var location = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            Directory.SetCurrentDirectory(location);

            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            SetDesktopBackgroundColor(configuration.DesktopBackgroundColor());
            SetDesktopWallpaper(configuration.DesktopWallpaperPath());
            SetLockScreenWallpaper(configuration.LockScreenWallpaperPath());

#if !DEBUG
            Console.WriteLine("Enjoy ;-)");
            Console.WriteLine("[Press any key to exit]");
            Console.ReadKey();
#endif
        }

        private static void SetDesktopBackgroundColor(string color)
        {
            Console.WriteLine($"Background color: ({color})");
            var elements = new[] { User32.COLOR_DESKTOP };
            var colors = new[] { User32.ParseWin32color(color) };
            User32.SetSysColors(elements.Length, elements, colors);
        }

        private static void SetDesktopWallpaper(string path)
        {
            Console.WriteLine($"Wallpaper: '{path}'");
            User32.SystemParametersInfo(User32.SPI_SETDESKWALLPAPER, 0, path, SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
        }

        private static void SetLockScreenWallpaper(string path)
        {
            // HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Personalization

            var personalization = @"SOFTWARE\Policies\Microsoft\Windows\Personalization";

            using (var personalizationSubKey = Registry.LocalMachine.OpenSubKey(personalization, RegistryRights.WriteKey))
            {
                var lockScreenImage = personalizationSubKey.GetValue("LockScreenImage");
                personalizationSubKey.SetValue("LockScreenImage", lockScreenImage);
            }
        }
    }

    public static class ProgramInfo
    {
        public const string Name = "MyDesktop";

        public const string Version = "1.2";
    }

    public static class ConfigurationRootExtensions
    {
        public static string DesktopWallpaperPath(this IConfigurationRoot configurationRoot)
        {
            return configurationRoot[nameof(DesktopWallpaperPath)];
        }

        public static string DesktopBackgroundColor(this IConfigurationRoot configurationRoot)
        {
            return configurationRoot[nameof(DesktopBackgroundColor)];
        }

        public static string LockScreenWallpaperPath(this IConfigurationRoot configurationRoot)
        {
            return configurationRoot[nameof(LockScreenWallpaperPath)];
        }
    }
}