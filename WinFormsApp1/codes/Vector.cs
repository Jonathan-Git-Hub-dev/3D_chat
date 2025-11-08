using System.Diagnostics;

namespace WinFormsApp1
{
    public class Vector
    {
        public double delta_x;
        public double delta_y;
        public double delta_z;

        public Vector(double movement_x, double movement_y, double movement_z)
        {
            delta_x = movement_x;
            delta_y = movement_y;
            delta_z = movement_z;
        }



        public void Vec_Len()
        {
            double len = Math.Sqrt(Math.Pow(delta_x, 2) + Math.Pow(delta_y, 2) + Math.Pow(delta_z, 2));
            Trace.WriteLine("This is our length " + len.ToString());
        }
        public Vector Copy()
        {
            return new Vector(delta_x, delta_y, delta_z);
        }
        public void Vector_Multiplication(double factor)
        {
            delta_x = delta_x * factor;
            delta_y = delta_y * factor;
            delta_z = delta_z * factor;
        }
        public static bool Same_Vector(Vector v1, Vector v2)
        {
            v1.Scale_Vector();
            v2.Scale_Vector();

            if (v1.delta_x < 0)
            {//make positive
                v1.Vector_Multiplication(-1);
            }

            if (v2.delta_x < 0)
            {//make positive
                v2.Vector_Multiplication(-1);
            }

            if (v1.delta_x == v2.delta_x && v1.delta_y == v2.delta_y && v1.delta_z == v2.delta_z)
            {
                return true;
            }

            return false;
        }

        public void Print_Vector()
        {
            Trace.WriteLine(delta_x.ToString() + " " + delta_y.ToString() + " " + delta_z.ToString());
        }

        public static Vector Vector_Between_Points(Point_3d p1, Point_3d p2)
        {
            Vector new_vector = new Vector(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);

            return new_vector;
        }

        public static Vector Cross_Product(Vector v1, Vector v2)
        {
            double x_component = v1.delta_y * v2.delta_z - v1.delta_z * v2.delta_y;
            double y_component = v1.delta_z * v2.delta_x - v1.delta_x * v2.delta_z;
            double z_component = v1.delta_x * v2.delta_y - v1.delta_y * v2.delta_x;

            Vector cross_product = new Vector(x_component, y_component, z_component);

            return cross_product;
        }

        public static Vector Cross_Product2(Vector v1, Vector v2)
        {
            double x_component = v1.delta_y * v2.delta_z - v1.delta_z * v2.delta_y;
            double y_component = v1.delta_z * v2.delta_x - v1.delta_x * v2.delta_z;
            double z_component = v1.delta_x * v2.delta_y - v1.delta_y * v2.delta_x;

            Vector cross_product = new Vector(x_component, y_component, z_component);

            return cross_product;
        }

        /*public static double Dot_Product(Vector v1, Vector v2)
        {
            return v1.delta_x * v2.delta_x +
                v1.delta_y * v2.delta_y +
                v1.delta_z * v2.delta_z;
        }*/

        public static Vector Normal_Vector(Face face)
        {
            Vector v1 = Vector_Between_Points(face.p1, face.p2);
            Vector v2 = Vector_Between_Points(face.p1, face.p3);
            return Cross_Product(v1, v2);
        }

        public void Scale_Vector()
        {
            //if all elements of vector are 0 this will divide by zero
            double distance = Math.Pow(delta_x, 2) + Math.Pow(delta_y, 2) + Math.Pow(delta_z, 2);
            distance = Math.Sqrt(distance);

            double scale_factor = 1 / distance;

            delta_x = delta_x * scale_factor;
            delta_y = delta_y * scale_factor;
            delta_z = delta_z * scale_factor;
        }

        public static double[] Line_Of_Sight_Vector(int xy_angle)
        {
            double x_movement;
            double y_movement;

            double xy = xy_angle / 100;

            if (xy < 90)
            {
                x_movement = Utility.Sin(xy_angle);
                y_movement = -1 * Utility.Cos(xy_angle);
            }
            else if (xy < 180)
            {
                x_movement = Utility.Cos(xy_angle - 90);
                y_movement = Utility.Sin(xy_angle - 90);
            }
            else if (xy < 270)
            {
                x_movement = -1 * Utility.Sin(xy_angle - 180);
                y_movement = Utility.Cos(xy_angle - 180);
            }
            else// < 360 
            {
                x_movement = -1 * Utility.Cos(xy_angle - 270);
                y_movement = -1 * Utility.Sin(xy_angle - 270);
            }

            return new double[] { x_movement, y_movement };
        }

        public static Vector Normal_Vector_Angle(int xy_angle, int z_angle)
        {
            double z_movement = Utility.Sin(z_angle / 100);

            double remainder = Math.Sqrt(1 - Math.Pow(z_movement, 2));

            double x_movement;
            double y_movement;

            double xy = xy_angle / 100;

            if (xy < 90)
            {
                x_movement = Utility.Sin(xy) * remainder;
                y_movement = -1 * Utility.Cos(xy) * remainder;
            }
            else if (xy < 180)
            {
                x_movement = Utility.Cos(xy - 90) * remainder;
                y_movement = Utility.Sin(xy - 90) * remainder;
            }
            else if (xy < 270)
            {
                x_movement = -1 * Utility.Sin(xy - 180) * remainder;
                y_movement = Utility.Cos(xy - 180) * remainder;
            }
            else// < 360 
            {
                x_movement = -1 * Utility.Cos(xy - 270) * remainder;
                y_movement = -1 * Utility.Sin(xy - 270) * remainder;
            }

            return new Vector(x_movement, y_movement, z_movement);
        }
    }
}
/*public static void Same_Plane(Face face1, Face face2)
{
    //vectors
    Vector nv1 = Vector.Normal_Vector(face1);
    Vector nv2 = Vector.Normal_Vector(face2);

    nv1.Print_Vector();
    nv2.Print_Vector();

    //doing because normal vectors might have zeros
    double xm, xa, ym, ya, zm, za;

    //same no matter what
    double d1 = -1 * nv1.delta_x * face1.p1.x - 1 * nv1.delta_y * face1.p1.y - 1 * nv1.delta_z * face1.p1.z;
    double d2 = -1 * nv2.delta_x * face2.p1.x - 1 * nv2.delta_y * face2.p1.y - 1 * nv2.delta_z * face2.p1.z;
    //1,0,0
    //0,1,0
    //
    //



    if ((nv1.delta_x != 0 && nv2.delta_y != 0) || (nv1.delta_y != 0 && nv2.delta_x != 0))
    {

        zm = 1;
        za = 0;

        double df = nv2.delta_x * nv1.delta_y - nv1.delta_x * nv2.delta_y;

        ym = nv1.delta_x * nv2.delta_z / df - nv2.delta_x * nv1.delta_z / df;//=0
        ya = +nv1.delta_x * (d2) / df - nv2.delta_x * (d1) / df;//1

        if (nv1.delta_x != 0)
        {
            xm = -1 * nv1.delta_z / nv1.delta_x - nv1.delta_y * ym / nv1.delta_x;
            xa = -1 * nv1.delta_y * ya / nv1.delta_x - (d1) / nv1.delta_x;
        }
        else
        {
            xm = -1 * nv2.delta_z / nv2.delta_x - nv2.delta_y * ym / nv2.delta_x;
            xa = -1 * nv2.delta_y * ya / nv2.delta_x - (d2) / nv2.delta_x;
        }

    }
    else if ((nv1.delta_x != 0 && nv2.delta_z != 0) || (nv1.delta_z != 0 && nv2.delta_x != 0))
    {
        Trace.WriteLine("Second Second");

        double df = nv2.delta_x * nv1.delta_z - nv1.delta_x * nv2.delta_z;//=1
                                                                          //z= + nv1.delta_x*nv2.delta_y*y/df +nv1.delta_x*(d2)/df - nv2.delta_x*nv1.delta_y*y/df - +nv2.delta_x*(d1)/df
        ym = 1;
        ya = 0;
        zm = nv1.delta_x * nv2.delta_y / df - nv2.delta_x * nv1.delta_y / df;//=0
        za = +nv1.delta_x * (d2) / df - nv2.delta_x * (d1) / df;//1

        if (nv1.delta_x != 0)
        {
            xm = -1 * nv1.delta_y / nv1.delta_x - nv1.delta_z * zm / nv1.delta_x;
            xa = -1 * nv1.delta_z * za / nv1.delta_x - (d1) / nv1.delta_x;
        }
        else
        {
            xm = -1 * nv2.delta_y / nv2.delta_x - nv2.delta_z * zm / nv2.delta_x;
            xa = -1 * nv2.delta_z * za / nv2.delta_x - (d2) / nv2.delta_x;
        }
    }
    else if ((nv1.delta_y != 0 && nv2.delta_z != 0) || (nv1.delta_z != 0 && nv2.delta_y != 0))
    {
        Trace.WriteLine("Third Third");
        //remove y or z 
        //find for other
        xm = 1;
        xa = 0;
        //removing y

        //double df = nv2.delta_y * nv1.delta_z - nv1.delta_y * nv2.delta_z;



        double df = nv2.delta_y * nv1.delta_z - nv1.delta_y * nv2.delta_z;//=1
        Trace.WriteLine("df " + df.ToString());
        //z= + nv1.delta_x*nv2.delta_y*y/df +nv1.delta_x*(d2)/df - nv2.delta_x*nv1.delta_y*y/df - +nv2.delta_x*(d1)/df

        zm = nv1.delta_y * nv2.delta_x / df - nv2.delta_y * nv1.delta_x / df;//=0
        za = nv1.delta_y * (d2) / df - nv2.delta_y * (d1) / df;//1

        if (nv1.delta_y != 0)
        {
            ym = -1 * nv1.delta_x / nv1.delta_y - nv1.delta_z * zm / nv1.delta_y;
            ya = -1 * nv1.delta_z * za / nv1.delta_y - (d1) / nv1.delta_y;
        }
        else
        {
            Trace.WriteLine("this is here");
            ym = -1 * nv2.delta_x / nv2.delta_y - nv2.delta_z * zm / nv2.delta_y;
            ya = -1 * nv2.delta_z * za / nv2.delta_y - (d2) / nv2.delta_y;
        }

    }
    else
    {
        Trace.WriteLine("i dont think this should happen");
        xm = 0;
        ym = 0;
        zm = 0;
        xa = 0;
        ya = 0;
        za = 0;
    }

    Trace.WriteLine("x = t * " + xm.ToString() + " + " + xa.ToString());
    Trace.WriteLine("y = t * " + ym.ToString() + " + " + ya.ToString());
    Trace.WriteLine("z = t * " + zm.ToString() + " + " + za.ToString());

    //return Same_Vector(f1_nv, f2_nv);
    //return false;
}*/