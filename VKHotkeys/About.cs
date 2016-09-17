using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace VKHotkeys
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            LinkLabel.Link link1 = new LinkLabel.Link();
            link1.LinkData = "http://vkhotkeys.tumblr.com/";
            linkLabel1.Links.Add(link1);

            LinkLabel.Link link2 = new LinkLabel.Link();
            link2.LinkData = "http://vk.com/vkhotkeys";
            linkLabel2.Links.Add(link2);

            LinkLabel.Link link3 = new LinkLabel.Link();
            link3.LinkData = "http://vkhotkeys.tumblr.com/donate/";
            linkLabel3.Links.Add(link3);

            LinkLabel.Link link4 = new LinkLabel.Link();
            link4.LinkData = "http://vkhotkeys.tumblr.com/ask/";
            linkLabel4.Links.Add(link4);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}
