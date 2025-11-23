using System.Diagnostics;

namespace WinFormsApp1
{
    public class Face
    {
        public Point_3d p1;
        public Point_3d p2;
        public Point_3d p3;

        public Face(Point_3d initial_p1, Point_3d initial_p2, Point_3d initial_p3)
        {
            p1 = initial_p1;
            p2 = initial_p2;
            p3 = initial_p3;
        }

        public static Point_3d Middle(Face face)
        {
            double x = (face.p1.x + face.p2.x + face.p3.x) / 3;
            double y = (face.p1.y + face.p2.y + face.p3.y) / 3;
            double z = (face.p1.z + face.p2.z + face.p3.z) / 3;

            return new Point_3d(x, y, z);
        }

        public Face Copy()
        {
            return new Face(p1.Copy(), p2.Copy(), p3.Copy());
        }

        public override bool Equals(object obj)
        {
            //assuming faces are made of three unique points
            Dictionary<Point_3d, int> tally = new Dictionary<Point_3d, int>();

            //add the frist face's points
            tally.Add(((Face)obj).p1, 1);
            tally.Add(((Face)obj).p2, 1);
            tally.Add(((Face)obj).p3, 1);

            //check if seconds face matches up
            if (tally.ContainsKey(p1) && tally.ContainsKey(p2) && tally.ContainsKey(p3))
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            //terrible hashing algo
            unchecked // Allows arithmetic overflow without throwing an exception
            {
                int hash_p1 = p1.GetHashCode();
                int hash_p2 = p2.GetHashCode();
                int hash_p3 = p3.GetHashCode();
                return hash_p1 + hash_p2 + hash_p2;
            }
        }

        public void Print_Face()
        {
            Trace.WriteLine("face");
            p1.Print_Point();
            p2.Print_Point();
            p3.Print_Point();

        }
        public static bool Neighbour_Face(Face face1, Face face2)
        {
            Dictionary<Point_3d, int> tally = new Dictionary<Point_3d, int>();

            //add the frist face
            tally.Add(face1.p1, 1);
            tally.Add(face1.p2, 1);
            tally.Add(face1.p3, 1);

            //if two matches face is a neighbour because they share an edge
            int matches = 0;
            if (tally.ContainsKey(face2.p1)) { matches++; }
            if (tally.ContainsKey(face2.p2)) { matches++; }
            if (tally.ContainsKey(face2.p3)) { matches++; }

            if (matches == 2)
            {
                return true;
            }

            return false;
        }

        public static Face[] Neighbours(Asset shape, Face face)
        {
            Face[] neighbours = new Face[3];
            int index = 0;

            for (int i = 0; i < shape.faces.Length; i++)
            {
                //face.Print_Face();
                //    shape.faces[i].Print_Face();
                if (face.Equals(shape.faces[i])) { continue; }//skip self

                if (Neighbour_Face(face, shape.faces[i]))
                {
                    neighbours[index] = shape.faces[i];
                    index++;
                }

                if (index == 3)
                {
                    break;
                }
            }

            return neighbours;
        }
        public static Point_3d[] Shared_Edge(Face face1, Face face2)
        {//returns shared edge between neighbour faces
            Dictionary<Point_3d, int> tally = new Dictionary<Point_3d, int>();

            //add the frist face
            tally.Add(face1.p1, 1);
            tally.Add(face1.p2, 1);
            tally.Add(face1.p3, 1);

            if (tally.ContainsKey(face2.p1))
            {
                tally[face2.p1]++;
            }
            else
            {
                tally.Add(face2.p1, 1);
            }

            if (tally.ContainsKey(face2.p2))
            {
                tally[face2.p2]++;
            }
            else
            {
                tally.Add(face2.p2, 1);
            }

            if (tally.ContainsKey(face2.p3))
            {
                tally[face2.p3]++;
            }
            else
            {
                tally.Add(face2.p3, 1);
            }

            List<Point_3d> points = new List<Point_3d>();
            List<Point_3d> non_edge_points = new List<Point_3d>();

            //get the two shared points
            foreach (KeyValuePair<Point_3d, int> kvp in tally)
            {
                if (kvp.Value == 2)
                {
                    points.Add(kvp.Key);
                }
                else
                {
                    non_edge_points.Add(kvp.Key);
                }
            }

            return points.Concat(non_edge_points).ToArray();
        }
        /*public static Point_3d[] Shared_Edge(Face face1, Face face2)
        {//returns shared edge between neighbour faces
            Dictionary<Point_3d, int> tally = new Dictionary<Point_3d, int>();

            //add the frist face
            tally.Add(face1.p1, 1);
            tally.Add(face1.p2, 1);
            tally.Add(face1.p3, 1);
            
            if (tally.ContainsKey(face2.p1))
            {
                tally[face2.p1]++;
            }
            else
            {
                tally.Add(face2.p1, 1);
            }

            if (tally.ContainsKey(face2.p2))
            {
                tally[face2.p2]++;
            }
            else
            {
                tally.Add(face2.p2, 1);
            }

            if (tally.ContainsKey(face2.p3))
            {
                tally[face2.p3]++;
            }
            else
            {
                tally.Add(face2.p3, 1);
            }

            List<Point_3d> points = new List<Point_3d>();

            //get the two shared points
            foreach (KeyValuePair<Point_3d, int> kvp in tally)
            {
                if (kvp.Value == 2)
                {
                    points.Add(kvp.Key);
                }
            }

            return points.ToArray();
        }*/

        public static double Plane_Equation(Vector nv, Point_3d plane_point, Point_3d candidate_point)
        {//when point is on plane returns 0
            return (
                nv.delta_x * plane_point.x - nv.delta_x * candidate_point.x +
                nv.delta_y * plane_point.y - nv.delta_y * candidate_point.y +
                nv.delta_z * plane_point.z - nv.delta_z * candidate_point.z
            );
        }



        public static bool Same_Plane(Face face1, Face face2)
        {
            Vector normal_vector = Vector.Normal_Vector(face1);
            Point_3d f1_centre = Face.Middle(face1);
            Point_3d f2_centre = Face.Middle(face2);

            double result = Face.Plane_Equation(normal_vector, f1_centre, f2_centre);

            //Trace.WriteLine("plane equation result" + result.ToString());

            if (Math.Abs(result) < 0.00001)
            {
                return true;
            }

            return false;
        }

        public static Point_3d[] Normal_Vector_Points(Face face)
        {//add normal vector to middle of the face
            Vector normal_vector = Vector.Normal_Vector(face);
            normal_vector.Scale_Vector();

            Point_3d middle = Face.Middle(face);

            Point_3d point1 = new Point_3d(middle.x + normal_vector.delta_x, middle.y + normal_vector.delta_y, middle.z + normal_vector.delta_z);
            Point_3d point2 = new Point_3d(middle.x - normal_vector.delta_x, middle.y - normal_vector.delta_y, middle.z - normal_vector.delta_z);


            return new Point_3d[] { point1, point2 };
        }

        public static bool Infront(Point_3d point, Point_3d reference, Vector nv)
        {
            double reference_d = Face.Plane_Equation(nv, reference, Point_3d.Point_Plus_Vector(reference, nv));

            
            double point_d = Face.Plane_Equation(nv, reference, point);

            //Console.WriteLine(point_d + " " + reference_d);
            //Console.WriteLine(Point_3d.Point_Plus_Vector(reference, nv).ToString());


            if (reference_d > 0 && point_d >= 0)//if on the plane its fine
            {
                return true;
            }
            if (reference_d < 0 && point_d <= 0)//if on plane its fine
            {// < 0
                return true;
            }

            return false;
        }
        public void Translate_Face(Point_3d middle, int angle, Point_3d origin)
        {
            this.p1.Translate_Point(middle, angle, origin);
            this.p2.Translate_Point(middle, angle, origin);
            this.p3.Translate_Point(middle, angle, origin);
        }

    }
}

/*public static double Closest_Point(Point_3d l1, Point_3d l2, Point_3d origin)
        {
            Vector v = Vector.Vector_Between_Points(l1, l2);
            Vector v1 = Vector.Vector_Between_Points(origin, l1);

            double dot1 = Vector.Dot_Product(v, v1);
            double dot2 = Vector.Dot_Product(v, v);

            double t = dot1 / dot2;

            Point_3d p = new Point_3d(l1.x + t * v.delta_x, l1.y + t * v.delta_y, l1.z + t * v.delta_z);

            //clamp down so point is between bounderi of the line
            bool x_bound = (((p.x <= l1.x && p.x >= l2.x) || (p.x >= l1.x && p.x <= l2.x)) ? true : false);
            bool y_bound = (((p.y <= l1.y && p.y >= l2.y) || (p.y >= l1.y && p.y <= l2.y)) ? true : false);
            bool z_bound = (((p.z <= l1.z && p.z >= l2.z) || (p.z >= l1.z && p.z <= l2.z)) ? true : false);

            if (!x_bound)
            {
                //either closer to l1 or l2
                if (Point_3d.Unsquared_Distance(p, l1) < Point_3d.Unsquared_Distance(p, l2))
                {
                    p = l1;
                }
                else
                {
                    p = l2;
                }
            }
            if (!y_bound)
            {
                //either closer to l1 or l2
                if (Point_3d.Unsquared_Distance(p, l1) < Point_3d.Unsquared_Distance(p, l2))
                {
                    p = l1;
                }
                else
                {
                    p = l2;
                }
            }
            if (!z_bound)
            {
                //either closer to l1 or l2
                if (Point_3d.Unsquared_Distance(p, l1) < Point_3d.Unsquared_Distance(p, l2))
                {
                    p = l1;
                }
                else
                {
                    p = l2;
                }
            }

            return Point_3d.Unsquared_Distance(p, origin);
        }*/