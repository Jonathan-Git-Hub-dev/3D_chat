using System.Diagnostics;

namespace WinFormsApp1
{
    class Screen_Point
    {
        public double x;
        public double y;

        public Screen_Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Screen_Point()
        {
            x = -1;
            y = -1;
        }


        public void Translate_2d(Point_3d origin, Point_3d point, int xy_angle, Vector normal_vector, Vector horizontal_vector, Vector vertical_vector)
        {
            double xy = xy_angle / 100;

            Point_3d centre_point = Point_3d.Find_Centre(origin, point, normal_vector);

            //Vector horizontal_vector = new Vector(Utility.Cos(xy), Utility.Sin(xy), 0);
            //Vector vertical_vector = Vector.Cross_Product(normal_vector, horizontal_vector);

            double j = (centre_point.z - point.z) / vertical_vector.delta_z;
            double x = point.x + j * vertical_vector.delta_x;
            double y = point.y + j * vertical_vector.delta_y;

            double distance_h = Math.Sqrt(Math.Pow(x - centre_point.x, 2) + Math.Pow(y - centre_point.y, 2));
            double distance_v = Math.Sqrt(
            Math.Pow(point.x - x, 2) +
            Math.Pow(point.y - y, 2) +
            Math.Pow(point.z - centre_point.z, 2)
            );


            double xstep = -1 * Utility.Cos(xy);
            double ystep = -1 * Utility.Sin(xy);
            double total = (point.x - centre_point.x) * xstep + (point.y - centre_point.y) * ystep;

            if (total > 0)
                distance_h *= -1;
            if (point.z < centre_point.z)
                distance_v *= -1;

            double distance = Math.Sqrt(Point_3d.Unsquared_Distance(origin, centre_point));

            if (distance == 0)
                Trace.WriteLine("this is not good\n");
            double max_w = distance * 2 / 2;//we need to programaticaaly do this
            double max_h = distance / 2;

            //need screen size
            /*return new Screen_Point(
                Globals.width/2 + Math.Floor(distance_h/max_w * Globals.width/2),
                Globals.height/2 + Math.Floor(distance_v/max_h * Globals.height/2)
            );*/
            this.x = Globals.width / 2 + Math.Floor(distance_h / max_w * Globals.width / 2);
            this.y = Globals.height / 2 + Math.Floor(distance_v / max_h * Globals.height / 2);
        }

        public void Extrapolate(Screen_Point reference)
        {
            double aproximate_max = 9000000000000;

            //find gradients
            int run = (int)(reference.x - x);
            int rise = (int)(reference.y - y);

            if (run == 0 && rise == 0)
            {
                //I dont think this should ever happen but idont know
                return;
            }

            if (run == 0)
            {
                //int intercept = reference.y;
                if (rise < 0)
                {
                    y = -1 * aproximate_max;
                }
                else
                {
                    y = aproximate_max;
                }
            }
            else if (rise == 0)
            {
                //int intercept = reference.y;
                if (run < 0)
                {
                    x = -1 * aproximate_max;
                }
                else
                {
                    x = aproximate_max;
                }
            }
            else
            {
                double intercept = reference.y - (run / rise) * reference.x;
                if (rise > run)
                {//rise domincant
                    if (rise < 0)
                    {
                        y = -1 * aproximate_max;
                    }
                    else
                    {
                        y = aproximate_max;
                    }
                    //find x
                    x = (y - intercept) / (rise / run);
                }
                else
                {//run doninant
                    if (run < 0)
                    {
                        x = -1 * aproximate_max;
                    }
                    else
                    {
                        x = aproximate_max;
                    }
                    //find y
                    y = (rise / run) * x + intercept;
                }
            }
        }


    }
}