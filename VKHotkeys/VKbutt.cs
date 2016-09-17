using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VKHotkeys
{
    public partial class VKbutt : UserControl
    {
        public VKbutt()
        {
            InitializeComponent();
        }

        protected void OnMouseEnter(object sender, EventArgs e)
        {
            this.button1.Image = global::VKHotkeys.Properties.Resources.hover;
        }

        protected void OnMouseUp(object sender, MouseEventArgs e)
        {
            this.button1.Image = global::VKHotkeys.Properties.Resources._default;
        }

        protected void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.button1.Image = global::VKHotkeys.Properties.Resources.click;
        }

        protected void OnMouseLeave(object sender, EventArgs e)
        {
            this.button1.Image = global::VKHotkeys.Properties.Resources._default;
        }

    }
}
