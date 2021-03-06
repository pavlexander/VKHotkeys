﻿namespace VKbutton
{
    class VKbutton : Button
    {
        private Button button1;

        [Description("Gets or sets whether or not the control is Super")]
        [Category("VKbutton")]
        [DefaultValue(false)]

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Image = global::VKbutton.Properties.Resources._default;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 33);
            this.button1.TabIndex = 0;
            this.button1.TabStop = false;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.button1.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.button1.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;

            this.ResumeLayout(false);
        }
    }
}
