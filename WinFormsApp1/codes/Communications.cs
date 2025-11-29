namespace WinFormsApp1
{
    public class Communications
    {
        public static string Encode_User_statistic(Point_3d origin, int xy_angle, Colour colour, int asset_choice)
        {
            string message = origin.ToString();

            message += " " + xy_angle.ToString();
            message += " " + colour.ToString();
            message += " " + asset_choice;
            message += " " + Globals.counter;

            Globals.counter++;
            return message;
        }

        public static void Decode_S(string message, ref Asset_Instance[] players)
        {
            string[] instance_data = message.Split("$");

            for (int i = 0; i < Globals.max_users; i++)
            {
                string[] data = instance_data[i].Split(" ");

                Point_3d origin = Decode_Origin(data[0]);
                int angle = Decode_Angle(data[1]);
                Colour colour = Decode_Colour(data[2]);
                int asset = Decode_Asset(data[3]);
                bool online = Decode_Online(data[4]);


                //if (i == 3)
                //{
                //    players[i].Update(new Point_3d(-30, 0, -5), new Colour(255, 0, 0), angle, 0, true);
                //}
                //else
                //{
                    players[i].Update(origin, colour, angle, asset, online);
                //}

                //Console.WriteLine(players[i].online);
            }
        }//we recieved: 0.000000,0.000000,0.000000 0 153,255,0 0 1$-1.000000,-3.000000,1.000000 5000 200,200,10 1 1$-1.000000,-1.000000,-1.000000 -1 -1,-1,-1 -1 0$-1.000000,-1.000000,-1.000000 -1 -1,-1,-1 -1 0
        //point, angle, colour, asset, online

        public static int Decode_Angle(string data)
        {
            return int.Parse(data);
        }

        public static int Decode_Asset(string data)
        {
            return int.Parse(data);
        }
        public static bool Decode_Online(string data)
        {
            int online = int.Parse(data);

            if (online == 1)
            {
                return true;
            }
            return false;
        }

        public static Point_3d Decode_Origin(string data)
        {
            string[] numbers = data.Split(",");

            double x = double.Parse(numbers[0]);
            double y = double.Parse(numbers[1]);
            double z = double.Parse(numbers[2]);

            return new Point_3d(x, y, z);
        }

        public static Colour Decode_Colour(string data)
        {
            string[] numbers = data.Split(",");

            int red = int.Parse(numbers[0]);
            int green = int.Parse(numbers[1]);
            int blue = int.Parse(numbers[2]);

            return new Colour(red, green, blue);
        }
    }
}