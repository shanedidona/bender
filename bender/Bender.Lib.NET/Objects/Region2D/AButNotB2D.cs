namespace Bender.Lib.NET
{
    public sealed class AButNotB2D : Region2D
    {
        readonly Region2D _a;
        readonly Region2D _b;

        public AButNotB2D(Region2D a, Region2D b)
        {
            _a = a;
            _b = b;
        }

        public override Rectangle LooseBoundingRectangle => _a.LooseBoundingRectangle;

        public override bool IsIn(double x, double y, double epsilon)
        {
            return _a.IsIn(x, y, epsilon) && (!_b.IsIn(x, y, epsilon));
        }
    }
}
