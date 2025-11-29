using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace WinFormsApp1
{
    class Render
    {
        public static void Print_Angle(Form1 screen, int xy_angle, int z_angle)
        {
            screen.angle_label.Text = "X-Y: " + (xy_angle / 100).ToString() + " Z: " + (z_angle / 100).ToString();
        }

        public static void Print_Coordinates(Form1 screen, Point_3d origin)
        {
            screen.coordinates_label.Text =
                "Coordinates: [" +
                Math.Round(origin.x, 2).ToString() +
                ", " +
                Math.Round(origin.y, 2).ToString() +
                ", " +
                Math.Round(origin.z, 2).ToString() +
                " ]"
            ;
        }

        public static void Print_Visuals(Form1 screen, Bitmap buffer)
        {
            screen.render_screen.Image = buffer;
        }

        public static void Clear_Screen(Bitmap bm)
        {
            //a bit manual but will do for now
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.Clear(Color.White);
            }
        }

        public static long[] Initialize_Span_Ring_Array()
        {
            long[] span_array = new long[100];
            for (int i = 0; i < span_array.Length; i++)
            {
                span_array[i] = 900;//random seed number
            }

            return span_array;
        }

        public static long Span_Ring_Array(ref long[] spans, long new_span)
        {
            //put in new element and remove oldest/ last element
            //return average
            long temp;
            long total = 0;

            //to prevent deviding by zero
            if (new_span == 0)
            {
                new_span = 1;
            }

            for (int i = 0; i < spans.Length; i++)
            {
                temp = spans[i];
                spans[i] = new_span;
                new_span = temp;

                total += spans[i];
            }

            return total / spans.Length;
        }

        //heavy rendering math equations

        public static bool Same_Side_Of_Zero(double a, double b)
        {
            if (a <= 0 && b <= 0)
            {
                return true;
            }
            if (a >= 0 && b >= 0)
            {
                return true;
            }

            return false;
        }

        public static bool Same_Side_Of_Line(Screen_Point a, Screen_Point b, Screen_Point reference, Screen_Point check)
        {
            double abg = (a.y - b.y) / (a.x - b.x);

            if (double.IsInfinity(abg))
            {
                if (b.y == a.y)
                {
                    if ((check.y > b.y && reference.y < b.y) || (check.y < b.y && reference.y > b.y))//on diference sides of y return
                    {
                        return false;
                    }
                }
                else
                {//x==x
                    if ((check.x > b.x && reference.x < b.x) || (check.x < b.x && reference.x > b.x))//on diference sides of y return
                    {
                        return false;
                    }
                }
            }
            else
            {
                double abi = b.y - (abg * b.x);
                double reference2 = abg * reference.x + abi - reference.y;
                double test2 = abg * check.x + abi - check.y;
                //Trace.WriteLine(reference2.ToString() + " " + test2.ToString() + " " + bcg.ToString());

                if (!Same_Side_Of_Zero(test2, reference2))
                {
                    return false;
                }
            }

            return true;
        }
        public static bool In_Bounds(Screen_Point a, Screen_Point b, Screen_Point p)
        {
            if (p.x <= Math.Max(a.x, b.x) &&
                        p.x >= Math.Min(a.x, b.x) &&
                        p.y <= Math.Max(a.y, b.y) &&
                        p.y >= Math.Min(a.y, b.y)
                    )//not more exteme then any of its point
            {
                return true;
            }
            return false;
        }

        public static bool On_Line(Screen_Point a, Screen_Point b, Screen_Point p)
        {
            if (a.x == b.x)//only rise
            {
                if (p.x == a.x && In_Bounds(a, b, p))
                {
                    return true;
                }
            }
            else if (a.y == b.y)//only run
            {
                if (p.y == a.y && In_Bounds(a, b, p))
                {
                    return true;
                }
            }
            else
            {
                //Trace.WriteLine("here");
                //line forn
                double gradient = (double)(a.y - b.y) / (double)(a.x - b.x);
                //-3 = 2x + i
                double intercept = ((double)a.y - gradient * (double)a.x);

                //one line mena this equation should equal 0 gx+i-y
                double tester = gradient * (double)p.x + intercept - (double)p.y;

                //Trace.WriteLine(gradient.ToString() +" "+ intercept.ToString() +" " +tester.ToString());
                if (tester >= -0.001 && tester <= 0.001)
                {
                    if (In_Bounds(a, b, p))//not more exteme then any of its point
                    {
                        return true;
                    }
                }
            }

            return false;
            //line for 
        }

        public static bool In_Triangle(Screen_Point a, Screen_Point b, Screen_Point c, Screen_Point p)
        {
            //following algorith not very precise around the edges
            if (On_Line(a, b, p) || On_Line(b, c, p) || On_Line(c, a, p))
            {
                return true;
            }

            //Barycentric coordinates taken from google
            // Calculate vectors
            Vector2 v0 = new Vector2((float)(c.x - a.x), (float)(c.y - a.y));
            Vector2 v1 = new Vector2((float)(b.x - a.x), (float)(b.y - a.y));
            Vector2 v2 = new Vector2((float)(p.x - a.x), (float)(p.y - a.y));

            // Calculate dot products
            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            // Calculate barycentric coordinates
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v < 1); // < 1 accounts for floating point precision
        }

        public static Point_3d Line_Plane_Interception(Point_3d origin, Vector nv1, Point_3d plane_point, Vector nv2)
        {
            //probalby should have some paralell checks

            double cons = nv2.delta_x * plane_point.x + nv2.delta_y * plane_point.y + nv2.delta_z * plane_point.z;
            double cons2 = cons - nv2.delta_x * origin.x - nv2.delta_y * origin.y - nv2.delta_z * origin.z;
            double t = cons2 / (nv2.delta_x * nv1.delta_x + nv2.delta_y * nv1.delta_y + nv2.delta_z * nv1.delta_z);
            return new Point_3d(origin.x + nv1.delta_x * t, origin.y + nv1.delta_y * t, origin.z + nv1.delta_z * t);
        }

        public static void Render_Triangle(Screen_Point a, Screen_Point b, Screen_Point c, Face face, Point_3d outside, Colour colour, Point_3d origin, int xy_angle, Vector nv, double[,] best, Bitmap bm, Point_3d tl)
        {
            //declare varibale for efficency
            Vector h_v = new Vector(Utility.Cos(xy_angle / 100), Utility.Sin(xy_angle / 100), 0);
            Vector v_v = Vector.Cross_Product(nv, h_v);


            Colour cc = Colour.Colour_By_Anlge(face, outside, colour);
            Color final_colour = Color.FromArgb(cc.red, cc.green, cc.blue);
            //Color final_colour = Color.FromArgb(10, 90, 40);


            //find bouding box
            int top = (int)Utility.Min(a.y, b.y, c.y);
            int left = (int)Utility.Min(a.x, b.x, c.x);
            int bottom = (int)Utility.Max(a.y, b.y, c.y);
            int right = (int)Utility.Max(a.x, b.x, c.x);

            //clamp boudning boc
            int x_start = Math.Max(left, 0);
            int x_end = Math.Min(right, Globals.width - 1);
            int y_start = Math.Max(top, 0);
            int y_end = Math.Min(bottom, Globals.height - 1);

            Vector face_nv = Vector.Normal_Vector(face);

            //Trace.WriteLine("triangle started to render " + x_start + " " + x_end + "     " + y_start + " " + y_end);
            //Console.WriteLine("triangle started to render " + x_start + " " + x_end + "     " + y_start + " " + y_end);

            for (int x = x_start; x <= x_end; x++)
            {
                for (int y = y_start; y <= y_end; y++)
                {
                    if (In_Triangle(a, b, c, new Screen_Point(x, y)))
                    {
                        //find anlge of viewer to specific pixel on screen
                        Point_3d pixel_in_3d_space = New_Angle_Point(x, y, h_v, v_v, tl);
                        Vector pixel_angle_vector = Vector.Vector_Between_Points(origin, pixel_in_3d_space);

                        //find where this vector intersects the face (find z_index)
                        Point_3d intersection = Line_Plane_Interception(origin, pixel_angle_vector, face.p1, face_nv);


                        double new_distance = Point_3d.Unsquared_Distance(origin, intersection);

                        if (best[x, y] == 0 || new_distance < best[x, y])
                        {
                            best[x, y] = new_distance;
                            bm.SetPixel(x, y, final_colour);
                        }
                    }
                }
            }

            //Trace.WriteLine("traignel done rendering");
            //Console.WriteLine("traignel done rendering");
        }



        public static Point_3d New_Angle_Point(int x, int y, Vector horizontal_vector, Vector vertical_vector, Point_3d tl)
        {
            double xx = (double)x;
            double yy = (double)y;
            double ww = (double)Globals.width;
            double hh = (double)Globals.height;

            double move_right = (xx + 1) / ww;
            double move_down = (yy + 1) / hh;

            return new Point_3d(tl.x + (move_right * 2 * horizontal_vector.delta_x) + (move_down * vertical_vector.delta_x),
                tl.y + (move_right * 2 * horizontal_vector.delta_y) + (move_down * vertical_vector.delta_y),
                tl.z + (move_right * 2 * horizontal_vector.delta_z) + (move_down * vertical_vector.delta_z));
        }



        //public static Point_3d 
        public static bool Translate_Face_2d(ref Screen_Point a, ref Screen_Point b, ref Screen_Point c, Face face, Point_3d origin, int xy_angle, Vector nv, Vector h_v, Vector v_v)
        {
            //Console.WriteLine("\t\tTranslate_Face 1");

            //Console.WriteLine("origin: " + origin.ToString());
            //Console.WriteLine("angles xy, z" + xy_angle);


            //check if all points behind using plane equations
            bool p1_infront = Face.Infront(face.p1, origin, nv);
            bool p2_infront = Face.Infront(face.p2, origin, nv);
            bool p3_infront = Face.Infront(face.p3, origin, nv);

            //Console.WriteLine("p1 + status " + face.p1.ToString() +  " " + p1_infront);
            //Console.WriteLine("p2 + status " + face.p2.ToString() + " " + p2_infront);
            //Console.WriteLine("p3 + status " + face.p3.ToString() + " " + p3_infront);


            if (!p1_infront && !p2_infront && !p3_infront)
            {
                Console.WriteLine("\t\tTranslate_Face 1.1");
                return false; //no points are infront of screen so impossible to have any renderign effect
            }



            Point_3d reference;
            //find an onscreen point
            if (p1_infront)
            {
                reference = face.p1;
            }
            else if (p2_infront)
            {
                reference = face.p2;
            }
            else
            {//p3
                reference = face.p3;
            }

            Screen_Point reference_point = new Screen_Point();
            reference_point.Translate_2d(origin, reference, xy_angle, nv, h_v, v_v);
            //for points not on screen

            //Console.WriteLine("\t\tTranslate_Face 2");
            //get all points

            if (p1_infront)
            {
                //Console.WriteLine("\t\tTranslate_Face 2.1");
                a.Translate_2d(origin, face.p1, xy_angle, nv, h_v, v_v);
                //Console.WriteLine("\t\tTranslate_Face 2.2");
            }
            else
            {
                //Console.WriteLine(p1_infront + " " + p2_infront + " " + p3_infront);
                //Console.WriteLine("\t\tTranslate_Face 2.3 " + face.p1.ToString() + " " + xy_angle + " " + reference.ToString());
                //get aproximate point extrapolate to near infinity
                face.p1.Earliest_Midpoint(origin, nv, reference);
                //Console.WriteLine("\t\tTranslate_Face 2.4");
                //plot out new point 
                a.Translate_2d(origin, face.p1, xy_angle, nv, h_v, v_v);
                //Console.WriteLine("\t\tTranslate_Face 2.5");
                //mody a by gradient
                a.Extrapolate(reference_point);
                //Console.WriteLine("\t\tTranslate_Face 2.6");
            }
            //Console.WriteLine("\t\tTranslate_Face 3");
            if (p2_infront)
            {
                //Console.WriteLine("\t\tTranslate_Face 3.1");
                b.Translate_2d(origin, face.p2, xy_angle, nv, h_v, v_v);
                //Console.WriteLine("\t\tTranslate_Face 3.2");
            }
            else
            {
                //Console.WriteLine(p1_infront + " " + p2_infront + " " + p3_infront);
                //Console.WriteLine("\t\tTranslate_Face 3.3 " + face.p2.ToString() + " " + xy_angle + " " + reference.ToString());
                //get aproximate point extrapolate to near infinity
                face.p2.Earliest_Midpoint(origin, nv, reference);
                //Console.WriteLine("\t\tTranslate_Face 3.4");
                //plot out new point 
                b.Translate_2d(origin, face.p2, xy_angle, nv, h_v, v_v);
                //Console.WriteLine("\t\tTranslate_Face 3.5");
                //mody a by gradient
                b.Extrapolate(reference_point);
                //Console.WriteLine("\t\tTranslate_Face 3.6");
            }
            //Console.WriteLine("\t\tTranslate_Face 4");
            
            if (p3_infront)
            {
                //Console.WriteLine("\t\tTranslate_Face 4.1");
                c.Translate_2d(origin, face.p3, xy_angle, nv, h_v, v_v);
                //Console.WriteLine("\t\tTranslate_Face 4.2");
            }
            else
            {
                //Console.WriteLine(p1_infront + " " + p2_infront + " " + p3_infront);
                //Console.WriteLine("\t\tTranslate_Face 4.3 " + face.p3.ToString() + " " + xy_angle + " " +  reference.ToString());
                //get aproximate point extrapolate to near infinity
                face.p3.Earliest_Midpoint(origin, nv, reference);
                //Console.WriteLine("\t\tTranslate_Face 4.4");
                //plot out new point 
                c.Translate_2d(origin, face.p3, xy_angle, nv, h_v, v_v);
                //Console.WriteLine("\t\tTranslate_Face 4.5");
                //mody a by gradient
                c.Extrapolate(reference_point);
                //Console.WriteLine("\t\tTranslate_Face 4.6");
            }

            //Console.WriteLine("\t\tTranslate_Face 5");

            return true;
        }

        public static void Render_Assets(Asset_Instance[] temp, int xy_angle, int z_angle, Point_3d origin, ref Bitmap bm)
        {
            double[,] best = new double[Globals.width, Globals.height];
            Clear_Screen(bm);

            Vector nv = Vector.Normal_Vector_Angle(xy_angle, z_angle);
            Point_3d d_of_1 = new Point_3d(origin.x + nv.delta_x, origin.y + nv.delta_y, origin.z + nv.delta_z);
            //Trace.WriteLine("ra 2");
            //vectors for movement in plane perpendiculat to viewing angle
            Vector horizontal_vector = new Vector(Utility.Cos(xy_angle / 100), Utility.Sin(xy_angle / 100), 0);
            Vector vertical_vector = Vector.Cross_Product(nv, horizontal_vector);
            //at a distance of 1 from the viewer this is the top left boundery of what is viewable
            Vector left = new Vector(-1 * horizontal_vector.delta_x, -1 * horizontal_vector.delta_y, -1 * horizontal_vector.delta_z);//
            Vector top = new Vector(-1 * vertical_vector.delta_x / 2, -1 * vertical_vector.delta_y / 2, -1 * vertical_vector.delta_z / 2);
            Point_3d top_left = Point_3d.Point_Plus_Vector(Point_3d.Point_Plus_Vector(d_of_1, top), left);
            //Trace.WriteLine("ra 3");


            //Console.WriteLine("Render Assets 1");
            for (int i = 0; i < Globals.max_users; i++)
            {
                if (i != Globals.id && temp[i].online)
                {
                    Render_An_Asset(temp[i], xy_angle, z_angle, origin, ref bm, ref best, nv, horizontal_vector, vertical_vector, top_left);
                }
            }
            //Console.WriteLine("Render Assets 2");
        }
        public static void Render_An_Asset(Asset_Instance temp, int xy_angle, int z_angle, Point_3d origin, ref Bitmap bm, ref double[,] best, Vector nv, Vector h_v, Vector v_v, Point_3d tl)
        {
            //Console.WriteLine("\tour index that is breaking \t" + temp.asset_index);
            for (int i = 0; i < Form1.assets[temp.asset_index].faces.Length; i++)
            {
                               //Console.WriteLine("\tRender AA 1");
                Point_3d inside = Form1.assets[temp.asset_index].internal_points[i].Copy();
                Point_3d outside = Form1.assets[temp.asset_index].external_points[i].Copy();

                inside.Translate_Point(Form1.assets[temp.asset_index].middle, temp.angle, temp.origin);
                outside.Translate_Point(Form1.assets[temp.asset_index].middle, temp.angle, temp.origin);

                double distance_inside = Point_3d.Unsquared_Distance(origin, inside);
                double distance_outside = Point_3d.Unsquared_Distance(origin, outside);

                if (distance_inside <= distance_outside)//only render when looking at external side of face
                {
                    continue;
                }

                Face face_t = Form1.assets[temp.asset_index].faces[i].Copy();
                face_t.Translate_Face(Form1.assets[temp.asset_index].middle, temp.angle, temp.origin);
                

                Screen_Point a = new Screen_Point();
                Screen_Point b = new Screen_Point();
                Screen_Point c = new Screen_Point();

                //Console.WriteLine("\tRender AA 1.2");

                if (!Translate_Face_2d(ref a, ref b, ref c, face_t, origin, xy_angle, nv, h_v, v_v))//face generates no on screen gemetries
                {
                    //Console.WriteLine("\tRender AA 1.3");
                    continue;
                }

                //Console.WriteLine("\tRender AA 2");
                //Console.WriteLine("Test dist new" + Point_3d.Unsquared_Distance(outside, Face.Middle(face_t)));
                if(Point_3d.Unsquared_Distance(outside, Face.Middle(face_t)) > 1.1 )
                {
                    //Console.WriteLine("Test dist" + Point_3d.Unsquared_Distance(Form1.assets[temp.asset_index].external_points[i], Face.Middle(Form1.assets[temp.asset_index].faces[i])));
                    //Console.WriteLine("Test dist new" + Point_3d.Unsquared_Distance(outside, Face.Middle(face_t)));

                    //Console.WriteLine("originla outdise point" + Form1.assets[temp.asset_index].external_points[i].ToString());
                    //Console.WriteLine()

                    //Console.WriteLine("rotation values" + Form1.assets[temp.asset_index].middle.ToString() + " " + temp.angle + " " + temp.origin.ToString());


                    Point_3d outside2 = Form1.assets[temp.asset_index].external_points[i].Copy();

                    
                    //outside2.Translate_Point2(Form1.assets[temp.asset_index].middle, temp.angle, temp.origin);

                    //Console.WriteLine("originl points");
                    //Console.WriteLine(Form1.assets[temp.asset_index].faces[i].p1.ToString());
                    //Console.WriteLine(Form1.assets[temp.asset_index].faces[i].p2.ToString());
                    //Console.WriteLine(Form1.assets[temp.asset_index].faces[i].p3.ToString());

                    //Console.WriteLine("after points");
                    //Console.WriteLine(face_t.p1.ToString());
                    //Console.WriteLine(face_t.p2.ToString());
                    //Console.WriteLine(face_t.p3.ToString());

                    //Console.WriteLine("o outside");
                    //Console.WriteLine(Form1.assets[temp.asset_index].external_points[i].ToString());
                    //Console.WriteLine("a outside");
                    //Console.WriteLine(outside.ToString());



                }

                Render_Triangle(a, b, c, face_t, outside, temp.colour, origin, xy_angle, nv, best, bm, tl);

               // Console.WriteLine("\tRender AA 3");

            }
            //Console.WriteLine("\tRender AA Post loop");
        }




    }
}