namespace Bender.Lib.NET
{
    public static class ElectrostaticGrid2DFactory
    {
        public static ElectrostaticGrid2D Gen1(double xMin, double yMin, int nx, int ny, double pixelSize, (double V, Region2D ElectrodeRegion2D)[] voltagesAndRegions, double epsilonOverPixelSize = 0.001)//First Gets Priority
        {
            var out1 = new ElectrostaticGrid2D(xMin, yMin, nx, ny, pixelSize);

            var xs = new double[nx];
            for (int i = 0; i < nx; i++)
            {
                xs[i] = xMin + i * pixelSize;
            }

            var ys = new double[ny];
            for (int i = 0; i < ny; i++)
            {
                ys[i] = yMin + i * pixelSize;
            }

            double epsilon = pixelSize * epsilonOverPixelSize;

            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    foreach ((double v, Region2D electrodeRegion2D) in voltagesAndRegions)
                    {
                        if (electrodeRegion2D.IsIn(xs[i], ys[j], epsilon))
                        {
                            out1.V[i, j] = v;
                            out1.ID[i, j] = 1;
                            break;
                        }
                    }
                }
            }

            return out1;
        }
    }
}
