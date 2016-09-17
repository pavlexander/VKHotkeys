using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatiN;
using WatiN.Core;

using System.Windows.Forms;

using System.Threading;

using System.IO;

using System.Runtime.InteropServices;
using System.Diagnostics;

using System.Text.RegularExpressions;

using System.Net;

using System.Web;

using Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace VKHotkeys
{
    class ExternalBrowser
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        private IE myie;
        private string current_location_main;
        private string current_location_sub;
        //private string login = "";
        //private string pass = "";
        private IntPtr window_handle;
        private bool hidden = false;
        private static bool autorename { get { return Form1.autorename; } }
        private static Dictionary<string, string> dic_params { get { return Form1.dic_params; } }

        private static string login_info { get { return Form1.login_info; } }
        private static string pas_info { get { return Form1.pas_info; } }

        public void Start()
        {
            try
            {
                myie = new IE();
            }
            catch (Exception ex)
            {
                fileoper.writeLog(DateTime.Now + "_IE_create_error.log", ex.Message);
                return;
            }

            myie.AutoClose = true;
            myie.GoTo("www.vk.com");
            myie.WaitForComplete();
            window_handle = myie.hWnd;

            current_location_main = "Home";
            current_location_sub = "";

            if (!IsLoggedIn())
            {
                //if (dic_params.ContainsKey("login") && dic_params.ContainsKey("pass"))
                if (login_info != "" && pas_info != "")
                {
                    Login();
                }
            }

            myie.WaitForComplete();

            Thread.Sleep(1000);

            if (!myie.TextField(Find.ById("quick_email")).Exists)
            {
                string songname = "";

                command_GoTo_My_music(out songname);
            }
        }

        public void UpdateMyLocation()
        {
            if (myie.Url.Contains("audio"))
            {
                current_location_main = "Music";

                if (myie.Url.Contains("recommendations"))
                {
                    current_location_sub = "Suggestions";
                }
                else if (myie.Url.Contains("popular"))
                {
                    current_location_sub = "Popular";
                }
                else if (myie.Url.Contains("audio"))
                {
                    current_location_sub = "Album";
                }
                else
                {
                    current_location_sub = "";
                }
            }
            else
            {
                current_location_main = "";
                current_location_sub = "";
            }

        }

        public bool BroExists()
        {
            if (window_handle == null) { return false; }
            return IsWindow(window_handle);
        }

        public void ShowHideWin()
        {
            if (BroExists())
            {
                if (!hidden) { myie.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.Hide); hidden = true; }
                else { myie.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.Show); hidden = false; }
            }
        }

        public void CloseWin()
        {
            if (BroExists())
            {
                myie.Close();
            }
        }

        public string error_log_in = "Error! You must be Logged In!";

        public void DownloadSong(out string result)
        {
            result = "";
            if (!IsLoggedIn())
            {
                result = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_main == "Music")
            {
                string download_name = "";
                string download_link_name = myie.Eval("javascript: songn = audioPlayer.lastSong[0] + '_' + audioPlayer.lastSong[1];");
                string download_link = myie.Eval("javascript: songn = audioPlayer.lastSong[0] + '_' + audioPlayer.lastSong[1]; url = document.getElementById('audio_info'+songn).value.split(',')[0];");

                download_link_name = download_link_name + "";
                if (autorename)
                {
                    download_name = myie.Eval("javascript: sname = audioPlayer.lastSong[5] + ' _ ' + audioPlayer.lastSong[6];");
                    download_name = Regex.Replace(HttpUtility.HtmlDecode(download_name), @"[^\w\s:]", "", RegexOptions.IgnorePatternWhitespace);
                }
                else
                {
                    download_name = Path.GetFileNameWithoutExtension(download_link);
                }
                
                string newpath = Path.Combine(@"downloads\", download_name + ".mp3");

                if (!Directory.Exists(@"downloads\")) { Directory.CreateDirectory(@"downloads\"); }
                if (File.Exists(newpath)) { result = "Error! File with that name already exists"; return; }

                
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(download_link, newpath);
                    }
                    result = "Saved: " + download_name;
                }
                catch
                {
                    result = "Error! Try again later.";
                }
            }
            else
            {
                result = "Error! Please go to your Music and start playing any song.";
            }
        }

        public void Login()
        {
            TextField login_fld = myie.ElementOfType<TextField>(Find.ById("quick_email"));
            if (!login_fld.Exists) { return; }
            login_fld.Value = login_info;

            TextField pas_fld = myie.ElementOfType<TextField>(Find.ById("quick_pass"));
            if (!pas_fld.Exists) { return; }
            pas_fld.Value = pas_info;

            WatiN.Core.Button sing_in_but = myie.ElementOfType<WatiN.Core.Button>(Find.ById("quick_login_button"));
            if (!sing_in_but.Exists) { return; }
            sing_in_but.Click();

            myie.WaitForComplete();
        }

        public void command_Player_PlayPause(out string songname)
        {
            songname = "";

            if (!IsLoggedIn())
            {
                songname = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_main != "Music") {  command_GoTo_My_music(out songname); }
            else
            {
                Div play_cntr = myie.ElementOfType<Div>(Find.ById("ac_play"));
                if (!play_cntr.Exists) { songname = "Error! No player exist"; return; }
                play_cntr.Click();

                myie.WaitForComplete();

                songname = CurrentSongName();
            }
        }

        public void command_Player_Previous(out string songname)
        {
            songname = "";

            if (!IsLoggedIn())
            {
                songname = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_main != "Music") { command_GoTo_My_music(out songname); }
            else
            {
                Div play_cntr = myie.ElementOfType<Div>(Find.ById("ac_prev"));
                if (!play_cntr.Exists) { songname = "Error! No player exist"; return; }
                play_cntr.Click();

                myie.WaitForComplete();

                songname = CurrentSongName();
            }
        }

        public void command_Player_Next(out string songname)
        {
            songname = "";

            if (!IsLoggedIn())
            {
                songname = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_main != "Music") { command_GoTo_My_music(out songname); }
            else
            {

                Div play_cntr = myie.ElementOfType<Div>(Find.ById("ac_next"));
                if (!play_cntr.Exists) { songname = "Error! No player exist"; return; }
                play_cntr.Click();

                myie.WaitForComplete();

                songname = CurrentSongName();
            }
        }

        public void command_Player_Add(out string songname)
        {
            songname = "";

            if (!IsLoggedIn())
            {
                songname = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_sub != "")
            {
                Div play_cntr = myie.ElementOfType<Div>(Find.ById("ac_add"));
                if (!play_cntr.Exists) { songname = "Error! No player exist"; return; }

                string style = play_cntr.Style.Display;

                if (style != "none")
                {
                    play_cntr.Click();
                    songname = CurrentSongName();
                }
                else
                {
                    songname = "Error! Song is already added";
                }

                //myie.RunScriptIgnoreEx("javascript: audioPlayer.addCurrentTrack();");
                //myie.WaitForComplete(500);

                //songname = CurrentSongName();
                //myie.WaitForComplete(500);
            }
        }

        public void clickOnMyMusic()
        {
            UpdateMyLocation();

            if (current_location_main != "Music")
            {
                Div play_cntr = myie.ElementOfType<Div>(Find.ById("side_bar"));
                if (!play_cntr.Exists) { return; }
                Link mus_link = play_cntr.ElementOfType<Link>(Find.ByUrl("http://vk.com/audio"));
                if (!mus_link.Exists) { return; }

                mus_link.Click();

                myie.WaitForComplete();
                Thread.Sleep(500);
                current_location_main = "Music";
                current_location_sub = "Album";
            }

        }

        public void clickOnMyAlbum()
        {
            UpdateMyLocation();

            if ((current_location_main == "Music") && (current_location_sub != "Album"))
            {
                myie.RunScriptIgnoreEx("javascript: Audio.loadAlbum(0);");
                Thread.Sleep(1000);
                myie.WaitForComplete();
                current_location_sub = "Album";
            }
        }

        public void clickOnSugg()
        {
            UpdateMyLocation();

            if ((current_location_main == "Music") && (current_location_sub != "Suggestions"))
            {
                myie.RunScriptIgnoreEx("javascript: Audio.loadRecommendations(true);");

                string url = myie.Url;
                DateTime current = DateTime.Now;
                while (!url.Contains("recommendations"))
                {
                    Application.DoEvents();
                    url = myie.Url;
                    if (DateTime.Now.Subtract(current).Seconds > 4)
                    {
                        myie.RunScriptIgnoreEx("javascript: Audio.loadRecommendations(true);");
                        Application.DoEvents();
                        Thread.Sleep(2000);
                        break;
                    }
                    Thread.Sleep(500);
                }

                Thread.Sleep(1000);
                myie.WaitForComplete();
                current_location_sub = "Suggestions";
            }
        }

        public void clickOnPopular()
        {
            UpdateMyLocation();

            if ((current_location_main == "Music") && (current_location_sub != "Popular"))
            {
                myie.RunScriptIgnoreEx("javascript: Audio.loadPopular(true);");

                string url = myie.Url;
                DateTime current = DateTime.Now;
                while (!url.Contains("popular"))
                {
                    Application.DoEvents();
                    url = myie.Url;
                    if (DateTime.Now.Subtract(current).Seconds > 4)
                    {
                        myie.RunScriptIgnoreEx("javascript: Audio.loadPopular(true);");
                        Application.DoEvents();
                        Thread.Sleep(2000);
                        break;
                    }
                    Thread.Sleep(500);
                }

                Thread.Sleep(1000);
                myie.WaitForComplete();
                current_location_sub = "Popular";
            }
        }

        public void PlayIfNotPlaying()
        {
            Div play_cntr = myie.ElementOfType<Div>(Find.ById("ac_play"));
            if (!play_cntr.Exists) { return; }

            if (!play_cntr.ClassName.Contains("playing")) { play_cntr.Click(); }
            
        }

        public void command_GoTo_My_music(out string songname)
        {
            songname = "";
            if (!IsLoggedIn())
            {
                songname = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_sub == "Album")
            {
                songname = "Error! You are already playing music from your album";
                PlayIfNotPlaying();
                return;
            }

            
            //clickOnMyMusic();
            //clickOnMyAlbum();
            myie.GoTo("http://vk.com/audio");
            myie.WaitForComplete();

            command_Play_From_Beginning(out songname);
        }

        public void command_GoToSug(out string songname)
        {
            songname = "";
            if (!IsLoggedIn())
            {
                songname = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_sub == "Suggestions")
            {
                songname = "Error! You are already playing suggested music";
                PlayIfNotPlaying();
                return;
            }

            //clickOnMyMusic();
            //clickOnSugg();
            myie.GoTo("http://vk.com/audio?act=recommendations");
            myie.WaitForComplete();

            command_Play_From_Beginning(out songname);
        }

        public void command_GoTo_Popular(out string songname)
        {
            songname = "";
            if (!IsLoggedIn())
            {
                songname = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_sub == "Popular")
            {
                songname = "Error! You are already playing popular music";
                PlayIfNotPlaying();
                return;
            }

            //clickOnMyMusic();
            //clickOnPopular();

            myie.GoTo("http://vk.com/audio?act=popular");
            myie.WaitForComplete();

            command_Play_From_Beginning(out songname);
        }

        public void command_Play_From_Beginning(out string name)
        {
            name = "";
            if (!IsLoggedIn())
            {
                name = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_main != "Music")
            {
                clickOnMyMusic();
            }

            if (current_location_main == "Music")
            {
                Div allmenu = myie.ElementOfType<Div>(Find.ById("initial_list")); if (!allmenu.Exists) { name = "Error! No music list found!"; return; }
                //var song_clickable = allmenu.ElementWithTag("div", Find.ByIndex(2)); if (!song_clickable.Exists) { name = "Error! No song found!"; return; }
                var song_clickable2 = allmenu.ElementWithTag("div", Find.ByIndex(3)); if (!song_clickable2.Exists) { name = "Error! No song found!"; return; }
                song_clickable2.Click();

                myie.WaitForComplete();
                string nameof = CurrentSongName();
                name = nameof;
                //string nameof = song_clickable.Text;
                //name = nameof.Substring(12, nameof.Length - 12).Split('\r')[0];

            }
        }

        public void command_Repeat(out string name)
        {
            name = "";
            if (!IsLoggedIn())
            {
                name = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (current_location_main != "Music")
            {
                clickOnMyMusic();
            }

            if (current_location_main == "Music")
            {
                Div repeat_button = myie.ElementOfType<Div>(Find.ById("ac_repeat"));
                if (repeat_button.Exists)
                {
                    string repeat_button_status = "";
                    repeat_button_status = repeat_button.ClassName;

                    if (repeat_button_status.Contains("on") && repeat_button_status != "")
                    {
                        name = "OFF"; // bilo ON stalo OFF
                    }
                    else
                    {
                        name = "ON";
                    }

                    repeat_button.Click();
                }
                else
                {
                    name = "Error! No Repeat button Found!";
                }
            }
        }

        public string CurrentSongName()
        {
            if (!IsLoggedIn())
            {
                return error_log_in;
            }

            Div current_name = myie.ElementOfType<Div>(Find.ById("ac_name"));
            if (!current_name.Exists) { return "Error! No song is opened"; }

            return current_name.Text;
        }

        public bool IsLoggedIn()
        {
            TextField tfff = null;

            try
            {
                tfff = myie.TextField(Find.ById("quick_email"));
            }
            catch (Exception ex)
            {
                string newlog = Path.Combine(@"log\", Regex.Replace(HttpUtility.HtmlDecode(DateTime.Now.ToString()), @"[^\w\s]", "", RegexOptions.IgnorePatternWhitespace) + "_error_before_hk_registration.log");

                fileoper.writeLog(newlog, ex.Message);
                return false;
            }

            if (tfff.Exists)
            {
                return false;
            }

            return true;
        }

        public void ExpandMusicList(out string output)
        {
            output = "";

            if (!IsLoggedIn())
            {
                output = error_log_in;
                return;
            }

            UpdateMyLocation();

            if (myie.Url.Contains("recommendations") || myie.Url.Contains("popular"))
            {
                output = "Error! Sorry, that won't work with recommendations and popular music!";
                return;
            }

            Div paley = myie.Div(Find.ById("audio")); if (!paley.Exists) { return; }
            Div paley2 = paley.Div(Find.ById("audio_wrap")); if (!paley2.Exists) { return; }
            Div allmenu = paley2.Div(Find.ById("audios_list")); if (!allmenu.Exists) { return; }
            Link morelink = allmenu.Link(Find.ById("more_link")); if (!morelink.Exists) { return; }

            string style = morelink.Style.Display;
            if (style.Contains("none"))
            {
                output = "Error! The list is already expanded!";
                return;
            }

            while (morelink.Style.Display != "none")
            {
                morelink.Click();
                myie.WaitForComplete();
                Thread.Sleep(500);
            }

            output = "Playlist is now expanded";
        }

        public List<string[]> GetMusicListSer()
        {
            if (!IsLoggedIn())
            {
                return null;
            }

            Div paley = myie.Div(Find.ById("audio")); if (!paley.Exists) { return null; }
            Div paley2 = paley.Div(Find.ById("audios_list")); if (!paley2.Exists) { return null; }
            Div allmenu = paley2.Div(Find.ById("initial_list")); if (!allmenu.Exists) { return null; }
            
            List<string[]> ret = new List<string[]>();

            ElementCollection alltext = allmenu.Children();

            int alltextc = alltext.Count; if (alltextc == 0) { return null; }
            int counter = 1;
            for (int i = 0; i < alltextc; i++)
            {
                string textxxx = alltext[i].Text;
                if (textxxx != null)
                {
                    string sub = textxxx.Substring(12, textxxx.Length - 12).Split('\r')[0];
                    ret.Add(new string[]{counter.ToString(), sub, alltext[i].Id});
                    //ret.Add(new string[] { counter.ToString(), Regex.Replace(HttpUtility.HtmlDecode(sub), @"[^\w\s:^-]", "", RegexOptions.IgnorePatternWhitespace), alltext[i].Id });
                    counter++;
                }
            }

            return ret;
        }

        public void command_Play_selected_song(string id, out string name)
        {
            name = "";

            var agency2 = myie.ElementOfType<Div>(Find.ById(id));
            if (!agency2.Exists) { name = "Error! No such song (1)"; return; }

            var agencyTab2 = agency2.ElementWithTag("div", Find.ByIndex(1));
            if (!agencyTab2.Exists) { name = "Error! No such song (2)"; return; }

            agencyTab2.Click();

            string somet1 = agency2.Text;
            name = somet1.Substring(12, somet1.Length - 12).Split('\r')[0];
        }

        public string[] command_Refresh_Info(out string name)
        {
            name = "";

            string repeat_butt_status = "";
            var repeat_button = myie.ElementOfType<Div>(Find.ById("ac_repeat"));
            if (repeat_button.Exists)
            {
                string repeat_button_status = "";
                repeat_button_status = repeat_button.ClassName;

                if (repeat_button_status.Contains("on") && repeat_button_status != "")
                {
                    repeat_butt_status = "ON";
                }
                else
                {
                    repeat_butt_status = "OFF";
                }
            }
            else
            {
                name = "Error! No Repeat button Found!";
                return null;
            }

            string album_status = "";

            if (myie.Url.Contains("recommendations"))
            {
                album_status = "Suggestions";
            }
            else if (myie.Url.Contains("popular"))
            {
                album_status = "Popular";
            }
            else if (myie.Url.Contains("audio"))
            {
                album_status = "My Album";
            }
            else
            {
                album_status = "Unknown";
            }

            return new string[2] { album_status, repeat_butt_status };
        }

    }
}
