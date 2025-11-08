namespace WinFormsApp1
{
    partial class Form1
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
            angle_label = new Label();
            render_screen = new PictureBox();
            coordinates_label = new Label();
            fps_label = new Label();
            render_worker = new System.ComponentModel.BackgroundWorker();
            communication_out_worker = new System.ComponentModel.BackgroundWorker();
            communication_in_worker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)render_screen).BeginInit();
            SuspendLayout();
            // 
            // angle_label
            // 
            angle_label.AutoSize = true;
            angle_label.ForeColor = Color.White;
            angle_label.Location = new Point(12, 950);
            angle_label.Name = "angle_label";
            angle_label.Size = new Size(88, 25);
            angle_label.TabIndex = 5;
            angle_label.Text = "X-Y:     Z: ";
            // 
            // render_screen
            // 
            render_screen.Location = new Point(12, 12);
            render_screen.Name = "render_screen";
            render_screen.Size = new Size(1172, 870);
            render_screen.TabIndex = 8;
            render_screen.TabStop = false;
            render_screen.MouseMove += pictureBox1_MouseMove;
            // 
            // coordinates_label
            // 
            coordinates_label.AutoSize = true;
            coordinates_label.ForeColor = Color.White;
            coordinates_label.Location = new Point(12, 920);
            coordinates_label.Name = "coordinates_label";
            coordinates_label.Size = new Size(175, 25);
            coordinates_label.TabIndex = 11;
            coordinates_label.Text = "Coordinates: [0, 0, 0]";
            // 
            // fps_label
            // 
            fps_label.AutoSize = true;
            fps_label.ForeColor = Color.White;
            fps_label.Location = new Point(1092, 932);
            fps_label.Name = "fps_label";
            fps_label.Size = new Size(60, 25);
            fps_label.TabIndex = 12;
            fps_label.Text = "FPS: 0";
            // 
            // render_worker
            // 
            render_worker.WorkerReportsProgress = true;
            render_worker.DoWork += render_worker_DoWork;
            render_worker.ProgressChanged += backgroundWorker1_ProgressChanged;
            // 
            // communication_out_worker
            // 
            communication_out_worker.DoWork += communiction_out_worker_DoWork;
            // 
            // communication_in_worker
            // 
            communication_in_worker.DoWork += communication_in_worker_DoWork;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1196, 988);
            Controls.Add(fps_label);
            Controls.Add(coordinates_label);
            Controls.Add(render_screen);
            Controls.Add(angle_label);
            Cursor = Cursors.No;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            Resize += Form1_Resize;
            ((System.ComponentModel.ISupportInitialize)render_screen).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public Label angle_label;
        public PictureBox render_screen;
        public Label coordinates_label;
        public Label fps_label;
        private System.ComponentModel.BackgroundWorker render_worker;
        private System.ComponentModel.BackgroundWorker communication_out_worker;
        private System.ComponentModel.BackgroundWorker communication_in_worker;
    }

}
