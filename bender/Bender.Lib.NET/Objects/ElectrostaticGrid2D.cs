namespace Bender.Lib.NET
{
    public sealed class ElectrostaticGrid2D
    {
        public readonly double XMin;
        public readonly double YMin;
        public double PixelSize;
        public double[,] V;
        public ushort[,] ID;

        public ElectrostaticGrid2D(double xMin, double yMin, int nx, int ny, double pixelSize)
        {
            if (nx < 3) { throw new NotSupportedException("nx < 3"); }
            if (ny < 3) { throw new NotSupportedException("ny < 3"); }
            if (pixelSize <= 0) { throw new NotSupportedException("pixelSize <= 0"); }

            XMin = xMin;
            YMin = yMin;
            PixelSize = pixelSize;
            V = new double[nx, ny];
            ID = new ushort[nx, ny];
        }
    }
}
