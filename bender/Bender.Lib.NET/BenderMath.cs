using OpenCvSharp;

namespace Bender.Lib.NET
{
    public static class BenderMath
    {
        public static double Sq(double x)
        {
            return x * x;
        }

        public static Mat RenderMat(ElectrostaticGrid2D electrostaticGrid2D, IVoltageColorGen voltageColorGen)
        {
            var vec3BArray = new Vec3b[electrostaticGrid2D.NY, electrostaticGrid2D.NX];
            for (int i = 0; i < electrostaticGrid2D.NY; i++)
            {
                for (int j = 0; j < electrostaticGrid2D.NX; j++)
                {
                    if (electrostaticGrid2D.ID[j, electrostaticGrid2D.NY - i - 1] == 0)
                    {
                        vec3BArray[i, j] = voltageColorGen.GenColor(electrostaticGrid2D.V[j, electrostaticGrid2D.NY - i - 1]);
                    }
                    else
                    {
                        vec3BArray[i, j] = new Vec3b(0, 0, 0);
                    }
                }
            }

            return Mat.FromPixelData(electrostaticGrid2D.NY, electrostaticGrid2D.NX, MatType.CV_8UC3, vec3BArray);
        }

        public static (double[] MeanAbsChangeArray, bool Finished) SolveField(
                ElectrostaticGrid2D electrostaticGrid2D,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries
            )//TODO:  this will be slow and replaced with a version that does demagnified versions first
        {
            return Solve2DFieldSingleStage(electrostaticGrid2D, relaxationFactor, meanAbsChangeStop, maxTries);
        }

        public static (double[] MeanAbsChangeArray, bool Finished) Solve2DFieldSingleStage(
                ElectrostaticGrid2D electrostaticGrid2D,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries
            )
        {
            double[,] v = electrostaticGrid2D.V;
            ushort[,] id = electrostaticGrid2D.ID;
            int nx = electrostaticGrid2D.NX;
            int ny = electrostaticGrid2D.NY;

            var out1List = new List<double>();
            double residAbsSum = 0;
            int numResid = 0;
            for (int tryI = 0; tryI < maxTries; tryI++)
            {
                for (int i = 0; i < nx; i++)
                {
                    for (int j = 0; j < ny; j++)
                    {
                        if (id[i, j] == 0)
                        {
                            int numNeighbors = 0;
                            double neighborSum = 0;

                            if (i != 0)
                            {
                                numNeighbors++;
                                neighborSum += v[i - 1, j];
                            }

                            if (i != nx - 1)
                            {
                                numNeighbors++;
                                neighborSum += v[i + 1, j];
                            }

                            if (j != 0)
                            {
                                numNeighbors++;
                                neighborSum += v[i, j - 1];
                            }

                            if (j != ny - 1)
                            {
                                numNeighbors++;
                                neighborSum += v[i, j + 1];
                            }

                            double neighborMean = neighborSum / numNeighbors;

                            double residual = v[i, j] - neighborMean;

                            numResid++;
                            residAbsSum += Math.Abs(residual);

                            v[i, j] -= relaxationFactor * residual;
                        }
                    }
                }

                double meanAbsResid = residAbsSum / numResid;
                Serilog.Log.Information("meanAbsResid {meanAbsResid}", meanAbsResid);

                out1List.Add(meanAbsResid);
                numResid = 0;
                residAbsSum = 0;

                if (meanAbsResid < meanAbsChangeStop)
                {
                    return (out1List.ToArray(), true);
                }
            }

            return (out1List.ToArray(), false);
        }
    }
}
