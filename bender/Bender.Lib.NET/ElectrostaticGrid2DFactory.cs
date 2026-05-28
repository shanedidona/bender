namespace Bender.Lib.NET
{
    public static class ElectrostaticGrid2DFactory
    {
        public static ElectrostaticGrid2D Gen1(double xMin, double yMin, int nx, int ny, double pixelSize, (ushort ID, Region2D ElectrodeRegion2D)[] idsAndRegions)//First Gets Priority; id can be duplicated and there can be gaps in id.  ID cannot be 0.
        {
            if (idsAndRegions.Select(x => x.ID).Contains((ushort)0))
            {
                throw new NotSupportedException("No Electrode can have ID==0; that is reserved for vacuum.");
            }

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

            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    foreach ((ushort id, Region2D electrodeRegion2D) in idsAndRegions)
                    {
                        if ()
                        {

                        }



                    }
                }
            }











            return out1;
        }
    }
}
