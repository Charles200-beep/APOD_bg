using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace APOD_bg
{
    public sealed class Wallpaper
    { 
        public static string URL { get; set; }
        public static int style { get; set; }
        public static void downloadSetWallpaper()
        {   
            string json;
            Uri bgUri;
            using (WebClient wc = new WebClient())
            {
                try
                {
                    json = wc.DownloadString(URL);
                }
                catch (WebException)
                {
                    MessageBox.Show("Could not download NASA data! Is the date that was selected incorrect?");
                    throw;
                }
            }
            Root deserializedJSON = JsonConvert.DeserializeObject<Root>(json);
            try
            {
                bgUri = new Uri(deserializedJSON.hdurl);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Could not download background! There could have been a video instead of a picture on this day!");
                throw;
            }
            Debug.WriteLine(bgUri.ToString());
            Set(bgUri, (Style)style);
        }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled = 0,
            Centered = 1,
            Stretched = 2
        }
        public static void Set(Uri uri, Style style)
        {
            System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

            System.Drawing.Image img = System.Drawing.Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
