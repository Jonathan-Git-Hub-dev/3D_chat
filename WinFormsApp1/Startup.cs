using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NAudio.Wave;

namespace WinFormsApp1
{
    public partial class Startup : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        int chosen_option = 0;
        int[] colour_choice = new int[] { 255, 0, 0 };
        Bitmap ccc = new Bitmap(48, 50);

        public Startup()
        {
            InitializeComponent();

            Golden_Button.FlatAppearance.BorderSize = 5; // Sets the border thickness to 5 pixels
            Colour_Pic(colour_choice);

            Initialize.Initialize_Screen();

            //selecter size 9 wide 20 tall
            Initialize_Pointer();
            Move_Pointer(0);

            Globals.startup_screen = this;

            AllocConsole();
            //Console.WriteLine("hello thi sis our new console");

            /*Vector nv = Vector.Normal_Vector_Angle(8000, 0);
            Console.WriteLine(nv.delta_x + " " + nv.delta_y + " " + nv.delta_z);

            bool temp = Face.Infront(new Point_3d(0.8815380000000002, 0.646849, -3.075), new Point_3d(0,6,0), nv);
            Console.WriteLine(temp);*/
        }

        public void Initialize_Pointer()
        {
            Bitmap bitmap = new Bitmap(Globals.asset_folder_2d + "Pointer.png");

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (bitmap.GetPixel(x, y).R == 255)
                    {
                        ////bitmap.SetPixel()
                        bitmap.MakeTransparent(bitmap.GetPixel(x, y));
                    }
                }
            }

            pointer_pb.Image = bitmap;
        }

        public void Move_Pointer(int index)
        {
            int pointer_width = 9;
            int offset = (pointer_width - 1) / 2;
            int base_index = colour_options_pb.Location.X;

            pointer_pb.Location = new Point(base_index + index - offset, pointer_pb.Location.Y);
        }


        private void Colour_Pic(int[] colour)
        {
            Color c = Color.FromArgb(colour[0], colour[1], colour[2]);
            for (int i = 0; i < selected_colour_pb.Width; i++)
            {
                for (int j = 0; j < selected_colour_pb.Height; j++)
                {
                    ccc.SetPixel(i, j, c);
                }
            }
            selected_colour_pb.Image = ccc;
        }

        //string[] buttons_list = {"button2", "button2", ""};

        private int[] Colour_By_ratio(int i, int width, int gap)
        {

            int[,] colour_rgb_array = new int[,] { { 255, 0, 0 }, { 255, 255, 0 }, { 0, 255, 0 }, { 0, 255, 255 }, { 0, 0, 255 }, { 255, 0, 255 }, { 255, 0, 0 } };

            int gap_size = width / gap;
            int index = i / gap_size;
            int mix = i % gap_size;

            double mix_a = (100 - (double)mix) / 100;
            double mix_b = (double)mix / 100;

            double r = (double)colour_rgb_array[index, 0] * mix_a + (double)colour_rgb_array[index + 1, 0] * mix_b;
            double g = (double)colour_rgb_array[index, 1] * mix_a + (double)colour_rgb_array[index + 1, 1] * mix_b;
            double b = (double)colour_rgb_array[index, 2] * mix_a + (double)colour_rgb_array[index + 1, 2] * mix_b;

            return new int[] { (int)r, (int)g, (int)b };
        }




        private async Task<(bool, int, int)> Connect()
        {

            int port = 8080; // Example port
            int new_port = -1;
            int id = -1;

            try
            {
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(Globals.server_ip, port);
                byte[] buffer = new byte[1024]; // Create a buffer to store received data
                int bytesRead;
                Trace.WriteLine("conected");
                NetworkStream stream = client.GetStream();

                //send out stats
                //byte[] data = Encoding.ASCII.GetBytes(colour_choice.ToString() + " " + chosen_option);

                // Send the data to the server
                //stream.Write(data, 0, data.Length);


                if ((bytesRead = stream.Read(buffer, 0, buffer.Length)) < 0)
                {
                    Trace.WriteLine("no data");
                    return (false, -1, -1);
                }
                string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Trace.WriteLine($"Received: {receivedMessage}");
                string[] nums = receivedMessage.Split(',');
                id = int.Parse(nums[0]);
                new_port = int.Parse(nums[1]);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return (false, -1, -1);
            }

            return (true, id, new_port);
        }
        private async void Connect_Button_Click(object sender, EventArgs e)
        {
            //info about our instance
            (bool status, int id, int new_port) data = await Connect();

            if (!data.status)
            {
                Connection_Status.Text = "Failed to connect, try again later";
                return;
            }


            //save users data to globals
            Globals.id = data.id;
            //Globals.id = 0;
            Globals.new_port = data.new_port;
            //Globals.new_port = 0;
            Globals.chosen_option = chosen_option;
            //Globals.colour_choice = colour_choice;
            Globals.colour_choice = new Colour(colour_choice);


            //Globals.main_screen.Show();
            Globals.main_screen.Handle_Display();
            Globals.menu.Show();
            Globals.menu.WindowState = FormWindowState.Minimized;//this becomes our aplications icon

            Globals.main_screen.activate_background_workers();

            this.Hide();
        }

        private void Machine_Button_Click(object sender, EventArgs e)
        {
            Machine_Button.FlatAppearance.BorderSize = 5;
            Golden_Button.FlatAppearance.BorderSize = 1;

            chosen_option = 0;
        }

        private void Golden_Button_Click(object sender, EventArgs e)
        {
            Machine_Button.FlatAppearance.BorderSize = 1;
            Golden_Button.FlatAppearance.BorderSize = 5;

            chosen_option = 1;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.X >= 600 || e.Y < 0 || e.Y >= 50)
            {
                return;
            }
            //else position inside of bit map selected

            int[] colour = Colour_By_ratio(e.X, 600, 6);

            colour_choice[0] = colour[0];
            colour_choice[1] = colour[1];
            colour_choice[2] = colour[2];
            Colour_Pic(colour_choice);

            Move_Pointer(e.X);
        }

        private void Soon_Button_Click(object sender, EventArgs e)
        {
            //Globals.main_screen.deactivate_background_workers();
        }
    }
}
