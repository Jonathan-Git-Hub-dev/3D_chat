namespace WinFormsApp1
{
    public class Colour
    {
        public int red;
        public int green;
        public int blue;
        public Colour(int r, int g, int b)
        {
            red = r;
            green = g;
            blue = b;
        }

        public Colour(int[] arr)
        {
            red = arr[0];
            green = arr[1];
            blue = arr[2];
        }


        public static Colour Colour_By_Anlge(Face face, Point_3d outside, Colour colour)
        {
            //new algo
            Point_3d mid = Face.Middle(face);
            Vector nv = Vector.Vector_Between_Points(mid, outside);

            Point_3d besty = Point_3d.Point_Plus_Vector(mid, Globals.lightest_vector);

            double dist = Math.Sqrt(Point_3d.Unsquared_Distance(outside, besty));

            double percent = (2 - dist) / 2;

            if(percent < 0)
            {
                Console.WriteLine("error here " + percent);
            }

            double percent2 = 0.9 * percent + 0.1;

            double red = colour.red * percent2;
            double green = colour.green * percent2;
            double blue = colour.blue * percent2;


            return new Colour((int)red, (int)green, (int)blue);
        }

        public override string ToString()
        {
            string message = red.ToString();
            message += "," + green.ToString();
            message += "," + blue.ToString();

            return message;
        }
    }
}