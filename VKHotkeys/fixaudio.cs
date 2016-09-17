using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using VKHotkeys.VK;
using VKHotkeys.Data;
using VKHotkeys.Parsers;

using System.IO;
using System.Runtime.InteropServices;

namespace VKHotkeys
{
    public partial class fixaudio : Form
    {
        //public int ReturnValue1 { get; set; }
        //private static Album currect_album { get { return Form1.currect_album; } }
        List<Album> newalbum = new List<Album>();
        List<Song> FixedListSongs;
        List<Song> toFixSongList;

        private Form1 form1;

        private List<string> ListOfAllSongs;

        private List<string> ListOfBrokenSongs;

        public void grabListOfAllSongs(List<Album> newalbum)
        {
            ListOfAllSongs = new List<string>();

            foreach (Album albbbbb in newalbum)
            {
                if (albbbbb.ID == 0)
                {
                    foreach (Song fsdfsdfs in albbbbb.Songs)
                    {
                        ListOfAllSongs.Add(fsdfsdfs.Author + " - " + fsdfsdfs.Name);
                    }
                    break;
                }
            }
        }

        public fixaudio(Form1 form1)
        {
            InitializeComponent();

            ListOfBrokenSongs = new List<string>();

            this.form1 = form1;
            form1.PopulateAlbum("http://vk.com/audio?act=load_audios_silent&al=1&please_dont_ddos=2&gid=0&id=");
            newalbum = form1.albums;
            grabListOfAllSongs(newalbum);
            form1.CleanBrokenLinks(newalbum);

            toFixSongList = new List<Song>();
            toFixSongList = form1.BrokenSongs;
            label2.Text = toFixSongList.Count.ToString();
            FixedListSongs = new List<Song>();

            aaaa = new WMPLib.WindowsMediaPlayer();

            for (int i = 0; i < toFixSongList.Count; i++)
            {
                Song newtestsong = new Song();
                newtestsong.FileNameForSave = "";
                newtestsong.Name = "";
                FixedListSongs.Add(newtestsong);
            }

                //int i = 0;
                //ListSongs = new List<Album>();

                foreach (Song toFixSong in toFixSongList)
                {
                    //populate with songs
                    dataGridView1.Rows.Add(toFixSong.DownloadURL.ToString(), toFixSong.Author + " - " + toFixSong.Name);
                    ListOfBrokenSongs.Add(toFixSong.Author + " - " + toFixSong.Name);

                    /*
                    Album bdbd = new Album();
                    bdbd = form1.FindSongs(toFixSong.Author + " " + toFixSong.Name);
                    string[] collection_to_add = new string[bdbd.Songs.Count + 1];
                    collection_to_add[0] = "";
                    int ii = 1;
                    foreach (Song songgg in bdbd.Songs)
                    {
                        collection_to_add[ii] = songgg.FileNameForSave;
                        ii++;
                    }
                    ListSongs.Add(bdbd);

                    i++;*/
                    //if (i == 2) { break; }
                }

        }

        private Album searchForThisSong(string name)
        {
            Album bdbd = new Album();
            bdbd = form1.FindSongs(name);
            return bdbd;
        }

        WMPLib.WindowsMediaPlayer aaaa;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int SelectedColumnIndex = e.ColumnIndex;
            int SelectedRowIndex = e.RowIndex;

            if (SelectedRowIndex >= 0)
            {
                textBox1.Text = toFixSongList[SelectedRowIndex].Author + " " + toFixSongList[SelectedRowIndex].Name;

                /*listBox1.Items.Clear();
                listBox1.Items.Add("");
                Album foundSongs = new Album();
                
                //foundSongs = searchForThisSong(dataGridView1.Rows[SelectedRowIndex].Cells[1].Value.ToString());
                foundSongs = searchForThisSong(toFixSongList[SelectedRowIndex].Author + " " + toFixSongList[SelectedRowIndex].Name);
                foreach (Song songgg in foundSongs.Songs)
                {
                    listBox1.Items.Add(songgg.FileNameForSave);
                }*/


                /*if (dataGridView1.Rows[SelectedRowIndex].Cells[2].Value != "")
                {

                }*/
            }

            if (SelectedColumnIndex == 3 && SelectedRowIndex >= 0)
            {
                aaaa.controls.stop();

                if (FixedListSongs[SelectedRowIndex].Name != "")
                {
                      //a.URL = "song.mp3";
                    aaaa.URL = FixedListSongs[SelectedRowIndex].DownloadURL;
                    aaaa.controls.play();
                }
                /*dataGridView1.BeginEdit(true);
                ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                int indd = comboBox.SelectedIndex;
                string urlll = ListSongs[SelectedRowIndex - 1].Songs[indd - 1].DownloadURL;
                
                MessageBox.Show(urlll);*/
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                return;
            }

            string selected_song_name = listBox1.SelectedItem.ToString();
            int selected_song_index = listBox1.SelectedIndex;

            dataGridView1.SelectedRows[0].Cells[2].Value = selected_song_name;

            if (selected_song_index == 0)
            {
                FixedListSongs[dataGridView1.SelectedRows[0].Index].DownloadURL = "";
                FixedListSongs[dataGridView1.SelectedRows[0].Index].Name = "";
                FixedListSongs[dataGridView1.SelectedRows[0].Index].oid = "";
                FixedListSongs[dataGridView1.SelectedRows[0].Index].aid = "";
                return;
            }
            else
            {
                selected_song_index--;
            }

            FixedListSongs[dataGridView1.SelectedRows[0].Index].DownloadURL = foundSongs.Songs[selected_song_index].DownloadURL;
            FixedListSongs[dataGridView1.SelectedRows[0].Index].Name = foundSongs.Songs[selected_song_index].FileNameForSave;
            FixedListSongs[dataGridView1.SelectedRows[0].Index].oid = foundSongs.Songs[selected_song_index].oid;
            FixedListSongs[dataGridView1.SelectedRows[0].Index].aid = foundSongs.Songs[selected_song_index].aid;
        }

        Album foundSongs;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "") { listBox1.Items.Clear(); }
            else
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("");
                foundSongs = new Album();
                foundSongs = searchForThisSong(textBox1.Text);
                foreach (Song songgg in foundSongs.Songs)
                {
                    listBox1.Items.Add(songgg.FileNameForSave);
                }
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            aaaa.controls.stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "List of broken songs (" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ")"; // Default file name
            saveFileDialog1.DefaultExt = ".txt"; // Default file extension
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog1.FileName, ListOfBrokenSongs.ToArray());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
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
        }

        List<int> edited_songs;

        public void button1_Click(object sender, EventArgs e)
        {
            // save list

            // add songs
            edited_songs = new List<int>();
            for (int i = FixedListSongs.Count - 1; i >= 0; i--)
            {
                Song fixedsong = FixedListSongs[i];
                if (fixedsong.Name != "")
                {
                    string url = ("http://vk.com/audio?act=add&aid=" + fixedsong.aid + "&al=1&search=1&oid=" + fixedsong.oid + "&top=0&hash=" + form1.add_hash);
                    VKUtils.PostRequest(url, form1.cook);
                    edited_songs.Add(i);
                }
            }

            // move songs
            edited_songs.Reverse();
            if (radioButton6.Checked == true)
            {
                form1.PopulateAlbum("http://vk.com/audio?act=load_audios_silent&al=1&please_dont_ddos=2&gid=0&id=");
                newalbum = form1.albums;

                Album myyyyyyyyy = new Album();
                foreach (Album albbbbb in newalbum)
                {
                    if (albbbbb.ID == 0)
                    {
                        myyyyyyyyy = albbbbb;
                        break;
                    }
                }

                for (int i = 0; i < edited_songs.Count; i++)
                {
                    string url = ("http://vk.com/audio?act=reorder_audios&aid=" + myyyyyyyyy.Songs[i].aid + "&al=1&before=" + toFixSongList[edited_songs[i]].aid + "&oid=" + myyyyyyyyy.Songs[i].oid + "&hash=" + form1.reorder_hash);
                    VKUtils.PostRequest(url, form1.cook);
                    //after:215303832
                }
            }

            // delete songs
            if (radioButton1.Checked == true) { }
            else if (radioButton3.Checked == true)
            {
                foreach (Song fsdfd in toFixSongList)
                {
                    string url = ("http://vk.com/audio?act=delete_audio&aid=" + fsdfd.aid + "&al=1&restore=1&oid=" + fsdfd.oid + "&hash=" + form1.delete_hash);
                    VKUtils.PostRequest(url, form1.cook);
                }
            }
            else if (radioButton4.Checked == true)
            {
                for (int i = 0; i < toFixSongList.Count; i++)
                {
                    if (edited_songs.Contains(i))
                    {
                        string url = ("http://vk.com/audio?act=delete_audio&aid=" + toFixSongList[i].aid + "&al=1&restore=1&oid=" + toFixSongList[i].oid + "&hash=" + form1.delete_hash);
                        VKUtils.PostRequest(url, form1.cook);
                    }
                }
            }

        }

    }
}
