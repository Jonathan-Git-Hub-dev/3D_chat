using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1
{
    class Asset_Passer
    {
        public static bool Is_Vertex_Declaration(string str)
        {//used to parse obj files
            //cannot be vertex if str empty or str is comment
            if (string.IsNullOrWhiteSpace(str)) return false;
            if (str[0] == Globals.comment) return false;

            if (str.Length > 2 && str[0] == 'v' && str[1] == ' ')
            {
                return true;
            }
            return false;
        }

        public static bool Is_Face_Declaration(string str)
        {//used to parse obj files
            //cannot be vertex if str empty or str is comment
            if (string.IsNullOrWhiteSpace(str)) return false;
            if (str[0] == Globals.comment) return false;

            if (str.Length > 2 && str[0] == 'f' && str[1] == ' ')
            {
                return true;
            }
            return false;
        }
        public static void Convert_File_Obj_To_Custom(string input_file, string output_file)
        {//Asset_Passer.Convert_File_Obj_To_Custom(@"C:\Users\waho2\Desktop\py\sk.obj", Globals.asset_folder + "t12.txt");
            List<Point_3d> points = new List<Point_3d>();
            string output_string = "";//buffer for file printing


            try
            {
                foreach (string file_line in File.ReadLines(input_file))
                {
                    if(Is_Vertex_Declaration(file_line))
                    {
                        string[] point_variables = file_line.Split(' ');
                        double x = Convert.ToDouble(point_variables[1]);
                        double y = Convert.ToDouble(point_variables[2]);
                        double z = Convert.ToDouble(point_variables[3]);

                        points.Add(new Point_3d(x, y, z));
                    }

                    if(Is_Face_Declaration(file_line))
                    {
                        //get faces details
                        string[] face_deatils = file_line.Split(' ');

                        int[] point_indexes = new int[3];

                        for(int i = 0; i< 3; i++)
                        {
                            string[] single_face_details = face_deatils[i + 1].Split('/');
                            int index = int.Parse(single_face_details[0]);


                            point_indexes[i] = index - 1;
                        }

                        //structure faces details
                        StringBuilder string_builder = new StringBuilder();
                        string_builder.AppendFormat("[{0}] [{1}] [{2}]", points[point_indexes[0]], points[point_indexes[1]], points[point_indexes[2]]);
                        output_string += string_builder.ToString() + "\n";
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error reading from file: " + ex.Message);
            }


            try//output new file
            {
                File.WriteAllText(output_file, output_string);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file: " + ex.Message);
            }
        }


        public static Asset Read_Asset(string file)
        {
            List<Face> f = new List<Face>();
            try
            {
                foreach (string line in File.ReadLines(file))
                {//load each line into a face
                    if(line.Length == 0) continue;

                    string[] line_contents = line.Split(Globals.comment);
                    string[] line_content = line_contents[0].Split(' ');
                    //only the first three are important

                    Point_3d[] points = new Point_3d[3]; 

                    for(int i=0; i< 3; i++)
                    {
                        //if (line_content[i][0] != '[' || line_content[i][line_content[i].Length - 1] != ']') { }//format error
                        string trimmed = line_content[i].Substring(1, line_content[i].Length - 2);
                        string[] numbers = trimmed.Split(",");

                        double x = Convert.ToDouble(numbers[0]);
                        double y = Convert.ToDouble(numbers[1]);
                        double z = Convert.ToDouble(numbers[2]);

                        points[i] = new Point_3d(x, y, z);
                    }
                    f.Add(new Face(points[0], points[1], points[2]));
                }

                Asset asset = new Asset(f.ToArray());
                asset.Bounding_Box();

                /*for (int i = 0; i < asset.faces.Length; i++)
                {
                    Trace.WriteLine("pre stuff");
                    asset.faces[i].Print_Face();
                }*/

                Shape_Internal_Points(ref asset);//get internal and external points

                


                return asset;
            }
            catch (FileNotFoundException ex)
            {
                //Console.WriteLine($"Error: The file '{filePath}' was not found. {ex.Message}");
            }

            return null;//panic here
        }
       
        public static Point_3d Extreme_Point_Y(Asset obj)
        {
            //finding point that is most extreme in the y direction
            Point_3d best = new Point_3d(obj.faces[0].p1.x, obj.faces[0].p1.y, obj.faces[0].p1.z);

            for(int i=0; i<obj.faces.Length; i++)
            {
                if (obj.faces[i].p1.y > best.y)
                {
                    best.Set_Point_3d(obj.faces[i].p1);
                }
                if (obj.faces[i].p2.y > best.y)
                {
                    best.Set_Point_3d(obj.faces[i].p2);
                }
                if (obj.faces[i].p3.y > best.y)
                {
                    best.Set_Point_3d(obj.faces[i].p3);
                }
            }

            return best;
        }

        public static Face[] Exterme_Neighbours(Point_3d extreme_point, Asset obj)
        {
            //Trace.WriteLine("this is the start of our exn func");
            //find face that extreme face that has a neigbour not on the same plane

            //get extreme face (has a point that is max or min of (x,y or z)) 
            int face_index = -1;
            for (int i = 0; i < obj.faces.Length; i++)
            {
                if (
                    Point_3d.Same_Point(extreme_point, obj.faces[i].p1) ||
                    Point_3d.Same_Point(extreme_point, obj.faces[i].p2) ||
                    Point_3d.Same_Point(extreme_point, obj.faces[i].p3)
                )
                {
                    face_index = i;
                    break;
                }
            }


            Dictionary<Face, bool> tryed_faces = new Dictionary<Face, bool>();
            tryed_faces.Add(obj.faces[face_index], true);

            List<Face> candidates = new List<Face>();
            candidates.Add(obj.faces[face_index]);

            //check candidates till right face found
            while (candidates.Count > 0)
            {
                Face cand = candidates[0];
                candidates.RemoveAt(0);

                Face[] others = Face.Neighbours(obj, cand);

                for (int i = 0; i < others.Length; i++)
                {
                    if (!Face.Same_Plane(others[i], cand))
                    {
                        //Trace.WriteLine("this is the end of our exn func 1");
                        return new Face[] { cand, others[i] };
                    }
                }
               
                //else add these faces as new candidates if not seen already
                for (int i = 0; i < others.Length; i++)
                {
                    if (!tryed_faces.ContainsKey(others[i]))
                    {
                        candidates.Add(others[i]);
                        tryed_faces.Add(others[i], true);
                    }
                }
                
            }
            //Trace.WriteLine("this is the end of our exn func 2");
            return null;//this should never be reached
        }

        public static void Shape_Internal_Points(ref Asset obj)
        {
            Dictionary<Face, (Point_3d, Point_3d)> internal_points = new Dictionary<Face, (Point_3d, Point_3d)>();

            //get starting pair of faces
            Point_3d start_point = Extreme_Point_Y(obj);
            Face[] starts = Exterme_Neighbours(start_point, obj);

            //Trace.WriteLine("these are our starts");
            //starts[0].Print_Face();
            //starts[1].Print_Face();


            Point_3d middle_of_faces = Point_3d.Middle_Point(Face.Middle(starts[0]), Face.Middle(starts[1]));

            //find normal vector point closer to this middle point
            Point_3d[] options = Face.Normal_Vector_Points(starts[0]);
            if (Point_3d.Unsquared_Distance(middle_of_faces, options[0]) < Point_3d.Unsquared_Distance(middle_of_faces, options[1]))
            {
                internal_points.Add(starts[0],(options[0], options[1]));
            }
            else//greater then, never equal
            {
                internal_points.Add(starts[0], (options[1], options[0]));
            }

            //the face to figure out and it solved nighbour for reference
            List<(Face, Face)> next = new List<(Face, Face)>();
            next.Add((starts[1], starts[0]));

           // Trace.WriteLine("our first face");
            //starts[0].Print_Face();
            (Point_3d inside, Point_3d b) xxxx = internal_points[starts[0]];
            //xxxx.inside.Print_Point();



            int li = 1;
            while (next.Count > 0)//faces not solved
            {
                
                (Face target, Face solved_neighbour) tuple = next[0]; // Get the tuple
                next.RemoveAt(0); // Remove the tuple

                if(internal_points.ContainsKey(tuple.target)) continue;//target has been solved

                //Trace.WriteLine("interation " + li.ToString());
                li++;

                


                //shared edge
                Point_3d[] edge = Face.Shared_Edge(tuple.target, tuple.solved_neighbour);
                Point_3d edge_centre = Point_3d.Middle_Point(edge[0], edge[1]);

                Vector outwards_vector1;
                Vector outwards_vector2; //Vector_Multiplication(double factor)




                //new new new new new bit
                //get vertex from each face that isnt part of the shated edge
                //Point_3d.Middle_Point(e, Point_3d p2)
                if (Face.Same_Plane(tuple.solved_neighbour, tuple.target))
                {
                    outwards_vector1 = Vector.Normal_Vector(tuple.solved_neighbour);
                }
                else
                {
                    Point_3d mid_plane_point = Point_3d.Middle_Point(edge[2], edge[3]);
                    outwards_vector1 = Vector.Vector_Between_Points(edge_centre, mid_plane_point);
                }

                outwards_vector1.Scale_Vector();
                outwards_vector2 = outwards_vector1.Copy();
                outwards_vector2.Vector_Multiplication(-1);

                Point_3d inside_reference_candidate1 = Point_3d.Point_Plus_Vector(edge_centre, outwards_vector1);
                Point_3d inside_reference_candidate2 = Point_3d.Point_Plus_Vector(edge_centre, outwards_vector2);



                Point_3d target_centre = Face.Middle(tuple.target);
                Point_3d solved_centre = Face.Middle(tuple.solved_neighbour);

                //figure out which of these points in inside using the plane equation
                Vector solved_normal_vector = Vector.Normal_Vector(tuple.solved_neighbour);
                (Point_3d inside, Point_3d b) xxx = internal_points[tuple.solved_neighbour];
                double reference = Face.Plane_Equation(solved_normal_vector, solved_centre, xxx.inside);
                double reference_candidate1 = Face.Plane_Equation(solved_normal_vector, solved_centre, inside_reference_candidate1);
                double reference_candidate2 = Face.Plane_Equation(solved_normal_vector, solved_centre, inside_reference_candidate2);

                //inside point candidates for target
                Vector normal_vector1 = Vector.Normal_Vector(tuple.target);
                //normal_vector1.Print_Vector();
                normal_vector1.Scale_Vector();
                Vector normal_vector2 = new Vector(-1 * normal_vector1.delta_x, -1 * normal_vector1.delta_y, -1 * normal_vector1.delta_z);
             


                Point_3d candidate1 = Point_3d.Point_Plus_Vector(target_centre, normal_vector1);
                Point_3d candidate2 = Point_3d.Point_Plus_Vector(target_centre, normal_vector2);
                

                //make candidate 1 the right one
                if (reference < 0)
                {
                    if (reference_candidate2 < 0)
                    {
                        //reference_candidate1 = reference_candidate2;
                        inside_reference_candidate1 = inside_reference_candidate2;
                    }
                }
                else//greater then, should never be equal
                {
                    if (reference_candidate2 > 0)
                    {
                        //reference_candidate1 = reference_candidate2;
                        inside_reference_candidate1 = inside_reference_candidate2;
                    }
                }

                

                //which ever candidate is closer to reference is the inside point
                if (Point_3d.Unsquared_Distance(inside_reference_candidate1, candidate1) < Point_3d.Unsquared_Distance(inside_reference_candidate1, candidate2))
                {
                    internal_points[tuple.target] = (candidate1, candidate2);
                }
                else
                {
                    internal_points[tuple.target] = (candidate2, candidate1);
                }

                //this point is now solved so get its neighbours for solving
                Face[] neigbours = Face.Neighbours(obj, tuple.target);

                //Trace.WriteLine("neigbours " + neigbours.Length);

                for(int i =0; i<neigbours.Length; i++)
                {
                    next.Add((neigbours[i], tuple.target));
                }



            }

            //var names = internal_points.Select(p => p.).ToArray();
            //return internal_points;
            //now extract these points and asscicate them wiht faces
            Point_3d[] ip = new Point_3d[obj.faces.Length];
            Point_3d[] op = new Point_3d[obj.faces.Length];

            for(int i=0; i<obj.faces.Length; i++)
            {
                (Point_3d ins, Point_3d outs) tup = internal_points[obj.faces[i]];
                ip[i] = tup.ins;
                op[i] = tup.outs;
            }


            obj.Set_Internal_Points(ip);
            obj.Set_External_Points(op);
        }

        public static void Get_Assets(ref Asset[] assets)
        {
            for (int i = 0; i < Globals.asset_options.Length; i++)
            {
                assets[i] = Asset_Passer.Read_Asset(Globals.asset_folder_3d + Globals.asset_options[i]);
            }
        }
}
}