using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable InconsistentNaming - using some C++ constants in uppder-case and don't want R# to complain about them.

namespace MyDesktop
{
    internal class User32
    {
        public const int SPI_SETDESKWALLPAPER = 0x0014;

        public const int COLOR_DESKTOP = 1;

        // SystemParametersInfoA function
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms724947(v=vs.85).aspx

        // For setting a string parameter
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, SPIF fWinIni);

        // For reading a string parameter
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, SPIF fWinIni);

        [DllImport("user32.dll")]
        public static extern bool SetSysColors(int cElements, int[] lpaElements, int[] lpaRgbValues);

        public static int ParseWin32color(string rgb)
        {
            // RGB macro is defined in WinGDI.h header file as follows:
            // #define RGB(r,g,b)      ((COLORREF)(((BYTE)(r)|((WORD)((BYTE)(g))<<8))|(((DWORD)(BYTE)(b))<<16)))
            return
                rgb
                    .Split(',')
                    .Select(int.Parse)
                    .Aggregate(
                        (Color: 0, Offset: 0),
                        (current, next) =>
                        (
                            Color: current.Color | next << current.Offset,
                            Offset: current.Offset + 8
                        )
                    )
                    .Color;
        }
    }

    [Flags]
    public enum SPIF
    {
        None = 0x00,

        /// <summary>Writes the new system-wide parameter setting to the user profile.</summary>
        SPIF_UPDATEINIFILE = 0x01,

        /// <summary>Broadcasts the WM_SETTINGCHANGE message after updating the user profile.</summary>
        SPIF_SENDCHANGE = 0x02,

        /// <summary>Same as SPIF_SENDCHANGE.</summary>
        SPIF_SENDWININICHANGE = 0x02
    }
}