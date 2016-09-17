using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using VKHotkeys.Data;

namespace VKHotkeys
{
    public partial class Song_Select : Form
    {
        //public Album newpopulate;

        public int ReturnValue1 { get; set; }

        private static Album currect_album { get { return Form1.currect_album; } }

        public Song_Select()
        {
            InitializeComponent();

            PopulateDGW();

            textBox1.Focus();
        }

        public void PopulateDGW()
        {
            dataGridView1.Rows.Clear();

            for (int i = 0; i < currect_album.Songs.Count; i++)
            {
                dataGridView1.Rows.Add(i,  currect_album.Songs[i].Author + " - " + currect_album.Songs[i].Name);
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string search = textBox1.Text;

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
                        dataGridView1.Rows.Add(i, currect_album.Songs[i].Author + " - " + currect_album.Songs[i].Name);
                    }
                }
            }

        }

        public bool ContainsNew(string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void vkb1_Click(object sender, EventArgs e)
        {
            this.ReturnValue1 = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            //this.ReturnValue1 = newpopulate[selected_song_id - 1][2];
            this.Close();
        }

        private void vkb2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
