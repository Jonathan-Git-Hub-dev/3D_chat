using System.Diagnostics;
using System.Text;

namespace WinFormsApp1
{
    public class Point_3d
    {
        public double x;
        public double y;
        public double z;

        public Point_3d(double initail_x, double initial_y, double initial_z)
        {
            x = initail_x;
            y = initial_y;
            z = initial_z;
        }

        public void Set_Point_3d(Point_3d passed_point)
        {
            x = passed_point.x;
            y = passed_point.y;
            z = passed_point.z;
        }

        public Point_3d Copy()
        {
            return new Point_3d(x, y, z);
        }

        public override string ToString()
        {
            StringBuilder string_builder = new StringBuilder();

            string_builder.AppendFormat("{0},{1},{2}", x, y, z);

            return string_builder.ToString();
        }
        public void Print_Point()
        {
            Trace.WriteLine("(" + x + "," + y + "," + z + ")");
        }

        public static double Unsquared_Distance(Point_3d point1, Point_3d point2)
        {//pythagerion distance before sqrt
            return Math.Pow((point1.x - point2.x), 2) + Math.Pow((point1.y - point2.y), 2) + Math.Pow((point1.z - point2.z), 2);
        }

        public static bool Same_Point(Point_3d point1, Point_3d point2)
        {
            if (point1.x == point2.x && point1.y == point2.y && point1.z == point2.z)
            {
                return true;
            }

            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (((Point_3d)obj).x == x && ((Point_3d)obj).y == y && ((Point_3d)obj).z == z)
            {
                return true;
            }

            return false;
        }
        public override int GetHashCode()
        {
            //absolutely terrible hash algo
            unchecked // Allows arithmetic overflow without throwing an exception
            {
                int hash = Math.Abs((int)x);
                hash += 3 * Math.Abs((int)z);
                hash += 7 * Math.Abs((int)y);

                return hash;
            }
        }
        public static Point_3d Middle_Point(Point_3d p1, Point_3d p2)
        {
            return new Point_3d(
                (p1.x + p2.x) / 2,
                (p1.y + p2.y) / 2,
                (p1.z + p2.z) / 2
            );
        }

        public static Point_3d Point_Plus_Vector(Point_3d point, Vector vector)
        {
            return new Point_3d(
                point.x + vector.delta_x,
                point.y + vector.delta_y,
                point.z + vector.delta_z
            );
        }

        public static Point_3d Find_Centre(Point_3d origin, Point_3d point, Vector normal_vector)
        {
            double t =
            (
                (
                    point.x * normal_vector.delta_x +
                    point.y * normal_vector.delta_y +
                    point.z * normal_vector.delta_z
                )
                -
                (
                    normal_vector.delta_x * origin.x +
                    normal_vector.delta_y * origin.y +
                    normal_vector.delta_z * origin.z
                )
            )
            /
            (
                Math.Pow(normal_vector.delta_x, 2) +
                Math.Pow(normal_vector.delta_y, 2) +
                Math.Pow(normal_vector.delta_z, 2)
            );

            return new Point_3d(
                origin.x + normal_vector.delta_x * t,
                origin.y + normal_vector.delta_y * t,
                origin.z + normal_vector.delta_z * t
            );

        }
        public void Rotate_Point(Point_3d middle, int angle)
        {//rotate point oround middle
            if (x == middle.x && y == middle.y)
            {
                return;
            }

            double distance = Math.Sqrt(Math.Pow(x - middle.x, 2) + Math.Pow(y - middle.y, 2));
            double curr_ang;

            //find current anlge
            if (x == middle.x)
            {
                if (y > middle.y)
                {
                    curr_ang = 180;
                }
                else
                {
                    curr_ang = 0;
                }
            }
            else if (y == middle.y)
            {
                if (x < middle.x)
                {
                    curr_ang = 270;
                }
                else
                {
                    curr_ang = 90;
                }
            }
            else if (x > middle.x && y < middle.y)
            {
                curr_ang = Utility.Atan(Math.Abs(x - middle.x), Math.Abs(y - middle.y));
            }
            else if (x > middle.x && y > middle.y)
            {
                curr_ang = 90 + Utility.Atan(Math.Abs(y - middle.y), Math.Abs(x - middle.x));
            }
            else if (x < middle.x && y > middle.y)
            {
                curr_ang = 180 + Utility.Atan(Math.Abs(x - middle.x), Math.Abs(y - middle.y));
            }
            else
            {
                curr_ang = 270 + Utility.Atan(Math.Abs(y - middle.y), Math.Abs(x - middle.x));
            }

            //add modifer anlge
            curr_ang += (angle / Globals.scaling);
            curr_ang %= 360;

            //translate to new point
            double new_x;
            double new_y;
            if (curr_ang == 0)
            {
                new_x = middle.x;
                new_y = middle.y - distance;
            }
            else if (curr_ang < 90)
            {
                new_x = middle.x + Math.Abs(Utility.Sin(curr_ang) * distance);
                new_y = middle.y - Math.Abs(Utility.Cos(curr_ang) * distance);
            }
            else if (curr_ang == 90)
            {
                new_x = middle.x + distance;
                new_y = middle.y;
            }
            else if (curr_ang < 180)
            {
                new_x = middle.x + Math.Abs(Utility.Cos(curr_ang - 90) * distance);
                new_y = middle.y + Math.Abs(Utility.Sin(curr_ang - 90) * distance);

                //Trace.WriteLine("\n\ndist " + distance.ToString() + " angle " + curr_ang.ToString());
                //Trace.WriteLine((Math.Abs(Utility.Sin(curr_ang - 90) * distance).ToString()));
            }
            else if (curr_ang == 180)
            {
                new_x = middle.x;
                new_y = middle.y + distance;
            }
            else if (curr_ang < 270)
            {
                new_x = middle.x - Math.Abs(Utility.Sin(curr_ang - 180) * distance);
                new_y = middle.y + Math.Abs(Utility.Cos(curr_ang - 180) * distance);
            }
            else if (curr_ang == 270)
            {
                new_x = middle.x - distance;
                new_y = middle.y;
            }
            else//if (curr_ang < 360)
            {
                new_x = middle.x - Math.Abs(Utility.Cos(curr_ang - 270) * distance);
                new_y = middle.y - Math.Abs(Utility.Sin(curr_ang - 270) * distance);
            }

            x = new_x;
            y = new_y;
        }

        public void Earliest_Midpoint(Point_3d origin, Vector nv, Point_3d reference)
        {
            //move point until it reaches the front of our plane
            //using binary search with a know from point
            do
            {
                x = Math.Min(x, reference.x) + Math.Abs(x - reference.x) / 2;
                y = Math.Min(y, reference.y) + Math.Abs(y - reference.y) / 2;
                z = Math.Min(z, reference.z) + Math.Abs(z - reference.z) / 2;

            } while (Face.Infront(this, origin, nv));
        }

        public void Relative_Point(Point_3d middle, Point_3d origin)
        {//move point of a obj realtive to objects origin
            double distance_x = x - middle.x;
            double distance_y = y - middle.y;
            double distance_z = z - middle.z;

            x = origin.x + distance_x;
            y = origin.y + distance_y;
            z = origin.z + distance_z;
        }

        public void Translate_Point(Point_3d middle, int angle, Point_3d origin)
        {
            this.Rotate_Point(middle, angle);
            this.Relative_Point(middle, origin);
        }
    }
}
