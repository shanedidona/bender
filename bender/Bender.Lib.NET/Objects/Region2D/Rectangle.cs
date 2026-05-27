namespace Bender.Lib.NET
{
    public sealed class Rectangle : Region2D
    {
        public readonly double XMin;
        public readonly double YMin;
        public readonly double XMax;
        public readonly double YMax;

        public Rectangle(double x1, double y1, double x2, double y2)
        {
            XMin = Math.Min(x1, x2);
            YMin = Math.Min(y1, y2);

            XMax = Math.Max(x1, x2);
            YMax = Math.Max(y1, y2);
        }

        public override bool IsIn(double x, double y, double epsilon)
        {
            if (x < XMin - epsilon) { return false; }
            if (y < YMin - epsilon) { return false; }

            if (XMax + epsilon < x) { return false; }
            if (YMax + epsilon < y) { return false; }

            return true;
        }
    }
}
