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

        public override Rectangle LooseBoundingRectangle => this;

        public override bool IsIn(double x, double y, double epsilon)
        {
            if (x < XMin - epsilon) { return false; }
            if (y < YMin - epsilon) { return false; }

            if (XMax + epsilon < x) { return false; }
            if (YMax + epsilon < y) { return false; }

            return true;
        }

        public static Rectangle CalculateBoundingBox(Rectangle[] rectangles)
        {
            if (rectangles.Length == 0)
            {
                throw new NotSupportedException("rectangles.Length == 0");
            }

            double xMin = rectangles[0].XMin;
            double yMin = rectangles[0].YMin;
            double xMax = rectangles[0].XMax;
            double yMax = rectangles[0].YMax;

            for (int i = 1; i < rectangles.Length; i++)
            {
                xMin = Math.Min(xMin, rectangles[i].XMin);
                yMin = Math.Min(yMin, rectangles[i].YMin);

                xMax = Math.Max(xMax, rectangles[i].XMax);
                yMax = Math.Max(yMax, rectangles[i].YMax);
            }

            return new Rectangle(xMin, yMin, xMax, yMax);
        }
    }
}
