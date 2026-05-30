namespace Bender.Lib.NET
{
    public sealed class AButNotB2D : Region2D
    {
        readonly Region2D _a;
        readonly Region2D _b;

        readonly Rectangle _looseBoundingRectangle;

        public AButNotB2D(Region2D a, Region2D b)
        {
            _a = a;
            _b = b;

            _looseBoundingRectangle = _a.LooseBoundingRectangle;
        }

        public override Rectangle LooseBoundingRectangle => _looseBoundingRectangle;

        public override bool IsIn(double x, double y, double dilation)
        {
            if (!_looseBoundingRectangle.IsIn(x,y,dilation)) { return false; }

            return _a.IsIn(x, y, dilation) && (!_b.IsIn(x, y, -dilation));//Notice the minus before dilation
        }
    }
}
