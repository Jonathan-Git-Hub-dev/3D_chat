using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsApp1
{
    public partial class Startup : Form
    {
        int chosen_option = 0;
        int[] colour_choice = new int[] { 255, 0, 0 };
        Bitmap ccc = new Bitmap(48, 50);

        //other screens
        public static Menu menu;
        //public static Form1 main_screen; 


        public Startup()
        {
            InitializeComponent();


            //Clear_All_Options();

            Golden_Button.FlatAppearance.BorderSize = 5; // Sets the border thickness to 5 pixels

            Initate_Picture();

            Colour_Pic(colour_choice);

            Golden_Icon.Image = Image.FromFile(Globals.asset_folder_2d + "Golden.png");
            Machine_Icon.Image = Image.FromFile(Globals.asset_folder_2d + "Machine.png");
            Soon_Icon.Image = Image.FromFile(Globals.asset_folder_2d + "Soon.png");


            Initialize.Initialize_Menu(ref menu);
            Globals.main_screen = new Form1(ref menu);
        }

        private void Colour_Pic(int[] colour)
        {
            Color c = Color.FromArgb(colour[0], colour[1], colour[2]);
            for (int i = 0; i < pictureBox2.Width; i++)
            {
                for (int j = 0; j < pictureBox2.Height; j++)
                {
                    ccc.SetPixel(i, j, c);
                }
            }
            pictureBox2.Image = ccc;
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

        private void Initate_Picture()
        {
            //a numer devisble by 6 makes it easier
            int width = 600;
            int height = 50;

            pictureBox1.Width = width;
            pictureBox1.Height = height;

            Bitmap bm = new Bitmap(width, height);

            // background-image: linear-gradient(90deg, red, yellow, rgb(0,255,0), aqua, rgb(0,0,255), rgb(255,0,255), red);


            int gaps = 6;
            //int gap_size = 100;

            int aa = 100;
            for (int i = 0; i < width; i++)
            {


                int[] colour = Colour_By_ratio(i, width, gaps);

                for (int j = 0; j < height; j++)
                {
                    bm.SetPixel(i, j, Color.FromArgb(255, colour[0], colour[1], colour[2]));
                }

                aa += 50;
                aa %= 255;
            }
            pictureBox1.Image = bm;
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
            /*(bool status, int id, int new_port) data = await Connect();

            if (!data.status)
            {
                Connection_Status.Text = "Failed to connect, try again later";
                return;
            }*/


            //Form1 newForm = new Form1(data.id, data.new_port, chosen_option, colour_choice); // Create an instance of Form2
            //Form1 newForm = new Form1(0, 0, chosen_option, colour_choice);
            //newForm.Show();

            //save users data to globals
            //Globals.id = data.id;
            Globals.id = 0;
            //Globals.new_port = data.new_port;
            Globals.new_port = 0;
            Globals.chosen_option = chosen_option;
            //Globals.colour_choice = colour_choice;
            Globals.colour_choice = new Colour(colour_choice);


            Globals.main_screen.Show();
            menu.Show();
            menu.WindowState = FormWindowState.Minimized;//this becomes our aplications icon

            this.Hide();
            //while (true)
            //{
            //Trace.WriteLine("still kicking");
            //Thread.Sleep(500);
            //}
            //else dispaly fail message
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
            int x = e.X; // X-coordinate of the mouse click
            int y = e.Y; // Y-coordinate of the mouse click

            //may click on object but release mouse over another object stuff up indexing
            if (x < 0 || x >= 600 || y < 0 || y >= 50)
            {
                return;
            }

            // You can then use these coordinates, for example, to display them:
            //Trace.WriteLine($"Mouse clicked at X: {x}, Y: {y}");

            int[] colour = Colour_By_ratio(x, 600, 6);
            colour_choice[0] = colour[0];
            colour_choice[1] = colour[1];
            colour_choice[2] = colour[2];
            Colour_Pic(colour_choice);


        }

        private void Startup_Load(object sender, EventArgs e)
        {
            /*Form1 newForm = new Form1(0, 0, chosen_option, colour_choice);
            newForm.Show();

            this.Hide();*/
        }
    }
}
