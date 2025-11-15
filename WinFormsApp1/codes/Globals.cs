namespace WinFormsApp1
{
    public static class Globals
    {
        //Movement Globals
        //bondries for user movement
        public static int x_negative_boarder = -50;
        public static int x_positive_boarder = 50;
        public static int y_negative_boarder = -50;
        public static int y_positive_boarder = 50;
        public static int z_negative_boarder = -20;
        public static int z_positive_boarder = 20;

        //Amount of movement per input
        public static int scaling = 100;
        public static double step_size = 0.25;

        //user viewing angles
        public static int minimum_xy_angle = 0;
        public static int maximum_xy_angle = 35999;
        public static int minimum_z_angle = -8000;
        public static int maximum_z_angle = 8000;
        /*
            xy  0        means user is looking down -y 
            xy  9000     means user is looking down +x
            xy  18000    means user is looking down +y
            xy  27000    means user is looking down -x
        
            z  -8000     means user is looking in the -z derection at an 80 degree angle (up)
            z  0         means user is looking flat with the xy axes
            z  8000      means user is looking in the +z derection at an 80 degree angle (down)
        */

        //Assets globals
        public static string asset_folder = @"..\..\..\codes\Assets\";
        public static string asset_folder_3d = asset_folder + @"3d\";
        public static string asset_folder_2d = asset_folder + @"2d\";

        public static string[] asset_options = { "golden_asset.txt", "machine_asset.txt" };
        //public static string[] asset_options = { "cube.txt", "machine_asset.txt" };

        public static int max_users = 4;

        //colour
        //public static Vector brightest = new Vector(0.58, 0.58, -0.58);


        public static char comment = '#';

        public static Vector lightest_vector = new(-0.5, 0.3, -0.81);

        public static int temp = 11;


        //rendering area
        public static int left_right_border = 60;
        public static int top_border = 5;
        public static int ribbon_height;
        public static int height;
        public static int width;
        public static int menu_left_distance;
        public static int menu_top_disatnce;
        public static int x_default;// = Globals.width / 2 + Globals.left_right_border / 2;
        public static int y_default;// = Globals.height / 2 + Globals.top_border;



        public static string server_ip = "3.25.130.4"; // Example: localhost


        //user data recieved
        public static int id;
        public static int new_port;
        public static int chosen_option;
        //public static int[] colour_choice = { 255, 0, 0};
        public static Colour colour_choice;




        public static Form1 main_screen;
        public static Menu menu;
        public static Startup startup_screen;

        public static bool showing_menu = false;
    }
}
