namespace WinFormsApp1
{
    public static class Utility
    {
        public static double Cos(double angle)
        {
            return Math.Cos(angle * (Math.PI / 180));
        }
        public static double Sin(double angle)
        {
            return Math.Sin(angle * (Math.PI / 180));
        }
        public static double Tan(double angle)
        {
            return Math.Tan(angle * (Math.PI / 180));
        }
        public static double Atan(double opposite, double adjacent)
        {
            return Math.Atan(opposite / adjacent) * (180 / Math.PI);
        }

        public static double Min(params double[] numbers)
        {
            double min = numbers[0];
            foreach (double number in numbers)
            {
                //Trace.WriteLine("option " + number.ToString());
                min = Math.Min(min, number);
            }
            //Trace.WriteLine("final " + min.ToString());
            return min;
        }

        public static double Max(params double[] numbers)
        {
            double max = numbers[0];
            foreach (double number in numbers)
            {
                max = Math.Max(max, number);
            }
            return max;
        }

        /*public static int MinI(params int[] numbers)
        {
            int min = numbers[0];
            foreach (int number in numbers)
            {
                //Trace.WriteLine("option " + number.ToString());
                min = Math.Min(min, number);
            }
            //Trace.WriteLine("final " + min.ToString());
            return min;
        }

        public static int MaxI(params int[] numbers)
        {
            int max = numbers[0];
            foreach (int number in numbers)
            {
                max = Math.Max(max, number);
            }
            return max;
        }*/
    }
}
