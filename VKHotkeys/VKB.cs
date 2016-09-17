using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace VKHotkeys
{
    class VKB : Button
    {
        public VKB() : base()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            base.AutoSize = false;
            base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            base.BackColor = System.Drawing.Color.Transparent;
            base.FlatAppearance.BorderSize = 0;
            base.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            base.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            base.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            base.UseVisualStyleBackColor = false;
            base.UseCompatibleTextRendering = true;
            base.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            base.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            base.ForeColor = System.Drawing.Color.White;
            base.Image = global::VKHotkeys.Properties.Resources._default;
            base.Location = new System.Drawing.Point(0, 0);
            base.Name = "button1";
            base.Size = new System.Drawing.Size(129, 31);
            base.TabIndex = 0;
            base.TabStop = false;
            base.Text = "button1";
            base.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            base.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            base.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            base.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
        }

        protected void OnMouseEnter(object sender, EventArgs e)
        {
            base.Image = global::VKHotkeys.Properties.Resources.hover;
        }

        protected void OnMouseUp(object sender, MouseEventArgs e)
        {
            base.Image = global::VKHotkeys.Properties.Resources._default;
        }

        protected void OnMouseDown(object sender, MouseEventArgs e)
        {
            base.Image = global::VKHotkeys.Properties.Resources.click;
        }

        protected void OnMouseLeave(object sender, EventArgs e)
        {
            base.Image = global::VKHotkeys.Properties.Resources._default;
        }

    }
}
