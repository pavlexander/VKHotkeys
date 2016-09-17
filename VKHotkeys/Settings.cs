using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;

using System.Runtime.InteropServices;
using System.Diagnostics;

using System.IO;
using System.Text.RegularExpressions;


using System.Net;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using System.Configuration;

namespace VKHotkeys
{
    public partial class Settings : Form
    {
        public string currentactivecontrol;
        public static Dictionary<string, string> dic_params_splash_test;
        public bool busy = false;
        public Dictionary<string, string> settings;

        private Form1 form1;

        public Settings(Form1 form1)
        {
            InitializeComponent();
            currentactivecontrol = "";

            /*
            var combo = Form1.ActiveForm.Controls.Find("comboBox1", true)[0];
            int index = ((ComboBox)combo).SelectedIndex;
            comboBox1.SelectedIndex = index;*/
            
            this.form1 = form1;
            comboBox1.SelectedIndex = form1.ProfileIndex;
        }

        private void pictureBox2_Click(object sender, EventArgs e) { ChoseColor("splash_text", "pictureBox2"); }

        private void pictureBox3_Click(object sender, EventArgs e) { ChoseColor("splash_background", "pictureBox3"); }

        private void pictureBox4_Click(object sender, EventArgs e) { ChoseColor("splash_border", "pictureBox4"); }

        public void ChoseColor(string textBox, string picturebox)
        {
            var txBox = this.Controls.Find(textBox, true)[0];
            var picBox = this.Controls.Find(picturebox, true)[0];
            ColorDialog cd = new ColorDialog();
            cd.Color = picBox.BackColor;
            cd.ShowDialog();
            Color col = new Color();
            col = cd.Color;
            string col_name = col.Name.ToString();
            var startsWithUpper = char.IsUpper(col_name, 0);
            if (startsWithUpper)
            {
                //Color fsdf = ColorTranslator.FromHtml(col_name);
                txBox.Text = Color.FromName(col_name).ToArgb().ToString("X8").ToLower();
            }
            else
            {
                txBox.Text = col_name;
            }
            picBox.BackColor = cd.Color;

            UpdateTextControl(txBox.Text, txBox.Name);
        }

        public void ControlLostFocus(object sender, EventArgs e) { UnlockHandler(currentactivecontrol); }

        protected override bool ProcessDialogKey(Keys keyData) // DISABLE ALT PRESS
        {
            if ((keyData & Keys.Alt) == Keys.Alt) { return true; } else { return base.ProcessDialogKey(keyData); }
        }

        public void MyKeyPress(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            var txBox = this.Controls.Find(currentactivecontrol, true)[0];

            string first = e.Modifiers.ToString();

            if (first != "None")
            {
                if ((e.KeyCode != Keys.ShiftKey) && (e.KeyCode != Keys.Alt) && (e.KeyCode != Keys.ControlKey))
                {
                    txBox.Text = e.Modifiers.ToString() + " + " + e.KeyCode.ToString();

                    UnlockHandler(currentactivecontrol);
                }
            }
            else if (first != "")
            {
                txBox.Text = e.KeyCode.ToString();
                UnlockHandler(currentactivecontrol);
            }

            string command = txBox.Text;
            if (settings.ContainsValue(command)) { command = "";  txBox.Text = ""; }

            UpdateTextControl(command, txBox.Name);
        }

        public void LockHandler(string controlname)
        {
            if (currentactivecontrol != "")
            {
                UnlockHandler(currentactivecontrol);
            }

            currentactivecontrol = controlname;

            var txBox = this.Controls.Find(controlname, true)[0];
            txBox.Focus();
            txBox.BackColor = Color.DodgerBlue;
            txBox.KeyDown += new KeyEventHandler(MyKeyPress);
            txBox.LostFocus += new EventHandler(ControlLostFocus);
        }

        public void UnlockHandler(string controlname)
        {
            var txBox = this.Controls.Find(controlname, true)[0];
            txBox.KeyDown -= MyKeyPress;
            txBox.LostFocus -= ControlLostFocus; //textBox1.KeyDown -= new KeyEventHandler(MyKeyPress);
            txBox.BackColor = Color.FromArgb(191, 205, 219);

            currentactivecontrol = "";
        }

        // Main

        private void vkB_mini1_Click(object sender, EventArgs e) { LockHandler("hk_1_playpause"); }

        private void vkB_mini9_Click(object sender, EventArgs e) { LockHandler("hk_2_previous"); }

        private void vkB_mini3_Click(object sender, EventArgs e) { LockHandler("hk_3_next"); }

        private void vkB_mini5_Click(object sender, EventArgs e) { LockHandler("hk_4_add"); }

        private void vkB_mini7_Click(object sender, EventArgs e) { LockHandler("hk_12_fromstart"); }

        private void vkB_mini27_Click(object sender, EventArgs e) { LockHandler("hk_5_repeat"); }

        // Browser

        private void vkB_mini12_Click(object sender, EventArgs e) { LockHandler("hk_6_play_albums"); }

        private void vkB_mini13_Click(object sender, EventArgs e) { LockHandler("hk_7_play_suggestions"); }

        private void vkB_mini15_Click(object sender, EventArgs e) { LockHandler("hk_8_play_popular"); }

        private void vkB_mini17_Click(object sender, EventArgs e) { LockHandler("textBox22"); }

        // Downloads

        private void vkB_mini20_Click(object sender, EventArgs e) { LockHandler("hk_9_download"); }

        // General

        private void vkB_mini23_Click(object sender, EventArgs e) { LockHandler("hk_10_show_current_song"); }

        private void vkB_mini25_Click(object sender, EventArgs e) { LockHandler("hk_0_global_lock_unlock"); }

        // List

        private void vkB_mini30_Click(object sender, EventArgs e) { LockHandler("hk_11_show_all_songs"); }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            CheckValidFormatHtmlColor("splash_text", "pictureBox2");
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            CheckValidFormatHtmlColor("splash_background", "pictureBox3");
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            CheckValidFormatHtmlColor("splash_border", "pictureBox4");
        }

        private void vkb11_Click(object sender, EventArgs e)
        {
            if (button_start(sender)) return;

            dic_params_splash_test = new Dictionary<string, string>();
            LoadSplashParamsToDic(dic_params_splash_test);

            Splasher splash2 = new Splasher();

            string songname = "Splash - (feat.) Example";
            splash2.LoadSplasherParams(splash2, songname, "test");
            dic_params_splash_test = null;
            Application.DoEvents();
            
            button_finish(sender);
        }

        private void vkb12_Click(object sender, EventArgs e)
        {
            string dir = @"downloads\";
            if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
            Process.Start(dir);
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

        private void CheckValidFormatHtmlColor(string textbox, string picturebox)
        {
            var txBox = this.Controls.Find(textbox, true)[0];
            var picBox = this.Controls.Find(picturebox, true)[0];
            try
            {
                string curtext = txBox.Text.Trim();

                if (curtext[0] == '#')
                {
                    curtext = curtext.Substring(1);
                    txBox.Text = curtext.ToLower();
                }

                picBox.BackColor = ColorTranslator.FromHtml("#" + curtext);
            }
            catch// (Exception ex)
            {
                MessageBox.Show("You have chosen the wrong color: " + txBox.Text + ". \nPlease make sure that the color you are using has a valid HTML format.", "Color Pick");
                txBox.Text = "ffffffff";
                picBox.BackColor = ColorTranslator.FromHtml("#" + "ffffffff");
            }
        }

        public bool IsValidPort(string port)
        {
            if (port == "")
            {
                return false;
            }

            try
            {
                int.Parse(port);
            }
            catch
            {
                return false;
            }

            if (port.Length > 0)
            {
                if (port[0] == '0')
                {
                    return false;
                }
            }

            return true;
        }

        public void FreeCheckbox(string control)
        {
            var chkBox = this.Controls.Find(control, true)[0];

            bool status = false; string command = "False";
            if (((CheckBox)chkBox).Checked) { status = true; command = "True"; }

            settings[chkBox.Name] = command;

            if (comboBox1.SelectedIndex == 0)
            {
                if (Properties.profile1.Default.Properties[chkBox.Name] != null) { Properties.profile1.Default[chkBox.Name] = status; Properties.profile1.Default.Save(); }
                else { Properties.Settings.Default[chkBox.Name] = status; Properties.Settings.Default.Save(); }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                if (Properties.profile2.Default.Properties[chkBox.Name] != null) { Properties.profile2.Default[chkBox.Name] = status; Properties.profile2.Default.Save(); }
                else { Properties.Settings.Default[chkBox.Name] = status; Properties.Settings.Default.Save(); }
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                if (Properties.profile3.Default.Properties[chkBox.Name] != null) { Properties.profile3.Default[chkBox.Name] = status; Properties.profile3.Default.Save(); }
                else { Properties.Settings.Default[chkBox.Name] = status; Properties.Settings.Default.Save(); }
            }
        }

        // hotkeys

        private void vkB_mini2_Click(object sender, EventArgs e) { UpdateTextControl("","hk_1_playpause"); }

        private void vkB_mini10_Click(object sender, EventArgs e) { UpdateTextControl("","hk_2_previous"); }

        private void vkB_mini4_Click(object sender, EventArgs e) { UpdateTextControl("","hk_3_next"); }

        private void vkB_mini6_Click(object sender, EventArgs e) { UpdateTextControl("","hk_4_add"); }

        private void vkB_mini8_Click(object sender, EventArgs e) { UpdateTextControl("","hk_12_fromstart"); }

        private void vkB_mini28_Click(object sender, EventArgs e) { UpdateTextControl("","hk_5_repeat"); }


        // browser

        private void vkB_mini11_Click(object sender, EventArgs e) { UpdateTextControl("","hk_6_play_albums"); }

        private void vkB_mini14_Click(object sender, EventArgs e) { UpdateTextControl("","hk_7_play_suggestions"); }

        private void vkB_mini16_Click(object sender, EventArgs e) { UpdateTextControl("","hk_8_play_popular"); }


        // download

        private void vkB_mini19_Click(object sender, EventArgs e) { UpdateTextControl("","hk_9_download"); }

        // general

        private void vkB_mini24_Click(object sender, EventArgs e) { UpdateTextControl("","hk_10_show_current_song"); }

        private void vkB_mini26_Click(object sender, EventArgs e) { UpdateTextControl("","hk_0_global_lock_unlock"); }

        // list

        private void vkB_mini29_Click(object sender, EventArgs e) { UpdateTextControl("","hk_11_show_all_songs"); }

        // lan

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProfileToSettings();
            LoadSettingsToFields();
        }

        public void LoadSplashParamsToDic(Dictionary<string, string> mydic)
        {
            mydic.Add("splash_time", splash_time.Text);
            mydic.Add("splash_text", splash_text.Text);
            mydic.Add("splash_background", splash_background.Text);
            mydic.Add("splash_border", splash_border.Text);
            //mydic.Add("splash_visibility", ((int)(255 * ((double)splash_visibility.Value / 100))).ToString());
            mydic.Add("splash_visibility", splash_visibility.Value.ToString());
            if (splash_shadow.Checked) { mydic.Add("splash_shadow", "True"); } else { mydic.Add("splash_shadow", "False"); }
            if (splash_display_for_songs.Checked) { mydic.Add("splash_display_for_songs", "True"); } else { mydic.Add("splash_display_for_songs", "False"); }
            //if (splash_display_for_profiles.Checked) { mydic.Add("splash_display_for_profiles", "yes"); } else { mydic.Add("splash_display_for_profiles", "no"); }
            if (splash_display_for_lock.Checked) { mydic.Add("splash_display_for_lock", "True"); } else { mydic.Add("splash_display_for_lock", "False"); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetProfile();
            LoadProfileToSettings();
            LoadSettingsToFields();
        }

        public void ResetProfile()
        {
            int index = comboBox1.SelectedIndex;

            if (index == 0)
            {
                var property = Properties.profile1.Default;
                property.Reset();
                //property.Save();
            }
            else if (index == 1)
            {
                var property = Properties.profile2.Default;
                property.Reset();
                //property.Save();
            }
            else if (index == 2)
            {
                var property = Properties.profile3.Default;
                property.Reset();
                //property.Save();
            }

            Properties.Settings.Default.Reset();
            //Properties.Settings.Default.Save();
        }

        private void LoadProfileToSettings()
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

                //property.Reset();

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

                //property.Reset();

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

        }

        private void LoadSettingsToFields()
        {
            foreach (Control control in this.Controls)
            {
                if (control is GroupBox)
                {
                    foreach (Control subcontrol in control.Controls)
                    {
                        if (settings.ContainsKey(subcontrol.Name))
                        {
                            if (subcontrol is TextBox)
                            {
                                ((TextBox)subcontrol).Text = settings[subcontrol.Name];
                            }
                            else if (subcontrol is CheckBox)
                            {
                                if (settings[subcontrol.Name] == "True") { ((CheckBox)subcontrol).Checked = true; } else { ((CheckBox)subcontrol).Checked = false; }
                            }
                            else if (subcontrol is TrackBar)
                            {
                                ((TrackBar)subcontrol).Value = Convert.ToInt32(settings[subcontrol.Name]);
                            }
                        }
                    }
                }
            }

        }

        private void splash_shadow_CheckedChanged(object sender, EventArgs e)
        {
            FreeCheckbox("splash_shadow");
        }

        private void unload_on_remote_CheckedChanged(object sender, EventArgs e)
        {
            FreeCheckbox("unload_on_remote");
        }

        private void splash_display_for_songs_CheckedChanged(object sender, EventArgs e)
        {
            FreeCheckbox("splash_display_for_songs");
        }

        private void splash_display_for_profiles_CheckedChanged(object sender, EventArgs e)
        {
            FreeCheckbox("splash_display_for_profiles");
        }

        private void splash_display_for_lock_CheckedChanged(object sender, EventArgs e)
        {
            FreeCheckbox("splash_display_for_lock");
        }

        private void gen_close_totray_CheckedChanged(object sender, EventArgs e)
        {
            FreeCheckbox("gen_close_totray");
        }

        private void gen_minimize_totray_CheckedChanged(object sender, EventArgs e)
        {
            FreeCheckbox("gen_minimize_totray");
        }

        public void UpdateTextControl(dynamic command, string controlname)
        {
            var control = this.Controls.Find(controlname, true)[0];
            settings[control.Name] = command.ToString();
            control.Text = command.ToString();

            if (comboBox1.SelectedIndex == 0)
            {
                if (Properties.profile1.Default.Properties[control.Name] != null) { Properties.profile1.Default[control.Name] = command; Properties.profile1.Default.Save(); }
                else { Properties.Settings.Default[control.Name] = command; Properties.Settings.Default.Save(); }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                if (Properties.profile2.Default.Properties[control.Name] != null) { Properties.profile2.Default[control.Name] = command; Properties.profile2.Default.Save(); }
                else { Properties.Settings.Default[control.Name] = command; Properties.Settings.Default.Save(); }
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                if (Properties.profile3.Default.Properties[control.Name] != null) { Properties.profile3.Default[control.Name] = command; Properties.profile3.Default.Save(); }
                else { Properties.Settings.Default[control.Name] = command; Properties.Settings.Default.Save(); }
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            string command = splash_time.Text;

            if (IsValidPort(command)) { UpdateTextControl(Convert.ToInt32(command), "splash_time"); }
            else { splash_time.Text = Convert.ToString(1500); UpdateTextControl(1500, "splash_time"); }
        }

        private void info_port_TextChanged(object sender, EventArgs e)
        {
            string command = info_port.Text;

            if (IsValidPort(command)) { UpdateTextControl(Convert.ToInt32(command), "info_port"); }
            else { info_port.Text = Convert.ToString(1580); UpdateTextControl(1580, "info_port"); }
        }

        private void login_name_TextChanged(object sender, EventArgs e)
        {
            string command = login_name.Text;
            UpdateTextControl(command, "login_name");
        }

        private void login_pas_TextChanged(object sender, EventArgs e)
        {
            string command = login_pas.Text;
            UpdateTextControl(command, "login_pas");
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            form1.RefreshData();
        }

        private void splash_visibility_ValueChanged(object sender, EventArgs e)
        {
            int command = splash_visibility.Value;

            UpdateTextControl(command, "splash_visibility");
        }
    }
}
