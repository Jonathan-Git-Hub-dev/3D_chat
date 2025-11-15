using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace WinFormsApp1
{
    public partial class Menu : Form
    {
        Form1 content;
        public static bool resize_called_on_initialization = true;
        public static bool first_display = true;
        public Menu()
        {
            InitializeComponent();

            //DesktopLocation = new Point(500, 500);

            //content = passed;
        }


        private void Button_Hide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Globals.showing_menu = false;
        }

        private void Button_Quit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void button_minimize_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.WindowState = FormWindowState.Minimized;
            Globals.main_screen.Hide();
        }

        private void Menu_Resize(object sender, EventArgs e)
        {
            if (resize_called_on_initialization)
            {
                resize_called_on_initialization = false;
                Trace.WriteLine("doin the fisrstsfd");
                return;
            }
            //when is this called
            //on creation  
            Trace.WriteLine("this has been resized (MENU)");
            if (this.WindowState == FormWindowState.Normal)
            {
                if (first_display)
                {
                    first_display = false;
                    return;
                }

                //esle this tab has been selected from task bar
                Globals.main_screen.Handle_Display();
            }


        }

        private void return_button_Click(object sender, EventArgs e)
        {
            //hide both screen
            Globals.main_screen.Hide();
            Globals.menu.Hide();
            Globals.main_screen.deactivate_background_workers();
            Globals.startup_screen.Show();
        }
    }
}
