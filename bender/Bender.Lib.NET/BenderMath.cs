using OpenCvSharp;

namespace Bender.Lib.NET
{
    public static class BenderMath
    {
        public static double Sq(double x)
        {
            return x * x;
        }

        public static Mat RenderMat(ElectrostaticGrid2D electrostaticGrid2D, IVoltageColorGen voltageColorGen, EquipotentialDraw2DSpec equipotentialDraw2DSpec)
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












                    var neighborVs = new List<double>();
                    if (i != 0) { neighborVs.Add(electrostaticGrid2D.V[i - 1, j]); }
                    if (j != 0) { neighborVs.Add(electrostaticGrid2D.V[i, j - 1]); }
                    if (i != electrostaticGrid2D.NX - 1) { neighborVs.Add(electrostaticGrid2D.V[i + 1, j]); }
                    if (j != electrostaticGrid2D.NY - 1) { neighborVs.Add(electrostaticGrid2D.V[i, j + 1]); }

                    for (int k = 0; k < equipotentialDraw2DSpec.BGRArray.Length; k++)
                    {
                        bool drewPixel = false;
                        foreach (double neighborV in neighborVs)
                        {
                            double min1 = Math.Min(electrostaticGrid2D.V[i, j], neighborV);
                            double max1 = Math.Max(electrostaticGrid2D.V[i, j], neighborV);

                            if (min1 <= equipotentialDraw2DSpec.Vs[k] && equipotentialDraw2DSpec.Vs[k] <= max1)
                            {
                                vec3BArray[i, j] = equipotentialDraw2DSpec.BGRArray[k];
                            }
                        }

                        if (drewPixel)
                        {
                            break;
                        }
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
            return Solve2DFieldSingleStage(electrostaticGrid2D.V, electrostaticGrid2D.ID, relaxationFactor, meanAbsChangeStop, maxTries);
        }

        public static (double[] MeanAbsChangeArray, bool Finished) Solve2DFieldSingleStage(
                double[,] v,
                ushort[,] id,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries
            )
        {
            int nx = v.GetLength(0);
            int ny = v.GetLength(1);

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
