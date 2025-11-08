using System.Diagnostics;

namespace WinFormsApp1
{
    public class Asset_Instance
    {
        public int asset_index;
        public Point_3d origin;
        public Colour colour;
        public int angle;
        public bool online;


        public Asset_Instance()
        {
            asset_index = 0;
            origin = new Point_3d(0, 0, 0);
            colour = new Colour(255, 0, 0);
            angle = 0;
            online = false;
        }

        public void Copy(Asset_Instance source)
        {
            asset_index = source.asset_index;

            origin.x = source.origin.x;
            origin.y = source.origin.y;
            origin.z = source.origin.z;

            colour.red = source.colour.red;
            colour.green = source.colour.green;
            colour.blue = source.colour.blue;

            angle = source.angle;
            online = source.online;
        }

        public void Update(Point_3d origin, Colour colour, int angle, int asset, bool online)
        {
            this.origin = origin;
            this.colour = colour;
            this.angle = angle;
            this.online = online;
            this.asset_index = asset;
        }

        public void Print()
        {
            Trace.WriteLine("ai: " + asset_index + ", angle: " + angle + ", online: " + (online ? "true" : "false"));
            origin.Print_Point();
            Trace.WriteLine(colour.ToString());
        }
    }
}