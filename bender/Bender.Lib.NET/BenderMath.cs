using OpenCvSharp;
using System.Diagnostics;

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

            for (int i = 0; i < electrostaticGrid2D.NX; i++)
            {
                for (int j = 0; j < electrostaticGrid2D.NY; j++)
                {
                    int pi = electrostaticGrid2D.NY - j - 1;
                    int pj = i;

                    if (electrostaticGrid2D.ID[i, j] == 0)
                    {
                        vec3BArray[pi, pj] = voltageColorGen.GenColor(electrostaticGrid2D.V[i, j]);
                    }
                    else
                    {
                        vec3BArray[pi, pj] = new Vec3b(0, 0, 0);
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
                                vec3BArray[pi, pj] = equipotentialDraw2DSpec.BGRArray[k];
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

        public static (double[] MeanAbsChangeArray, bool Finished) SolveField2(
                ElectrostaticGrid2D electrostaticGrid2D,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries
            )//TODO:  this will be slow and replaced with a version that does demagnified versions first
        {
            return Solve2DFieldSingleStage2(electrostaticGrid2D.V, electrostaticGrid2D.ID, relaxationFactor, meanAbsChangeStop, maxTries);
        }

        public static (double[] MeanAbsChangeArray, bool Finished) Solve2DFieldSingleStage(
                double[,] v,
                ushort[,] id,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries
            )
        {
            Stopwatch sw1 = Stopwatch.StartNew();

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
                //Serilog.Log.Information("meanAbsResid {meanAbsResid}", meanAbsResid);

                out1List.Add(meanAbsResid);
                numResid = 0;
                residAbsSum = 0;

                if (meanAbsResid < meanAbsChangeStop)
                {
                    double[] out1Array1 = out1List.ToArray();

                    Serilog.Log.Information("Solve2DFieldSingleStage took {timeMS} ms", sw1.ElapsedMilliseconds);

                    return (out1Array1, true);
                }
            }

            double[] out1Array = out1List.ToArray();

            Serilog.Log.Information("Solve2DFieldSingleStage took {timeMS} ms", sw1.ElapsedMilliseconds);

            return (out1Array, false);
        }

        public static (double[] MeanAbsChangeArray, bool Finished) Solve2DFieldSingleStage2(
                double[,] v,
                ushort[,] id,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries
            )
        {
            Stopwatch sw1 = Stopwatch.StartNew();

            int nx = v.GetLength(0);
            int ny = v.GetLength(1);

            double oneOver3 = 1.0 / 3.0;

            var out1List = new List<double>();
            for (int tryI = 0; tryI < maxTries; tryI++)
            {
                #region Middle
                double residAbsSum = 0;
                int numResid = 0;
                for (int i = 1; i < nx - 1; i++)
                {
                    for (int j = 1; j < ny - 1; j++)
                    {
                        if (id[i, j] == 0)
                        {
                            double neighborMean = 0.25 * (v[i - 1, j] + v[i + 1, j] + v[i, j - 1] + v[i, j + 1]);
                            double residual = v[i, j] - neighborMean;

                            residAbsSum += Math.Abs(residual);
                            numResid++;
                            v[i, j] -= relaxationFactor * residual;
                        }
                    }
                }
                #endregion

                #region i==0 Edge (but not corners)
                for (int j = 1; j < ny - 1; j++)
                {
                    int i = 0;
                    if (id[i, j] == 0)
                    {
                        double neighborMean = oneOver3 * (v[i, j - 1] + v[i, j + 1] + v[i + 1, j]);
                        double residual = v[i, j] - neighborMean;

                        residAbsSum += Math.Abs(residual);
                        numResid++;
                        v[i, j] -= relaxationFactor * residual;
                    }
                }
                #endregion

                #region i==nx-1 Edge (but not corners)
                for (int j = 1; j < ny - 1; j++)
                {
                    int i = nx - 1;
                    if (id[i, j] == 0)
                    {
                        double neighborMean = oneOver3 * (v[i, j - 1] + v[i, j + 1] + v[i - 1, j]);
                        double residual = v[i, j] - neighborMean;

                        residAbsSum += Math.Abs(residual);
                        numResid++;
                        v[i, j] -= relaxationFactor * residual;
                    }
                }
                #endregion

                #region j==0 Edge (but not corners)
                for (int i = 1; i < nx - 1; i++)
                {
                    int j = 0;
                    if (id[i, j] == 0)
                    {
                        double neighborMean = oneOver3 * (v[i - 1, j] + v[i + 1, j] + v[i, j + 1]);
                        double residual = v[i, j] - neighborMean;

                        residAbsSum += Math.Abs(residual);
                        numResid++;
                        v[i, j] -= relaxationFactor * residual;
                    }
                }
                #endregion

                #region j==ny-1 Edge (but not corners)
                for (int i = 1; i < nx - 1; i++)
                {
                    int j = ny - 1;
                    if (id[i, j] == 0)
                    {
                        double neighborMean = oneOver3 * (v[i - 1, j] + v[i + 1, j] + v[i, j - 1]);
                        double residual = v[i, j] - neighborMean;

                        residAbsSum += Math.Abs(residual);
                        numResid++;
                        v[i, j] -= relaxationFactor * residual;
                    }
                }
                #endregion

                #region Corners
                {
                    #region i==0; j==0
                    {
                        int i = 0;
                        int j = 0;
                        if (id[i, j] == 0)
                        {
                            double neighborMean = 0.5 * (v[i + 1, j] + v[i, j + 1]);
                            double residual = v[i, j] - neighborMean;

                            residAbsSum += Math.Abs(residual);
                            numResid++;
                            v[i, j] -= relaxationFactor * residual;
                        }
                    }
                    #endregion

                    #region i==nx-1; j==0
                    {
                        int i = nx - 1;
                        int j = 0;
                        if (id[i, j] == 0)
                        {
                            double neighborMean = 0.5 * (v[i - 1, j] + v[i, j + 1]);
                            double residual = v[i, j] - neighborMean;

                            residAbsSum += Math.Abs(residual);
                            numResid++;
                            v[i, j] -= relaxationFactor * residual;
                        }
                    }
                    #endregion

                    #region i==0; j==ny-1
                    {
                        int i = 0;
                        int j = ny - 1;
                        if (id[i, j] == 0)
                        {
                            double neighborMean = 0.5 * (v[i + 1, j] + v[i, j - 1]);
                            double residual = v[i, j] - neighborMean;

                            residAbsSum += Math.Abs(residual);
                            numResid++;
                            v[i, j] -= relaxationFactor * residual;
                        }
                    }
                    #endregion

                    #region i==nx-1; j==ny-1
                    {
                        int i = nx - 1;
                        int j = ny - 1;
                        if (id[i, j] == 0)
                        {
                            double neighborMean = 0.5 * (v[i - 1, j] + v[i, j - 1]);
                            double residual = v[i, j] - neighborMean;

                            residAbsSum += Math.Abs(residual);
                            numResid++;
                            v[i, j] -= relaxationFactor * residual;
                        }
                    }
                    #endregion
                }
                #endregion

                double meanAbsResid = residAbsSum / numResid;
                //Serilog.Log.Information("meanAbsResid {meanAbsResid}", meanAbsResid);

                out1List.Add(meanAbsResid);

                if (meanAbsResid < meanAbsChangeStop)
                {
                    double[] out1Array1 = out1List.ToArray();

                    Serilog.Log.Information("Solve2DFieldSingleStage took {timeMS} ms", sw1.ElapsedMilliseconds);

                    return (out1Array1, true);
                }
            }

            double[] out1Array = out1List.ToArray();

            Serilog.Log.Information("Solve2DFieldSingleStage took {timeMS} ms", sw1.ElapsedMilliseconds);

            return (out1Array, false);
        }
    }
}
