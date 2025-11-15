using System.Diagnostics;

namespace WinFormsApp1
{
    class Initialize
    {
        public static void Get_Screen_Size(Form1 screen)
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            int width = workingArea.Width - Globals.left_right_border;

            Globals.width = width;
            Globals.height = width / 2;

            //Rectangle screenRectangle = screen.RectangleToScreen(screen.ClientRectangle);

            //Globals.ribbon_height = screenRectangle.Top - screen.Top;
            Globals.ribbon_height = 34;

            Globals.x_default = workingArea.Width / 2;
            Globals.y_default = workingArea.Height / 2;

            Trace.WriteLine("new cent " + Globals.x_default + " " + Globals.y_default);
        }

        public static void Structure_Componenets(ref Bitmap bm, Form1 screen)
        {
            //set up bitmap for rendering
            bm = new Bitmap(Globals.width, Globals.height); // Create a new Bitmap
            screen.render_screen.Location = new Point(Globals.left_right_border / 2, Globals.top_border);
            screen.render_screen.Size = new Size(Globals.width, Globals.height);



            //move labels
            screen.coordinates_label.Location = new Point(Globals.left_right_border / 2, Globals.height + 4);
            screen.angle_label.Location = new Point(Globals.left_right_border / 2, Globals.height + 24);
            screen.fps_label.Location = new Point(Globals.width - 80, Globals.height + 4);

        }

        public static void Initialize_Menu(ref Menu menu)
        {
            menu = new Menu();
            /*Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;


            int width = workingArea.Width;
            int height = workingArea.Height;

            int menu_width = menu.Width;
            int menu_height = menu.Height;

           
            Globals.menu_left_distance = (width - menu_width)/2;
            Globals.menu_top_disatnce = (height - menu_height) / 2;*/
        }

        public static void Initialize_Screen()
        {
            Globals.main_screen = new Form1();
            Globals.menu = new Menu();
        }

        public static void Initial_Print(Form1 screen)
        {
            Render.Print_Coordinates(screen, new Point_3d(0, 0, 0));
            Render.Print_Angle(screen, 0, 0);
        }

        public static void Initiate_Users(ref Asset_Instance[] players)
        {
            for (int i = 0; i < Globals.max_users; i++)
            {
                players[i] = new Asset_Instance();
            }

        }
    }
}