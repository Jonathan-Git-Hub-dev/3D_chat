using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;


//probaly should sclae normal vector in th function itself
namespace WinFormsApp1
{
    /*
     things to do
     way less magic numbers
     probably too many file
     */
    public partial class Form1 : Form
    {
        public static Asset[] assets = new Asset[Globals.asset_options.Length];
        public static Asset_Instance[] players = new Asset_Instance[Globals.max_users];
     
        Menu modalForm;// = new Menu();

        //public Form1(int id, int port, int asset_num, int[] colour_choice)
        public Form1(ref Menu menu)
        {

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Maximized;
            this.Icon = new Icon(Globals.asset_folder_2d + "main_icon.ico");
            Asset_Passer.Get_Assets(ref assets);
            this.KeyPreview = true; // Enable KeyPreview for the form
            Initialize.Initiate_Users(ref players);

            InitializeComponent();

            players[3].online = true;


            modalForm = menu;
            /*user_id = id;
            udp_port = port;
            asset_number = asset_num;
            colour_c = new Colour(colour_choice);*/

            //var modalForm = new Menu();

            //this.Hide_Mouse();

        }

        private static readonly object variable_lock = new object();
        private static readonly object render_lock = new object();
        bool rendered = true;


        //used to derive angles
        //100 = 1 degree
        //max of 35999 after which it raps around
        int xy_angle = 0;
        int z_angle = 0;

        Point_3d origin = new Point_3d(0, 6, 0);

        Bitmap bm;

        bool cage_mouse = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize.Get_Screen_Size(this);
            Initialize.Structure_Componenets(ref bm, this);
            Initialize.Initial_Print(this);
            //Initialize.Initialize_Menu(ref modalForm);

            //this.Hide_Mouse();


            render_worker.RunWorkerAsync();
            //communication_out_worker.RunWorkerAsync();
            if (Globals.new_port != 0)
            {
                communication_in_worker.RunWorkerAsync();
            }
        }

        private void communiction_out_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //set up udp

            UdpClient udpClient = new UdpClient();

            while (true)
            {
                //get our data use a mutex
                string message = Communications.Encode_User_statistic(origin, xy_angle, Globals.colour_choice, Globals.chosen_option);
                byte[] data = Encoding.UTF8.GetBytes(message);


                udpClient.Send(data, data.Length, Globals.server_ip, Globals.new_port);
                Thread.Sleep(1000);
            }
        }

        private async void communication_in_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            UdpClient udpClient = new UdpClient();

            // Prepare the data to send
            string message = "sending useles data for binding";
            byte[] data2 = Encoding.UTF8.GetBytes(message);

            // Specify the remote endpoint (IP address and port)
            int remotePort = 8081;
            //Trace.WriteLine(udp_port + " " + remotePort);


            // Send the data
            udpClient.Send(data2, data2.Length, Globals.server_ip, remotePort);


            //we send the first udp message then active
            communication_out_worker.RunWorkerAsync();


            //Trace.WriteLine($"UDP message sent to {Globals.server_ip}:{remotePort}");
            while (true)
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                byte[] receiveBytes = result.Buffer;
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                //Trace.WriteLine("we recieved: " + returnData);

                lock (variable_lock)
                {
                    //Communications.Decode_S(returnData, ref players);
                }

                /*for(int i=0; i<Globals.max_users; i++)
                {
                    players[i].Print();
                    Trace.WriteLine("");
                }*/
            }

            udpClient.Close();

        }

        public void Hide_Mouse()//(Form1 screen)
        {
            Cursor.Position = new Point(Globals.width / 2 + Globals.left_right_border / 2, Globals.height / 2 + Globals.top_border);
            Cursor.Hide();
        }

        private void render_worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)//, Bitmap bm)
        {
            //return;
            DateTimeOffset now;
            long start, stop;

            long[] span_array = Render.Initialize_Span_Ring_Array();


            //Thread.Sleep(2000);

            int pass_xy_angle;
            int pass_z_angle;
            Point_3d pass_origin;

            Asset_Instance[] pass_players = new Asset_Instance[Globals.max_users];
            for (int i = 0; i < Globals.max_users; i++)
            {
                pass_players[i] = new Asset_Instance();
            }


            while (true)//true
            {


                while (true)
                {
                    bool temp = false;
                    lock (render_lock)
                    {
                        //Trace.WriteLine("1 acc");
                        if (rendered)
                        {
                            temp = true;
                        }
                        //Trace.WriteLine("1 rell");
                    }

                    if (temp)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);//sleep so bit map can be used
                    }
                }

                //Trace.WriteLine("watiig for v l");
                lock (variable_lock)
                {
                    pass_xy_angle = xy_angle;
                    pass_z_angle = z_angle;
                    pass_origin = origin.Copy();

                    players[0].angle += 200;
                    players[0].angle %= 35999;

                    for (int i = 0; i < Globals.max_users; i++)
                    {
                        pass_players[i].Copy(players[i]);
                    }
                }
                //Trace.WriteLine("got v l");

                start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                //Trace.WriteLine("w rl");
                lock (render_lock)
                {
                    //Trace.WriteLine("2 acc");
                    Render.Render_Assets(pass_players, pass_xy_angle, pass_z_angle, pass_origin, ref bm);
                    rendered = false;
                    //Trace.WriteLine("2 rell");
                }
                //Trace.WriteLine("g wl");
                stop = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long span = stop - start;

                long fps = 1000 / Render.Span_Ring_Array(ref span_array, span);

                //rendered = false;
                //Trace.WriteLine("start call");

                DateTime currentTime = DateTime.Now;
                //Trace.WriteLine($"Current time: {currentTime.ToString("HH:mm:ss")}");
                render_worker.ReportProgress((int)span);
                //Trace.WriteLine("end call");
                //Monitor.Wait(render_lock);

                //Monitor.Exit(render_lock)

            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Trace.WriteLine("start print");
            lock (render_lock)
            {
                //Trace.WriteLine("3 acc");
                this.render_screen.Image = bm;
                rendered = true;
                //Trace.WriteLine("3 rel");
            }
            fps_label.Text = "FPS: ~" + e.ProgressPercentage.ToString();
        }





        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                /*//display mouse
                Cursor.Show();
                DialogResult d = modalForm.ShowDialog(this);
                if (d == DialogResult.OK)
                //if (modalForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Process the result when the modal form is closed with OK
                    Trace.WriteLine("done good");
                    //Trace.WriteLine(modalForm.Location.ToString() + " actual current loc");

                    //re hide
                    Hide_Mouse();
                }
                else if (d == DialogResult.No)
                {
                    Trace.WriteLine("minmize");

                }
                else
                {
                    Trace.WriteLine("done bad");
                }*/
                modalForm.WindowState = FormWindowState.Normal;
                Globals.showing_menu = true;

            }

            if(Globals.showing_menu)//only take input when not showing menu
            {
                return;
            }

            

            lock (variable_lock)
            {
                Movement.Handle_Movement(ref origin, xy_angle, e);
                //Render.Print_Coordinates(this, origin);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Globals.showing_menu)//only take input when not showing menu
            {
                return;
            }

            //Trace.WriteLine("event generated");
            lock (variable_lock)
            {
                Movement.Handle_Mouse(e, ref xy_angle, ref z_angle, cage_mouse);
                Render.Print_Angle(this, xy_angle, z_angle);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Trace.WriteLine("this has been resized");
        }
    }
}

