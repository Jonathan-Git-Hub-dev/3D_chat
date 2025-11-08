namespace WinFormsApp1
{
    public class Shape
    {
        public Colour colour;
        public Face[] faces;//required
        public Point_3d[] internal_points;
        public Point_3d[] external_points;
        public Point_3d middle;
        public Point_3d origin;
        public int angle;

        //middle point exists at origin with all points relative

        public Shape(Face[] passed_faces, Colour c, Point_3d m, Point_3d o)
        {
            faces = passed_faces;
            colour = c;
            middle = m;
            origin = o;
            angle = 0;

            internal_points = null;
            external_points = null;

        }

        public void Set_Faces(Face[] passed_faces)
        {
            faces = passed_faces;
        }
        public void Set_Internal_Points(Point_3d[] passed_points)
        {
            internal_points = passed_points;
        }
        public void Set_External_Points(Point_3d[] passed_points)
        {
            external_points = passed_points;
        }

        public void Bounding_Box()
        {
            double max_x = faces[0].p1.x;
            double max_y = faces[0].p1.y;
            double min_x = faces[0].p1.x;
            double min_y = faces[0].p1.y;

            for (int i = 0; i < faces.Length; i++)
            {
                min_x = Utility.Min(min_x, faces[i].p1.x, faces[i].p2.x, faces[i].p3.x);
                min_y = Utility.Min(min_y, faces[i].p1.y, faces[i].p2.y, faces[i].p3.y);
                max_x = Utility.Max(max_x, faces[i].p1.x, faces[i].p2.x, faces[i].p3.x);
                max_y = Utility.Max(max_y, faces[i].p1.y, faces[i].p2.y, faces[i].p3.y);
                //Trace.WriteLine("min x");
            }
            double x = min_x + (max_x - min_x) / 2;
            double y = min_y + (max_y - min_y) / 2;
            //Trace.WriteLine("min x " + min_x.ToString() + " max " + max_x.ToString() + " x " + x);
            //Trace.WriteLine("min y " + min_y.ToString() + " max " + max_y.ToString() + " y " + y);

            //z is irelevant
            this.middle = new Point_3d(x, y, 0);
        }


    }
}