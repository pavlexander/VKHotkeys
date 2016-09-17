using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace VKButton
{
    public class VKButton : Button
    {
        public VKButton()
        {
            InitializeComponent();
        }

        protected void OnMouseEnter(object sender, EventArgs e)
        {
            this.button1.Image = global::VKbutton.Properties.Resources.hover;
        }

        protected void OnMouseUp(object sender, MouseEventArgs e)
        {
            this.button1.Image = global::VKbutton.Properties.Resources._default;
        }

        protected void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.button1.Image = global::VKbutton.Properties.Resources.click;
        }

        protected void OnMouseLeave(object sender, EventArgs e)
        {
            this.button1.Image = global::VKbutton.Properties.Resources._default;
        }
    }
}
