using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Diagnostics;

using System.Media;
using System.Windows.Interop;

using System.Windows.Threading;

using System.Threading;

namespace VKHotkeys
{
    /// <summary>
    /// Interaction logic for Splasher.xaml
    /// </summary>
    public partial class Splasher : Window
    {

        //public static int[] allCards { get { return logics.allCards; } }

        public static Dictionary<string, string> dic_params { get { return Form1.settings; } }
        public static Dictionary<string, string> dic_params_splash_test { get { return Settings.dic_params_splash_test; } }

        public Splasher()
        {
            InitializeComponent();
        }

        public bool checkReason(string reason)
        {
            if (reason == "1" && dic_params["splash_display_for_songs"] == "True") { return true; }
            //if (reason == "2" && dic_params["splash_for_profiles"] == "yes") { return true; }
            if (reason == "3" && dic_params["splash_display_for_lock"] == "True") { return true; }
            if (reason == "ex" ) { return true; }
            if (reason == "test") { return true; }

            return false;
        }

        public void LoadSplasherParams(Splasher splash, string songname, string reason)
        {
            if (checkReason(reason))
            {
                Dictionary<string, string> usethis;
                if (reason == "test") { usethis = new Dictionary<string, string>(dic_params_splash_test); }
                else { usethis = new Dictionary<string, string>(dic_params); }

                splash.ShowActivated = false;

                int splash_time = Int32.Parse(usethis["splash_time"]);

                System.Drawing.Color text = System.Drawing.ColorTranslator.FromHtml("#" + usethis["splash_text"]);
                System.Drawing.Color bckgr = System.Drawing.ColorTranslator.FromHtml("#" + usethis["splash_background"]);
                System.Drawing.Color brdr = System.Drawing.ColorTranslator.FromHtml("#" + usethis["splash_border"]);

                int visibility = Int32.Parse(usethis["splash_visibility"]);

                int display_shadow;

                if (usethis["splash_shadow"] == "True")
                {
                    display_shadow = 1;
                }
                else
                {
                    display_shadow = 0;
                }

                double width = Convert.ToDouble((songname.Count() * 22));

                this.c_myTextBlock.Text = songname;
                this.c_border1.Width = width;
                this.c_rectangle1.Width = width;

                if (width < 200)
                {
                    this.Width = 300;
                }
                else
                {
                    this.Width = width + 60;
                }
                this.c_shadow1.Opacity = display_shadow;

                System.Drawing.Color bck_color = System.Drawing.Color.FromArgb(((int)(255 * ((double)visibility / 100))), bckgr.R, bckgr.G, bckgr.B);
                this.c_rectangle1.Fill = ConvertToBrush(bck_color);

                System.Drawing.Color text_color = System.Drawing.Color.FromArgb(255, text.R, text.G, text.B);
                this.c_myTextBlock.Foreground = ConvertToBrush(text_color);

                System.Drawing.Color border_color = System.Drawing.Color.FromArgb(255, brdr.R, brdr.G, brdr.B);
                this.c_border1.BorderBrush = ConvertToBrush(border_color);

                Thread.Sleep(5);

                splash.Show();
                StartCloseTimer(splash_time);
                splash.Close();
            }
            else
            {
                //splash.Close();
            }
        }

        public System.Windows.Media.Brush ConvertToBrush(System.Drawing.Color color)
        {
            System.Windows.Media.Color c2 = new System.Windows.Media.Color();
            c2.A = color.A;
            c2.R = color.R;
            c2.G = color.G;
            c2.B = color.B;

            var converter = new System.Windows.Media.BrushConverter();
            return (System.Windows.Media.Brush)converter.ConvertFromString(c2.ToString());
        }

        public void StartCloseTimer(int time)
        {
            //Show();
            Thread.Sleep(time - 1);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            Win32m.makeTransparent(hwnd);
        }

    }
}
