namespace Bender.Lib.NET
{
    public sealed class Or2D : Region2D
    {
        readonly Region2D[] _regions;

        public Or2D(Region2D[] regions)
        {
            if (regions.Length == 0)
            {
                throw new NotSupportedException("!regions.Any() is not supported for now.");
            }

            _regions = regions;
        }

        public override bool IsIn(double x, double y, double epsilon)
        {
            foreach (Region2D region in _regions)
            {
                if (region.IsIn(x, y, epsilon))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
