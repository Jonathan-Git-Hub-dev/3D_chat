using NAudio.Wave;
using NAudio.Wave;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        public Form1()
        {

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Maximized;
            this.Icon = new Icon(Globals.asset_folder_2d + "main_icon.ico");
            Asset_Passer.Get_Assets(ref assets);
            this.KeyPreview = true; // Enable KeyPreview for the form
            Initialize.Initiate_Users(ref players);

            InitializeComponent();

            Initialize.Get_Screen_Size(this);
            Initialize.Structure_Componenets(ref bm, this);
            Initialize.Initial_Print(this);

            //players[3].online = true;


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

        Bitmap[] bm;

        bool cage_mouse = false;

        public void activate_background_workers()
        {
            render_worker.RunWorkerAsync();
            //communication_out_worker.RunWorkerAsync();
            if (Globals.new_port != 0)
            {
                communication_in_worker.RunWorkerAsync();
            }
        }

        public void deactivate_background_workers()
        {
            render_worker.CancelAsync();
            communication_in_worker.CancelAsync();
            //communication_out_worker.CancelAsync();
        }


        public void Hide_Mouse()//(Form1 screen)
        {
            Cursor.Position = new Point(Globals.x_default, Globals.y_default);
            //Cursor.Hide();
        }

        public void Handle_Display()
        {
            lock (render_lock)
            {//get mutex for bitmap
                this.Show();
            }
            Hide_Mouse();
        }


        byte[] audio_save = new byte[1200];
        bool audio_save_done = false;

        private async void communication_in_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            using (var waveIn = new NAudio.Wave.WaveInEvent())
            using (var _waveOut = new NAudio.Wave.WaveOutEvent())
            {
                //change to both in and out


                UdpClient udpClient = new UdpClient(11111);
                // make server aware of our udp port
                byte[] data2 = Encoding.UTF8.GetBytes("sending useles data for binding");
                udpClient.Send(data2, data2.Length, Globals.server_ip, Globals.new_port);







                waveIn.DeviceNumber = 0;
                waveIn.WaveFormat = new NAudio.Wave.WaveFormat(Globals.samples_per_second, Globals.sample_in_bytes * 8, 1);
                waveIn.BufferMilliseconds = Globals.sample_milliseconds;

                var bufferedWaveProvider = new NAudio.Wave.BufferedWaveProvider(waveIn.WaveFormat);
                bufferedWaveProvider.BufferLength = 1200 * 5; // Total buffer size
                bufferedWaveProvider.DiscardOnBufferOverflow = true;
                _waveOut.Init(bufferedWaveProvider);




                //we send the first udp message then active
                //communication_out_worker.RunWorkerAsync();

                long last_publish = 0;


                //set up sending port
                UdpClient udpClientOut = new UdpClient();
                //IPEndPoint localEndPoint = (IPEndPoint)udpClientOut.Client.LocalEndPoint;



                //start audio
                _waveOut.Play();
                waveIn.StartRecording();

                //send audio

                waveIn.DataAvailable += (sender, e) =>
                {
                    byte[] f_data = [.. e.Buffer, .. Globals.audio_data_char];

                    udpClientOut.Send(f_data, f_data.Length, Globals.server_ip, Globals.new_port);
                };

                while (!worker.CancellationPending)
                {
                    //When sever has sent data and dispose of it
                    if (udpClient.Client.Poll(1000, SelectMode.SelectRead))
                    {
                        UdpReceiveResult result = await udpClient.ReceiveAsync();

                        Console.WriteLine("recieved: " + Encoding.ASCII.GetString(result.Buffer));

                        byte[] receiveBytes = result.Buffer.Take(result.Buffer.Length - 1).ToArray();

                        if (result.Buffer[result.Buffer.Length - 1] == Globals.spacial_data_char[0])
                        {//space
                            string returnData = Encoding.ASCII.GetString(receiveBytes);

                            lock (variable_lock)
                            {
                                Communications.Decode_S(returnData, ref players);
                            }
                        }
                        else
                        {//audio
                            bufferedWaveProvider.AddSamples(receiveBytes, 0, 1200);
                        }
                    }

                    //publish to server
                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    if (now - last_publish > 1000)//do every second
                    {
                        last_publish = now;

                        //get our data use a mutex
                        string message2 = Communications.Encode_User_statistic(origin, xy_angle, Globals.colour_choice, Globals.chosen_option);


                        byte[] data3 = Encoding.UTF8.GetBytes(message2);

                        byte[] f_data = [.. data3, .. Globals.spacial_data_char];

                        udpClientOut.Send(f_data, f_data.Length, Globals.server_ip, Globals.new_port);
                    }

                }

                //stop audio
                _waveOut.Stop();
                waveIn.StopRecording();
                e.Cancel = true; // Indicate that the operation was cancelled

                return; // Exit the DoWork method
            }
            //udpClient.Close();

        }

        public enum Render_State
        {
            Free,
            Used
        }

        Render_State[] render_states = { Render_State.Used, Render_State.Free };


        //render to oppisite screen
        //send signal till switch
        //start rendering when switched

        //render_index
        //print_index



        private void render_worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            int render_index = 1;

            //return;
            DateTimeOffset now;
            long start, stop;

            long[] span_array = Render.Initialize_Span_Ring_Array();

            int pass_xy_angle;
            int pass_z_angle;
            Point_3d pass_origin;


            //int render_to = 1;


            Asset_Instance[] pass_players = new Asset_Instance[Globals.max_users];
            for (int i = 0; i < Globals.max_users; i++)
            {
                pass_players[i] = new Asset_Instance();
            }

            while (!worker.CancellationPending)//true
            {

                //Console.WriteLine("started render sequence");
                //Console.WriteLine("started render sequence " + render_index);
                while (true)
                {
                    lock (render_lock)
                    {

                        //Trace.WriteLine("1 rell");
                        if (render_states[render_index] != Render_State.Used)
                        {
                            break;
                        }
                    }

                    Thread.Sleep(100);//sleep so bit map can be used
                    //not a great system
                }

                //Console.WriteLine("got data for render sequence");

                //Trace.WriteLine("watiig for v l");
                lock (variable_lock)
                {
                    pass_xy_angle = xy_angle;
                    pass_z_angle = z_angle;
                    pass_origin = origin.Copy();

                    //players[0].angle += 200;
                    //players[0].angle %= 35999;

                    for (int i = 0; i < Globals.max_users; i++)
                    {
                        pass_players[i].Copy(players[i]);
                    }
                }
                //Trace.WriteLine("got v l");

                start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                //Console.WriteLine("got to before the render function");

                Render.Render_Assets(pass_players, pass_xy_angle, pass_z_angle, pass_origin, ref bm[render_index]);

                //Console.WriteLine("ended render sequence");

                stop = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long span = stop - start;

                long fps = 1000 / Render.Span_Ring_Array(ref span_array, span);



                DateTime currentTime = DateTime.Now;
                //Trace.WriteLine($"Current time: {currentTime.ToString("HH:mm:ss")}");
                render_worker.ReportProgress((int)span);

                lock (render_lock)
                {
                    render_states[render_index] = Render_State.Used;
                }

                render_index++;
                render_index %= 2;





            }
            e.Cancel = true; // Indicate that the operation was cancelled
            //Trace.WriteLine("render workder done " + DateTime.Now.ToLongTimeString());

            return; // Exit the DoWork method
        }

        //this should be reset every time this screen is called by startup
        int print_index = 1;
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Render_State[] render_states = { Render_State.Used, Render_State.Free };

            //Trace.WriteLine("3 acc");
            render_screen.Image = bm[print_index];
            rendered = true;

            print_index++;
            print_index %= 2;


            //unleash other screen
            lock (render_lock)
            {
                render_states[print_index] = Render_State.Free;
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
                Globals.menu.WindowState = FormWindowState.Normal;
                Globals.showing_menu = true;

            }

            if (Globals.showing_menu)//only take input when not showing menu
            {
                return;
            }



            lock (variable_lock)
            {
                Movement.Handle_Movement(ref origin, xy_angle, e);
                Render.Print_Coordinates(this, origin);
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

        private void communication_audio_worker_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}

