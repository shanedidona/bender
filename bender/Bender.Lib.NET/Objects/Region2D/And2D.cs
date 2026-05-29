namespace Bender.Lib.NET
{
    public sealed class And2D : Region2D
    {
        readonly Region2D[] _regions;

        readonly Rectangle _looseBoundingRectangle;

        public And2D(Region2D[] regions)
        {
            if (regions.Length == 0)
            {
                throw new NotSupportedException("!regions.Any() is not supported for now.");
            }

            _regions = regions;

            _looseBoundingRectangle = Rectangle.CalculateBoundingBox(_regions.Select(x => x.LooseBoundingRectangle).ToArray());
        }

        public override Rectangle LooseBoundingRectangle => _looseBoundingRectangle;

        public override bool IsIn(double x, double y, double epsilon)
        {
            foreach (Region2D region in _regions)
            {
                if (!region.IsIn(x, y, epsilon))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
