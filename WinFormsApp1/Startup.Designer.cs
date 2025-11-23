using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace WinFormsApp1
{
    
        partial class Startup
        {
            /// <summary>
            ///  Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Startup));
            Connect_Button = new Button();
            Golden_Button = new Button();
            Soon_Button = new Button();
            Machine_Button = new Button();
            colour_options_pb = new PictureBox();
            selected_colour_pb = new PictureBox();
            Golden_Icon = new PictureBox();
            label1 = new Label();
            Machine_Icon = new PictureBox();
            Soon_Icon = new PictureBox();
            label2 = new Label();
            Connection_Status = new Label();
            pointer_pb = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)colour_options_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)selected_colour_pb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Golden_Icon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Machine_Icon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Soon_Icon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pointer_pb).BeginInit();
            SuspendLayout();
            // 
            // Connect_Button
            // 
            Connect_Button.Location = new Point(17, 379);
            Connect_Button.Name = "Connect_Button";
            Connect_Button.Size = new Size(104, 33);
            Connect_Button.TabIndex = 0;
            Connect_Button.Text = "Connect";
            Connect_Button.UseVisualStyleBackColor = true;
            Connect_Button.Click += Connect_Button_Click;
            // 
            // Golden_Button
            // 
            Golden_Button.FlatStyle = FlatStyle.Flat;
            Golden_Button.Location = new Point(203, 9);
            Golden_Button.Name = "Golden_Button";
            Golden_Button.Size = new Size(200, 55);
            Golden_Button.TabIndex = 1;
            Golden_Button.Text = "Golden";
            Golden_Button.UseVisualStyleBackColor = true;
            Golden_Button.Click += Golden_Button_Click;
            // 
            // Soon_Button
            // 
            Soon_Button.FlatStyle = FlatStyle.Flat;
            Soon_Button.Location = new Point(657, 9);
            Soon_Button.Name = "Soon_Button";
            Soon_Button.Size = new Size(200, 55);
            Soon_Button.TabIndex = 2;
            Soon_Button.Text = "Cooming soon";
            Soon_Button.UseVisualStyleBackColor = true;
            Soon_Button.Click += Soon_Button_Click;
            // 
            // Machine_Button
            // 
            Machine_Button.FlatStyle = FlatStyle.Flat;
            Machine_Button.Location = new Point(430, 9);
            Machine_Button.Name = "Machine_Button";
            Machine_Button.Size = new Size(200, 55);
            Machine_Button.TabIndex = 3;
            Machine_Button.Text = "Machine";
            Machine_Button.UseVisualStyleBackColor = true;
            Machine_Button.Click += Machine_Button_Click;
            // 
            // colour_options_pb
            // 
            colour_options_pb.Image = Properties.Resources.colour_spread;
            colour_options_pb.Location = new Point(257, 281);
            colour_options_pb.Name = "colour_options_pb";
            colour_options_pb.Size = new Size(600, 25);
            colour_options_pb.TabIndex = 4;
            colour_options_pb.TabStop = false;
            colour_options_pb.MouseUp += pictureBox1_MouseUp;
            // 
            // selected_colour_pb
            // 
            selected_colour_pb.Location = new Point(203, 281);
            selected_colour_pb.Name = "selected_colour_pb";
            selected_colour_pb.Size = new Size(48, 50);
            selected_colour_pb.TabIndex = 5;
            selected_colour_pb.TabStop = false;
            // 
            // Golden_Icon
            // 
            Golden_Icon.Image = Properties.Resources.Golden;
            Golden_Icon.Location = new Point(203, 70);
            Golden_Icon.Name = "Golden_Icon";
            Golden_Icon.Size = new Size(200, 200);
            Golden_Icon.TabIndex = 8;
            Golden_Icon.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 24);
            label1.Name = "label1";
            label1.Size = new Size(169, 25);
            label1.TabIndex = 9;
            label1.Text = "Choose a Character:";
            // 
            // Machine_Icon
            // 
            Machine_Icon.Image = Properties.Resources.Machine;
            Machine_Icon.Location = new Point(430, 70);
            Machine_Icon.Name = "Machine_Icon";
            Machine_Icon.Size = new Size(200, 200);
            Machine_Icon.TabIndex = 10;
            Machine_Icon.TabStop = false;
            // 
            // Soon_Icon
            // 
            Soon_Icon.Image = Properties.Resources.Soon;
            Soon_Icon.Location = new Point(657, 70);
            Soon_Icon.Name = "Soon_Icon";
            Soon_Icon.Size = new Size(200, 200);
            Soon_Icon.TabIndex = 11;
            Soon_Icon.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 293);
            label2.Name = "label2";
            label2.Size = new Size(148, 25);
            label2.TabIndex = 12;
            label2.Text = "Choose a Colour:";
            // 
            // Connection_Status
            // 
            Connection_Status.AutoSize = true;
            Connection_Status.Location = new Point(142, 383);
            Connection_Status.Name = "Connection_Status";
            Connection_Status.Size = new Size(0, 25);
            Connection_Status.TabIndex = 13;
            // 
            // pointer_pb
            // 
            pointer_pb.Location = new Point(257, 311);
            pointer_pb.Name = "pointer_pb";
            pointer_pb.Size = new Size(9, 20);
            pointer_pb.TabIndex = 14;
            pointer_pb.TabStop = false;
            // 
            // Startup
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(892, 437);
            Controls.Add(pointer_pb);
            Controls.Add(Connection_Status);
            Controls.Add(label2);
            Controls.Add(Soon_Icon);
            Controls.Add(Machine_Icon);
            Controls.Add(label1);
            Controls.Add(Golden_Icon);
            Controls.Add(selected_colour_pb);
            Controls.Add(colour_options_pb);
            Controls.Add(Machine_Button);
            Controls.Add(Soon_Button);
            Controls.Add(Golden_Button);
            Controls.Add(Connect_Button);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Startup";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "3D Chat Laucher";
            ((System.ComponentModel.ISupportInitialize)colour_options_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)selected_colour_pb).EndInit();
            ((System.ComponentModel.ISupportInitialize)Golden_Icon).EndInit();
            ((System.ComponentModel.ISupportInitialize)Machine_Icon).EndInit();
            ((System.ComponentModel.ISupportInitialize)Soon_Icon).EndInit();
            ((System.ComponentModel.ISupportInitialize)pointer_pb).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Connect_Button;
            private Button Golden_Button;
            private Button Soon_Button;
            private Button Machine_Button;
            private PictureBox colour_options_pb;
            private PictureBox selected_colour_pb;
            private PictureBox Golden_Icon;
            private Label label1;
            private PictureBox Machine_Icon;
            private PictureBox Soon_Icon;
            private Label label2;
            private Label Connection_Status;
        private PictureBox pointer_pb;
    }
    

}