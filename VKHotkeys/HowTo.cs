using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace VKHotkeys
{
    public partial class HowTo : Form
    {
        class CustomButton : System.Windows.Forms.Button
        {
            protected override bool ShowFocusCues
            {
                get
                {
                    return false;
                }
            }
        }

        public HowTo()
        {
            InitializeComponent();
            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"txt\instruc_rus.rtf";
            if (File.Exists(path))
            {
                richTextBox1.LoadFile(path, RichTextBoxStreamType.RichText);
            }
            else
            {
                richTextBox1.Text = "no file";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path = @"txt\instruc_eng.rtf";
            if (File.Exists(path))
            {
                richTextBox1.LoadFile(path, RichTextBoxStreamType.RichText);
            }
            else
            {
                richTextBox1.Text = "no file";
            }
        }
    }
}
