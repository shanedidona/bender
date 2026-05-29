using static Bender.Lib.NET.BenderMath;

namespace Bender.Lib.NET
{
    public sealed class Circle : Region2D
    {
        public readonly double XC;
        public readonly double YC;
        public readonly double R;

        readonly Rectangle BoundingRectangle;

        public Circle(double xC, double yC, double r)
        {
            XC = xC;
            YC = yC;
            R = r;

            BoundingRectangle = new Rectangle(XC - R, YC - R, XC + R, YC + R);
        }

        public override bool IsIn(double x, double y, double epsilon)
        {
            if (!BoundingRectangle.IsIn(x, y, epsilon)) { return false; }

            if (Sq(R + epsilon) < Sq(x - XC) + Sq(y - YC))
            {
                return false;
            }

            return true;
        }
    }
}
