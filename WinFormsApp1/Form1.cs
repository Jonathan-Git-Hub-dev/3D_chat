using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
        public static int user_id = -1;
        public static int udp_port = 0;
        public static int asset_number = 0;
        public static Colour colour_c;

        public Form1(int id, int port, int asset_num, int[] colour_choice)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Icon = new Icon(Globals.asset_folder_2d + "main_icon.ico");
            Asset_Passer.Get_Assets(ref assets);
            this.KeyPreview = true; // Enable KeyPreview for the form
            Initialize.Initiate_Users(ref players);

            InitializeComponent();

            user_id = id;
            udp_port = port;
            asset_number = asset_num;
            colour_c = new Colour(colour_choice);



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

            this.Hide_Mouse();


            render_worker.RunWorkerAsync();
            //communication_out_worker.RunWorkerAsync();
            communication_in_worker.RunWorkerAsync();
        }

        private void communiction_out_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //set up udp

            UdpClient udpClient = new UdpClient();

            while (true)
            {
                //get our data use a mutex
                string message = Communications.Encode_User_statistic(origin, xy_angle, colour_c, asset_number);
                byte[] data = Encoding.UTF8.GetBytes(message);


                udpClient.Send(data, data.Length, Globals.server_ip, udp_port);
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
            Trace.WriteLine(udp_port + " " + remotePort);


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

                Communications.Decode_S(returnData, ref players);

                for(int i=0; i<Globals.max_users; i++)
                {
                    players[i].Print();
                    Trace.WriteLine("");
                }
            }

            udpClient.Close();

        }

        public void Hide_Mouse()//(Form1 screen)
        {
            Cursor.Position = new Point(Globals.width / 2 + Globals.left_right_border / 2, Globals.height / 2 + Globals.top_border);
            //Cursor.Hide();
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
                        if (rendered)
                        {
                            temp = true;
                        }
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

                start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                lock (render_lock)
                {
                    Render.Render_Assets(pass_players, pass_xy_angle, pass_z_angle, pass_origin, ref bm);
                    rendered = false;
                }
                stop = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long span = stop - start;

                long fps = 1000 / Render.Span_Ring_Array(ref span_array, span);

                //rendered = false;
                //Trace.WriteLine("start call");
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
                this.render_screen.Image = bm;
                rendered = true;
            }
            fps_label.Text = "FPS: ~" + e.ProgressPercentage.ToString();
        }





        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //MessageBox.Show("helo");
                var modalForm = new Menu();
                if (modalForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Process the result when the modal form is closed with OK
                }

            }

            /*if(e.KeyCode == Keys.G)
            {
                Trace.WriteLine("move done");
                Random random = new Random();
                int randomNumber = random.Next(200);
                Cursor.Position = new Point(100+randomNumber, 100+randomNumber);

                return;
            }*/

            lock (variable_lock)
            {
                Movement.Handle_Movement(ref origin, xy_angle, e);

                Render.Print_Coordinates(this, origin);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            //Trace.WriteLine("event generated");
            /*lock (variable_lock)
            {
                Movement.Handle_Mouse(e, ref xy_angle, ref z_angle, cage_mouse);
                Render.Print_Angle(this, xy_angle, z_angle);
                
                
            }*/
        }
    }
}

/*
//audio communications code
using (var waveIn = new WaveInEvent())
        {
            waveIn.DeviceNumber = 0;
            //(samples per second )
            waveIn.WaveFormat = new WaveFormat(4000,16, 1);
            waveIn.BufferMilliseconds = 100;//every half second
            //100
            //try some stuff
            //var waveOut = new WaveOutEvent();
            //var audioFileReader = new AudioFileReader(filePath);
            //waveOut.Init(e.Buffer);
            //waveOut.Play();
            WaveOutEvent _waveOut;
            BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
            bufferedWaveProvider.BufferLength = (int)(waveIn.WaveFormat.AverageBytesPerSecond * 0.5); // 2 seconds buffer
            bufferedWaveProvider.DiscardOnBufferOverflow = true; // Discard old data if buffer overflows

            // Initialize WaveOut (speaker output)
            _waveOut = new WaveOutEvent();
            _waveOut.Init(bufferedWaveProvider);

            // Start recording and playback
            //_waveIn.StartRecording();
            Byte[] last= new Byte[800];

            waveIn.DataAvailable += (sender, e) =>
            {
                Console.WriteLine(e.Buffer.Length.ToString() + " " + e.BytesRecorded.ToString());
                //maybe 2 bytes per sample

                //e.buffer
                //e.bytesrecorded
                //Console.WriteLine("got data");
                Byte[] temp = new Byte[800];
                Array.Copy(e.Buffer, temp, 800);
                //combine with last
                for (int i=0; i<last.Length; i+=2)
                {
                    //do for 2byes at a time

                    byte[] byteArray = { e.Buffer[i], e.Buffer[i+1] };
                    // Convert the byte array to a short using BitConverter.ToInt16
                    short resultShort = BitConverter.ToInt16(byteArray, 0);
                    resultShort /= 2;
                    //byte[] bytes = BitConverter.GetBytes(resultShort);
                    //e.Buffer[i] = bytes[0];
                    //e.Buffer[i+1] = bytes[1];

                    byte[] byteArray2 = { last[i], last[i + 1] };
                    // Convert the byte array to a short using BitConverter.ToInt16
                    short resultShort2 = BitConverter.ToInt16(byteArray2, 0);
                    resultShort2 /= 2;
                    resultShort += resultShort2;
                    byte[] bytes = BitConverter.GetBytes(resultShort);
                    e.Buffer[i] = bytes[0];
                    e.Buffer[i + 1] = bytes[1];


                    //e.Buffer[i] = (Byte)((int)e.Buffer[i] / 2);
                    //e.Buffer[i] = (Byte)(((int)e.Buffer[i]/2 + (int)last[i]/2));
                    //e.Buffer[i] = 0;// (Byte)(((int)e.Buffer[i] + (int)last[i]) / 2);
                }

                Array.Copy(temp, last, 1600);
                bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);



            };

            _waveOut.Play();
            waveIn.StartRecording();
            Console.ReadKey();
            waveIn.StopRecording();
        }



 
*/
