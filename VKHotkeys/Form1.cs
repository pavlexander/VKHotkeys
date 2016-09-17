using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;

using System.IO;

using System.Runtime.InteropServices;
using System.Diagnostics;

using System.Text.RegularExpressions;

using System.Media;
using System.Windows.Interop;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

using System.Net;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using System.Web.Script.Serialization;

using System.Threading.Tasks;

using System.Web;

using System.Configuration;


using VKHotkeys.VK;
using VKHotkeys.Data;
using VKHotkeys.Parsers;

namespace VKHotkeys
{
    public partial class Form1 : Form
    {
        public int ProfileIndex
        {
            get { return this.comboBox1.SelectedIndex; }
            set { this.comboBox1.SelectedIndex = value; }
        }

        delegate void additem1(string str);
        delegate void callmethod();

        private KeyboardHook hook = new KeyboardHook();
        private Dictionary<string, string> splash_params;
        private Dictionary<string, int> hotkeyDic;
        private bool HKenabled;

        public static Dictionary<string, string> settings;

        public Thread LANcontrol;
        public bool busy = false;
        public bool mustclose = false;

        public Song_Select ss_form;

        public TcpListener tcplisten;

        public static string login_info;
        public static string pas_info;

        //private bool authorized;
        //public Commands commands_proccessor;
        //private static int current_playing_id { get { return Commands.current_playing_id; } }

        public bool HKEnabledEx;

        public Form1()
        {
            InitializeComponent();

            panel1.Width = this.Width;

            label26.Parent = pictureBox5; label26.BackColor = Color.Transparent;
            label49.Parent = pictureBox5; label49.BackColor = Color.Transparent;
            label50.Parent = pictureBox5; label50.BackColor = Color.Transparent;

            label3.Parent = pictureBox5; label3.BackColor = Color.Transparent;

            label51.Parent = pictureBox5; label51.BackColor = Color.Transparent;
            label24.Parent = pictureBox5; label24.BackColor = Color.Transparent;
            label53.Parent = pictureBox5; label53.BackColor = Color.Transparent;
            label1.Parent = pictureBox5; label1.BackColor = Color.Transparent;

            label44.Parent = pictureBox5; label44.BackColor = Color.Transparent;
            label47.Parent = pictureBox5; label47.BackColor = Color.Transparent;
            label55.Parent = pictureBox5; label55.BackColor = Color.Transparent;
            label2.Parent = pictureBox5; label2.BackColor = Color.Transparent;

            GetHostAndIP();

            comboBox1.SelectedIndex = Properties.Settings.Default.last_profile;

            InitializeCommands();

            hotkeyDic = new Dictionary<string, int>();
            splash_params = new Dictionary<string, string>();

            notifyIcon1.Visible = true;
            HKEnabledEx = false;

            axWindowsMediaPlayer1.settings.setMode("loop", true);

            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
        }

        public void GetHostAndIP()
        {
            try
            {
                myhost = Dns.GetHostName();
            }
            catch
            {
                myhost = "Error!";
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (settings == null || settings["gen_minimize_totray"] == "True")
            {
                MinimizeToTrayMin();
            }
        }

        public void MinimizeToTrayMin()
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(200);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        public void MinimizeToTrayCl()
        {
            if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(200);
                this.Hide();
            }
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (busy)
            {
                return;
            }

            if (!authorized)
            {
                return;
            }

            string command = e.Modifier.ToString() + e.Key.ToString();

            if (hotkeyDic.ContainsKey(command))
            {
                busy = true;
                
                if (hotkeyDic[command] == 0) // lock
                {
                    if (HKenabled && !HKEnabledEx)
                    {
                        CleanHotkeysExceptOne();
                        ShowSplash("All hotkeys except THIS one were unloaded", "3");
                    }
                    else if (HKEnabledEx)
                    {
                        HKEnabledEx = false;
                        RegisterAllHotkeys();
                        ShowSplash("Hotkeys are now loaded. (profile: '" + comboBox1.SelectedItem.ToString() + "')", "3");
                    }
                }
                else if (hotkeyDic[command] == 1) // pause/play
                {
                    hk_Play_Pause();

                    ShowSplash(hk_current_song_name(), "1");
                }
                else if (hotkeyDic[command] == 2) // back
                {
                    hk_Prev();

                    //ShowSplash(hk_current_song_name(), "1");
                }
                else if (hotkeyDic[command] == 3) // next
                {
                    hk_Next();

                    //ShowSplash(hk_current_song_name(), "1");
                }
                else if (hotkeyDic[command] == 4) // add
                {
                    if (currect_album.Name == "sug" || currect_album.Name == "pop" || currect_album.Name == "friend")
                     {
                         hk_Add();
                         ShowSplash(hk_current_song_name() + " was added.", "1");
                     }
                     else
                     {
                         ShowSplash("Can't add song from your own album.", "1");
                     }
                }
                else if (hotkeyDic[command] == 5) // goto: my music 
                {
                    hk_Repeat();

                    string rep = "Off";
                    if (repeat) { rep = "On"; }

                    ShowSplash("Repeat is now: " + rep, "1");
                }
                else if (hotkeyDic[command] == 6) // goto: my music 
                {
                    hk_MyMysic();

                    //ShowSplash(hk_current_song_name(), "1");
                }
                else if (hotkeyDic[command] == 7) // goto: suggestions
                {
                    hk_Suggestions();

                    //ShowSplash(hk_current_song_name(), "1");
                }
                else if (hotkeyDic[command] == 8) // goto: popular
                {
                    hk_Popular();

                    //ShowSplash(hk_current_song_name(), "1");
                }
                else if (hotkeyDic[command] == 9) // download
                {
                    hk_Download();

                    ShowSplash("Downloaded", "1");
                }
                else if (hotkeyDic[command] == 10) // show current
                {
                    ShowSplash(hk_current_song_name(), "ex");
                }
                else if (hotkeyDic[command] == 11)  // search and play song
                {
                    using (ss_form = new Song_Select())
                    {
                        var result = ss_form.ShowDialog();

                        string result_of_form;

                        if (result == DialogResult.OK)
                        {
                            int val = ss_form.ReturnValue1;

                            axWindowsMediaPlayer1.Ctlcontrols.playItem(pl.get_Item(val));
                            //browser_ex.command_Play_selected_song(val, out result_of_form); // I have chone to play song "val"
                        }
                        else
                        {
                            result_of_form = "Error! Press OK next time";
                        }

                        //ShowSplash(hk_current_song_name(), "1");
                    }
                }
                else if (hotkeyDic[command] == 12) // from beginning
                {
                    hk_First();

                    //ShowSplash(hk_current_song_name(), "1");
                }
            }

            Application.DoEvents();
            busy = false;
        }

        public void ShowSplash(string text, string reason) 
        {
            if (reason == "1" && settings["splash_display_for_songs"] != "True") { return; }
            //if (reason == "2" && settings["splash_for_profiles"] != "yes") { return; }
            if (reason == "3" && settings["splash_display_for_lock"] != "True") { return; }

            Splasher splash2 = new Splasher();
            splash2.LoadSplasherParams(splash2, text, reason);
        }

        public void RegisterAllHotkeys()
        {
            hotkeyDic = new Dictionary<string, int>();
            CleanHotkeys();

            foreach (KeyValuePair<String, String> setting in settings)
            {
                if (setting.Key.StartsWith("hk") && setting.Value != "")
                {
                    string result = Regex.Replace(setting.Key, @"[^\d]", "");
                    UpdateHotkey(setting.Value, Convert.ToInt32(result));
                }

            }

            if (button2.InvokeRequired) { button2.BeginInvoke(new additem1(additemBut1), "Unload"); }
            else { button2.Text = "Unload"; }

            HKenabled = true;
        }

        private void UpdateHotkey(string textFrom, int command)
        {
            if (textFrom.Contains('+'))
            {
                string modifier_key = textFrom.Split('+')[0].Trim();
                string key_key = textFrom.Split('+')[1].Trim();

                Keys key_restored = (Keys)Enum.Parse(typeof(Keys), key_key);
                VKHotkeys.ModifierKeys mod_key_restored = (ModifierKeys)Enum.Parse(typeof(VKHotkeys.ModifierKeys), modifier_key);

                hook.RegisterHotKey(command, mod_key_restored, key_restored);
                hotkeyDic.Add((modifier_key + key_key), command);
            }
            else
            {
                Keys key_restored = (Keys)Enum.Parse(typeof(Keys), textFrom);
                hook.RegisterHotKey(command, VKHotkeys.ModifierKeys.None, key_restored);
                hotkeyDic.Add((VKHotkeys.ModifierKeys.None.ToString() + textFrom), command);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowForm();
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((settings == null || (settings["gen_close_totray"].ToString() != "True")) && button8.Text == "Stop")
            {
                LANcontrol.Abort();
                tcplisten.Stop();
            }

            if (mustclose || (settings == null))
            {
                notifyIcon1.Visible = false;
                hook.Dispose();
            }
            else if (((settings["gen_close_totray"].ToString() == "True")) && (e.CloseReason == CloseReason.UserClosing) && (e.CloseReason != CloseReason.TaskManagerClosing))
            {
                e.Cancel = true;
                MinimizeToTrayCl();
            }
            else
            {
                notifyIcon1.Visible = false;
                hook.Dispose();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (button8.Text == "Stop")
            {
                LANcontrol.Abort();
                tcplisten.Stop();
            }

            mustclose = true;
            this.Close();
        }

        public System.Windows.Media.Brush ConvertToBrush(Color color)
        {
            System.Windows.Media.Color c2 = new System.Windows.Media.Color();
            c2.A = color.A;
            c2.R = color.R;
            c2.G = color.G;
            c2.B = color.B;

            var converter = new System.Windows.Media.BrushConverter();
            return (System.Windows.Media.Brush)converter.ConvertFromString(c2.ToString());
        }

        public void CleanHotkeys()
        {
            if (HKenabled)
            {
                hook.Dispose();
                HKenabled = false;

                if (button2.InvokeRequired) { button2.BeginInvoke(new additem1(additemBut1), "Load"); }
                else { button2.Text = "Load"; }
            }
        }

        void additemBut1(string str)
        {
            button2.Text = str;
        }

        public void CleanHotkeysExceptOne()
        {
            if (HKenabled)
            {
                hook.DisposeExceptOne();
                //HKenabled = false;
                HKEnabledEx = true;

                //label27.Text = "not loaded (except 1)";

                if (button2.InvokeRequired) { button2.BeginInvoke(new additem1(additemBut1), "Load (all)"); }
                else { button2.Text = "Load (all)"; }
            }
        }

        private bool button_start(object sender)
        {
            if (busy)
            {
                return true;
            }
            busy = true;
            Button but = (Button)sender;
            but.Enabled = false;
            //this.Enabled = false;

            return false;
        }

        private void button_finish(object sender)
        {
            Button but = (Button)sender;
            Application.DoEvents();
            but.Enabled = true;
            //this.Enabled = true;
            busy = false;
        }

        private System.Drawing.Bitmap ChangeColors(System.Drawing.Bitmap image)
        {
            HueModifier filter = new HueModifier(40);
            SaturationCorrection filter2 = new SaturationCorrection(0.4f);
            // apply the filter
            //filter.ApplyInPlace(image);

            return filter2.Apply(filter.Apply(image));
        }

        private void LANlistener()
        {
            int port = Convert.ToInt32(settings["info_port"].ToString());

            tcplisten = new TcpListener(IPAddress.Any, port);
            
            tcplisten.Start();
            TcpClient handlerSocket = tcplisten.AcceptTcpClient();
            while (true)
            {
                if (handlerSocket.Connected)
                {
                    string reccom = "";
                    string sentcom = "";

                    try
                    {
                        using (NetworkStream Nw = new NetworkStream(handlerSocket.Client))
                        {
                            using (StreamReader reader = new StreamReader(Nw))
                            {
                                string toreceive = reader.ReadLine();
                                reccom = toreceive;
                                string text_message = DoCommand(toreceive);
                                sentcom = text_message;
                                using (StreamWriter writer = new StreamWriter(Nw))
                                {
                                    writer.WriteLine(text_message);
                                    writer.Flush();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error received at: Server" + "\n Received message: " + reccom + "\n Sent message: " + sentcom + "\n Error message: \n " + ex.Message + "\n Source: \n" + ex.Source.ToString(), "Printscreen and send me image!");
                    }

                    busy = false;
                    handlerSocket.Close();
                    //tcplisten.Start();
                    handlerSocket = tcplisten.AcceptTcpClient();
                }
            }
        }

        public string DoCommand(string mes)
        {
            string output = "Success!";
            
            if (busy)
            {
                return "Error! Application is busy";
            }

            if (!authorized)
            {
                return "Error! You must be logged in!";
            }

            busy = true;

            if (mes == "play") // play
            {
                hk_Play_Pause();

                output = "play/pause";
            }
            else if (mes == "prev") // previous
            {
                hk_Prev();

                output = "Back";
            }
            else if (mes == "next") // next
            {
                hk_Next();

                output = "Next";
            }
            else if (mes == "myalb") // go to my album
            {
                this.BeginInvoke(new callmethod(hk_MyMysic));
                //hk_MyMysic();

                output = "My album is playing!";
            }
            else if (mes == "sug") // go to suggestions
            {
                this.BeginInvoke(new callmethod(hk_Suggestions));
                //hk_Suggestions();

                output = "Suggestions are playing!";
            }
            else if (mes == "pop") // go to popular
            {
                this.BeginInvoke(new callmethod(hk_Popular));
                //hk_Popular();

                output = "Popular is playing!";
            }
            else if (mes == "first") // play first
            {
                hk_First();

                output = "Fist song is playing!";
            }
            else if (mes == "add") // add
            {
                if (currect_album.Name == "sug" || currect_album.Name == "pop")
                {
                    hk_Add();
                    output = hk_current_song_name() + " was added.";
                }
                else
                {
                    output = "Can't add song from your own album.";

                }
            }
            else if (mes == "download") // downloaded
            {
                hk_Download();

                output = "Downloaded";
            }
            else if (mes == "rep") // repeat
            {
                this.BeginInvoke(new callmethod(hk_Repeat));
                //hk_Repeat();

                if (repeat) { output = "Repeat is now ON"; }
                else { output = "Repeat is now OFF"; }
            }
            else if (mes == "muslist") // get all songs
            {
                List<string> songlist = new List<string>();

                songlist = GetMusicListSer();

                if (songlist == null)
                {
                    output = "Error! Couldn't get the list!";
                }
                else
                {
                    output = new JavaScriptSerializer().Serialize(songlist);
                }
            }
            else if (mes.Contains("audio")) // play THIS song
            {
                axWindowsMediaPlayer1.Ctlcontrols.playItem(pl.get_Item(Convert.ToInt32(mes.Split(' ')[1])));

                output = "playing";
            }
            else if (mes == "ref") // refresh info
            {
                string rerf;

                if (repeat) { rerf = "ON"; }
                else { rerf = "OFF"; }

                string[] refresher = new string[2] { currect_album.Name, rerf };

                output = new JavaScriptSerializer().Serialize(refresher);
            }
            else
            {
                output = "Error! Connection failed at some point..";
            }

            Application.DoEvents();
            busy = false;

            return output;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                current_playing_id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                axWindowsMediaPlayer1.Ctlcontrols.playItem(pl.Item[current_playing_id]);

                //label50.Text = currect_album.Songs[current_playing_id].Author.ToString() + " - " + currect_album.Songs[current_playing_id].Name.ToString();
            }

            SetSelectionDGW();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (settings["login_name"] == "" || settings["login_pas"] == "")
            {
                MessageBox.Show("Login or Password is not set", "Can't login");
                return;
            }

            if (button_start(sender)) return;

            string error = "";
            
            bool result1 = CommandLogin(out error);
            if (!result1) { MessageBox.Show("Error code: " + error + ". \nYour login is: '" + settings["login_name"] + "'.\nYour pas is: '" + settings["login_pas"] + "'. \nPlease double-check if everything is correct.", "Can't login"); button_finish(sender); return; }

            bool result2 = RefreshAlbumList(out error);
            if (!result2) { MessageBox.Show(error, "Can't refresh album list"); button_finish(sender); return; }

            comboBox2.SelectedIndex = mymus;

            button_finish(sender);

            //getfriends();


            if ( result1 && result2 ) { button1.Enabled = false; button1.Text = "Success"; button1.BackColor = Color.Gray; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button_start(sender)) return;

            if (HKenabled)
            { CleanHotkeys(); button2.Text = "Load"; ShowSplash("Unloaded", "ex"); }
            else
            { RegisterAllHotkeys(); ShowSplash("All hotkeys were loaded", "ex"); }

            button_finish(sender);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!authorized) { return;  }

            //int total = comboBox2.Items.Count;
            int index = comboBox2.SelectedIndex;
            string value = comboBox2.Text.ToString();

            currect_album = null;
            textBox27.Text = ""; // search ""

            dataGridView1.Rows.Clear();

            if (index == Endoflist - 1)
            {
                CommandPopular(); // popular
                
            }
            else if (index == Endoflist - 2)
            {
                CommandSuggestions(); //suggestions
            }
            else if (index >= Endoflist) // friends
            {
                CommandFriend(index - Endoflist); // index in list
            }
            else
            {
                CommandMyMysic();
                
            }

            dataGridView1.Focus();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            hk_Repeat();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            About about_form = new About();
            about_form.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (button_start(sender)) return;

            Process.Start("http://vkhotkeys.tumblr.com/donate/");

            button_finish(sender);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            HowTo howto_form = new HowTo();
            howto_form.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (settings["unload_on_remote"] == "True")
            {
                if (HKenabled)
                {

                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new callmethod(CleanHotkeys));
                    }
                    else
                    {
                        CleanHotkeys();
                    }

                    ShowSplash("Unloaded", "ex");
                }
            }

            if (button8.Text == "Start")
            {
                LANcontrol = new Thread(LANlistener);
                //LANcontrol.SetApartmentState(ApartmentState.STA);
                //LANcontrol.IsBackground = true;
                LANcontrol.Start();
                button8.Text = "Stop";
            }
            else
            {
                LANcontrol.Abort();
                tcplisten.Stop();
                button8.Text = "Start";
            }
        }

        private void button4_Click(object sender, EventArgs e) // open settings
        {
            if (button_start(sender)) return;

            //unload all hotkeys
            if (HKenabled) { CleanHotkeys(); ShowSplash("unloaded", "ex"); }

            Settings myset = new Settings(this);
            myset.Show();

            button_finish(sender);
        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            if (currect_album == null) { return;  }

            string search = textBox27.Text;

            if (search == "")
            {
                PopulateDGW();
            }
            else
            {
                dataGridView1.Rows.Clear();

                for (int i = 0; i < currect_album.Songs.Count; i++)
                {
                    if (ContainsNew(currect_album.Songs[i].FileNameForSave, search))
                    {
                        dataGridView1.Rows.Add(i, null, currect_album.Songs[i].Author + " - " + currect_album.Songs[i].Name);
                    }
                }
            }

            SetSelectionDGW();

        }

        public bool ContainsNew(string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        public void PopulateDGW()
        {
            dataGridView1.Rows.Clear();

            for (int i = 0; i < currect_album.Songs.Count; i++)
            {
                dataGridView1.Rows.Add(i, null, currect_album.Songs[i].Author + " - " + currect_album.Songs[i].Name);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // user data

            if (HKenabled)
            { CleanHotkeys(); button2.Text = "Load"; ShowSplash("Unloaded", "ex"); }

            RefreshData();
        }

        public void RefreshData()
        {
            settings = new Dictionary<string, string>();

            int index = comboBox1.SelectedIndex;
            //dynamic property;

            if (index == 0)
            {
                var property = Properties.profile1.Default;

                foreach (SettingsProperty currentProperty in property.Properties)
                {
                    string name = currentProperty.Name;
                    string value = property[name].ToString();

                    settings.Add(name, value);
                }

            }
            else if (index == 1)
            {
                var property = Properties.profile2.Default;

                foreach (SettingsProperty currentProperty in property.Properties)
                {
                    string name = currentProperty.Name;
                    string value = property[name].ToString();

                    settings.Add(name, value);
                }

            }
            else if (index == 2)
            {
                var property = Properties.profile3.Default;

                foreach (SettingsProperty currentProperty in property.Properties)
                {
                    string name = currentProperty.Name;
                    string value = property[name].ToString();

                    settings.Add(name, value);
                }
            }

            //globals

            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                string name = currentProperty.Name;
                string value = Properties.Settings.Default[name].ToString();

                settings.Add(name, value);
            }

            textBox28.Text = settings["login_name"];
            textBox29.Text = settings["login_pas"];

            label3.Text = myhost + " : " + settings["info_port"];

            Properties.Settings.Default.last_profile = comboBox1.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        public string myhost;

        public bool ended = false;

        public WMPLib.IWMPMedia latest_song;
        public WMPLib.IWMPMedia current_song;

        /*if (ended)
{
    axWindowsMediaPlayer1.Ctlcontrols.playItem(latest_song);
                
    ended = false;
    return;
}*/
        bool skipnext = false;
        bool skipnextnext = false;
        bool skipextra = false;

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (ended)
            {
                skipnext = true;
                ended = false;
                axWindowsMediaPlayer1.Ctlcontrols.playItem(latest_song);
                return;
            }

            if (e.newState == 6) // buffering
            {
                skipextra = true;
            }

            if (e.newState == 8) // media ended
            {
                if (repeat)
                {
                    if (currect_album.Songs.Count < 2) { return; }
                    latest_song = current_song;
                    ended = true;
                    return;
                }

                if (current_playing_id == currect_album.Songs.Count - 1) // end of playlist
                {
                    if (comboBox2.SelectedIndex == comboBox2.Items.Count - 1)
                    {
                        CommandExtendPopular(); // popular

                    }
                    else if (comboBox2.SelectedIndex == comboBox2.Items.Count - 2)
                    {
                        CommandExtendSuggestions(); //suggestions
                    }
                }

            }
            else if (e.newState == 3) // playing
            {
                if (skipnext) { skipnext = false; skipnextnext = true; return; }
                if (skipnextnext) { skipnextnext = false; return; }
                if (skipextra) { skipextra = false; return; }

                UpdateSongInfo();
                SetSelectionDGW();
            }

            /*
            if (e.newState == 9 && repeat && ended) { axWindowsMediaPlayer1.Ctlcontrols.playItem(latest_song); return; }

            if (e.newState == 8) // media ended
            {
                if (repeat)
                {
                    ended = true;
                    latest_song = current_song;
                    return;
                    //axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
                    //axWindowsMediaPlayer1.Ctlcontrols.previous();
                    //axWindowsMediaPlayer1.Ctlcontrols.playItem(axWindowsMediaPlayer1.Ctlcontrols.currentItem);
                }

                if (current_playing_id == currect_album.Songs.Count - 1) // end of playlist
                {
                    if (comboBox2.SelectedIndex == comboBox2.Items.Count - 1)
                    {
                        CommandExtendPopular(); // popular

                    }
                    else if (comboBox2.SelectedIndex == comboBox2.Items.Count - 2)
                    {
                        CommandExtendSuggestions(); //suggestions
                    }
                }

            }
            else if (e.newState == 3) // playing
            {
                if (repeat || ended) { ended = false; return; }
                UpdateSongInfo();
                SetSelectionDGW();
            }*/
          
        }

        public void SetSelectionDGW()
        {
            if ( current_playing_id  < dataGridView1.Rows.Count)
            {
                dataGridView1.Rows[current_playing_id].Selected = true;
            }

            /*
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                int value = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value); 

                if (value == current_playing_id)
                {
                    dataGridView1.Rows[i].Selected = true;
                    break;
                }

                if (value > current_playing_id)
                {
                    break;
                }

            }*/
        }

        public void UpdateSongInfo()
        {
            current_song = axWindowsMediaPlayer1.Ctlcontrols.currentItem;
            current_playing_id = Convert.ToInt32(axWindowsMediaPlayer1.Ctlcontrols.currentItem.getItemInfo("s_id").ToString());
            label50.Text = hk_current_song_name();

            /*current_song = axWindowsMediaPlayer1.Ctlcontrols.currentItem;
            //MessageBox.Show(axWindowsMediaPlayer1.Ctlcontrols.currentItem.getItemInfo("s_id").ToString());

            string source = current_song.sourceURL;

            for (int i = 0; i < currect_album.Songs.Count; i++ )
            {
                if (currect_album.Songs[i].DownloadURL == source)
                {
                    current_playing_id = i;
                    label50.Text = hk_current_song_name();
                    break;
                }
            }*/

        }

        private List<string> ListOfAllSongs;

        public void grabListOfAllSongs(Album newalbum)
        {
            ListOfAllSongs = new List<string>();

            foreach (Song fsdfsdfs in newalbum.Songs)
            {
                ListOfAllSongs.Add(fsdfsdfs.Author + " - " + fsdfsdfs.Name);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!authorized)
            {
                MessageBox.Show("You must login first");
                return;
            }

            grabListOfAllSongs(currect_album);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "Full list of songs (" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ")"; // Default file name
            saveFileDialog1.DefaultExt = ".txt"; // Default file extension
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog1.FileName, ListOfAllSongs.ToArray());
            }

            //axWindowsMediaPlayer1.Ctlcontrols.pause();
            //fixaudio formfix = new fixaudio(this);
            //formfix.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!authorized)
            {
                MessageBox.Show("You must login first");
                return;
            }

            hk_Shuffle();
        }

    }
}
