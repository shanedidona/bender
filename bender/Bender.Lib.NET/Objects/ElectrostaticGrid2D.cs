namespace Bender.Lib.NET
{
    public sealed class ElectrostaticGrid2D
    {
        public readonly double XMin;
        public readonly double YMin;
        public readonly int NX;
        public readonly int NY;
        public readonly double PixelSize;
        public double[,] V;//[xi,yi]
        public ushort[,] ID;//[xi,yi]

        public ElectrostaticGrid2D(double xMin, double yMin, int nx, int ny, double pixelSize)
        {
            if (nx < 3) { throw new NotSupportedException("nx < 3"); }
            if (ny < 3) { throw new NotSupportedException("ny < 3"); }
            if (pixelSize <= 0) { throw new NotSupportedException("pixelSize <= 0"); }

            XMin = xMin;
            YMin = yMin;
            NX = nx;
            NY = ny;
            PixelSize = pixelSize;
            V = new double[nx, ny];
            ID = new ushort[nx, ny];
        }

        public (double VMin, double VMax) PresentMinMaxV()
        {
            double vMin = V[0, 0];
            double vMax = V[0, 0];


            for (int i = 0; i < NX; i++)
            {
                for (int j = 0; j < NY; j++)
                {
                    vMin = Math.Min(vMin, V[i, j]);
                    vMax = Math.Max(vMax, V[i, j]);
                }
            }

            return (vMin, vMax);
        }

        public HashSet<ushort> PresentIDs()
        {
            var out1 = new HashSet<ushort>();
            for (int i = 0; i < NX; i++)
            {
                for (int j = 0; j < NY; j++)
                {
                    out1.Add(ID[i, j]);
                }
            }

            return out1;
        }
    }
}
