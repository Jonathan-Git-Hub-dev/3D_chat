namespace WinFormsApp1
{
    partial class Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button_hide = new Button();
            button_quit = new Button();
            SuspendLayout();
            // 
            // button_hide
            // 
            button_hide.Location = new Point(12, 12);
            button_hide.Name = "button_hide";
            button_hide.Size = new Size(130, 128);
            button_hide.TabIndex = 0;
            button_hide.Text = "Close Menu";
            button_hide.UseVisualStyleBackColor = true;
            button_hide.Click += button1_Click;
            // 
            // button_quit
            // 
            button_quit.Location = new Point(484, 12);
            button_quit.Name = "button_quit";
            button_quit.Size = new Size(137, 128);
            button_quit.TabIndex = 1;
            button_quit.Text = "Quit";
            button_quit.UseVisualStyleBackColor = true;
            button_quit.Click += button2_Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(633, 347);
            Controls.Add(button_quit);
            Controls.Add(button_hide);
            Name = "Menu";
            Text = "Menu";
            ResumeLayout(false);
        }

        #endregion

        private Button button_hide;
        private Button button_quit;
    }
}