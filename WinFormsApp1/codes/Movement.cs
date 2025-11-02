using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace WinFormsApp1
{
    class Movement
    {
        private static double[] Vector_XY(int xy_angle)
        {//return a ratio of the direction a user is looking
            double x_movement;
            double y_movement;

            double xy = xy_angle / Globals.scaling;

            if (xy < 90)
            {
                x_movement = Utility.Sin(xy);
                y_movement = -1 * Utility.Cos(xy);
            }
            else if (xy < 180)
            {
                x_movement = Utility.Cos(xy - 90);
                y_movement = Utility.Sin(xy - 90);
            }
            else if (xy < 270)
            {
                x_movement = -1 * Utility.Sin(xy - 180);
                y_movement = Utility.Cos(xy - 180);
            }
            else// < 360 
            {
                x_movement = -1 * Utility.Cos(xy - 270);
                y_movement = -1 * Utility.Sin(xy - 270);
            }

            return new double[] { x_movement * Globals.step_size, y_movement * Globals.step_size };
        }
        public static void Keep_Player_In_Playable_Area(ref Point_3d origin)
        {
            Math.Max(origin.x, Globals.x_negative_boarder);
            Math.Min(origin.x, Globals.x_positive_boarder);
            Math.Max(origin.y, Globals.y_negative_boarder);
            Math.Min(origin.y, Globals.y_positive_boarder);
            Math.Max(origin.z, Globals.z_negative_boarder);
            Math.Min(origin.z, Globals.z_positive_boarder);
        }
        
        public static void Handle_Movement(ref Point_3d origin, int xy_angle , KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)//Forward
                Movement.Handle_Forward(xy_angle, ref origin); 
            if (e.KeyCode == Keys.A)//Left
                Movement.Handle_Left(xy_angle, ref origin);
            if (e.KeyCode == Keys.D)//Right
                Movement.Handle_Right(xy_angle, ref origin);
            if (e.KeyCode == Keys.S)//Backwards
                Movement.Handle_Backwards(xy_angle, ref origin);
            if (e.KeyCode == Keys.Space)//Up
                Movement.Handle_Up(xy_angle, ref origin);
            if (e.Shift)//Down
                Movement.Handle_Down(xy_angle, ref origin);

            Keep_Player_In_Playable_Area(ref origin);
        }

        public static void Handle_Forward(int xy_angle, ref Point_3d origin)
        {
            double[] movements = Vector_XY(xy_angle);

            origin.x = origin.x + movements[0];
            origin.y = origin.y + movements[1];
        }
        public static void Handle_Left(int xy_angle, ref Point_3d origin)
        {
            double[] movements = Vector_XY(xy_angle);

            origin.x = origin.x + movements[1];
            origin.y = origin.y - movements[0];
        }
        public static void Handle_Right(int xy_angle, ref Point_3d origin)
        {
            double[] movements = Vector_XY(xy_angle);

            origin.x = origin.x - movements[1];
            origin.y = origin.y + movements[0];
        }

        public static void Handle_Backwards(int xy_angle, ref Point_3d origin)
        {
            double[] movements = Vector_XY(xy_angle);

            origin.x = origin.x - movements[0];
            origin.y = origin.y - movements[1];
        }

        public static void Handle_Up(int xy_angle, ref Point_3d origin)
        {
            origin.z = origin.z - Globals.step_size;
        }

        public static void Handle_Down(int xy_angle, ref Point_3d origin)
        {
            origin.z = origin.z + Globals.step_size;  
        }

        public static void Handle_Mouse(MouseEventArgs e, ref int xy_angle, ref int z_angle, bool cage_mouse)
        {
            //center of screen relative to picture box
            int x = e.X + Globals.left_right_border/2;
            int y = e.Y + Globals.top_border + Globals.ribbon_height;

            //Trace.WriteLine("x,y " + x + " " + y);
            
            int move_h = x - (Globals.width / 2 + Globals.left_right_border / 2);
            //find movement vertically
            int move_v = y - (Globals.height / 2 + Globals.top_border);
            //Trace.WriteLine("movements " + move_h + " " + move_v);


           


            xy_angle += move_h;
            z_angle += move_v;


            //clamping xy
            if (xy_angle > Globals.maximum_xy_angle)
            {
                xy_angle = Globals.minimum_xy_angle + (xy_angle - Globals.maximum_xy_angle);
            }
            if (xy_angle < Globals.minimum_xy_angle)
            {
                xy_angle = Globals.maximum_xy_angle + xy_angle;
            }

            //clamp z
            z_angle = Math.Max(z_angle, Globals.minimum_z_angle);
            z_angle = Math.Min(z_angle, Globals.maximum_z_angle);


            //Trace.WriteLine("x,y " + xy_angle + " " + z_angle);

            //move cursor back to middle*/
            //Cursor.Position = new Point(Globals.width / 2 + Globals.left_right_border/2, Globals.height / 2 + Globals.top_border);
            Cursor.Position = new Point(Globals.width / 2 + Globals.left_right_border / 2, Globals.height / 2 + Globals.top_border);
        }
    }
}