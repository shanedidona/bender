using Bender.Lib.NET;
using Bender.Lib.NET.Interop;
using Serilog;

namespace benderEXE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[] xs = Enumerable.Range(4, 1000).Select(x => 1.0 * x).ToArray();
            double[] ys = new double[xs.Length];
            for (int i = 0; i < xs.Length; i++)
            {
                double h = 1.0 / (xs[i] + 1.0);
                ys[i] = 2.0 / (1.0 + Math.Sin(Math.PI * h));
            }

            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.Add.Scatter(xs, ys);
            plot.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bender", "Results", "omega.png"), 1920, 1080);




            return;

            int c = InteropClass.Add(2, 3);


            string template = "[{Timestamp:HH:mm:ss.fff} {Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: template)
                //.WriteTo.File(outputTemplate: template, path: "", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Logger made");

            RelaxFactorExplore();

            double xMin = 0;
            double yMin = 0;
            int nx = 200;
            int ny = 300;
            double pixelSize = 0.005;

            var voltagesAndRegions = new List<(double V, Region2D ElectrodeRegion2D)>();
            voltagesAndRegions.Add((0, new Circle(0.1, 0.2, 0.02)));
            voltagesAndRegions.Add((1, new Circle(0.1, 0.4, 0.02)));
            voltagesAndRegions.Add((-0.5, new Rectangle(0.5, 0.5, 0.6, 0.7)));

            ElectrostaticGrid2D electrostaticGrid2D = ElectrostaticGrid2DFactory.Gen1(xMin, yMin, nx, ny, pixelSize, voltagesAndRegions.ToArray());

            var solve1Var = BenderMath.SolveField(electrostaticGrid2D, 1.8, 1E-9, 1_000_000_000);

            ElectrostaticGrid2D electrostaticGrid2D2 = ElectrostaticGrid2DFactory.Gen1(xMin, yMin, nx, ny, pixelSize, voltagesAndRegions.ToArray());

            var solve2Var = BenderMath.SolveField2(electrostaticGrid2D2, 1.8, 1E-9, 1_000_000_000);

            ElectrostaticGrid2D electrostaticGrid2DCPP = ElectrostaticGrid2DFactory.Gen1(xMin, yMin, nx, ny, pixelSize, voltagesAndRegions.ToArray());

            var solveCPPVar = BenderMath.SolveFieldCPP(electrostaticGrid2DCPP, 1.8, 1E-9, 1_000_000_000);

            double totalAbsDiff = 0;
            double maxAbsDiff = 0;
            double totalAbsDiffCPP = 0;
            double maxAbsDiffCPP = 0;
            double totalAbsDiffCPP2 = 0;
            double maxAbsDiffCPP2 = 0;
            for (int i = 0; i < electrostaticGrid2D.V.GetLength(0); i++)
            {
                for (int j = 0; j < electrostaticGrid2D.V.GetLength(1); j++)
                {
                    totalAbsDiff += Math.Abs(electrostaticGrid2D.V[i, j] - electrostaticGrid2D2.V[i, j]);
                    maxAbsDiff = Math.Max(maxAbsDiff, Math.Abs(electrostaticGrid2D.V[i, j] - electrostaticGrid2D2.V[i, j]));

                    totalAbsDiffCPP += Math.Abs(electrostaticGrid2D.V[i, j] - electrostaticGrid2DCPP.V[i, j]);
                    maxAbsDiffCPP = Math.Max(maxAbsDiffCPP, Math.Abs(electrostaticGrid2D.V[i, j] - electrostaticGrid2DCPP.V[i, j]));

                    totalAbsDiffCPP2 += Math.Abs(electrostaticGrid2D2.V[i, j] - electrostaticGrid2DCPP.V[i, j]);
                    maxAbsDiffCPP2 = Math.Max(maxAbsDiffCPP2, Math.Abs(electrostaticGrid2D2.V[i, j] - electrostaticGrid2DCPP.V[i, j]));
                }
            }

            Serilog.Log.Information("totalAbsDiff = " + totalAbsDiff);
            Serilog.Log.Information("maxAbsDiff = " + maxAbsDiff);

            Serilog.Log.Information("totalAbsDiffCPP = " + totalAbsDiffCPP);
            Serilog.Log.Information("maxAbsDiffCPP = " + maxAbsDiffCPP);

            Serilog.Log.Information("totalAbsDiffCPP2 = " + totalAbsDiffCPP2);
            Serilog.Log.Information("maxAbsDiffCPP2 = " + maxAbsDiffCPP2);

            string resultsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bender", "Results");
            Directory.CreateDirectory(resultsFolder);

            IVoltageColorGen voltageColorGen = new VoltageColorGen2Color(-1, 1, 255, 0, 0, 0, 0, 255);

            EquipotentialDraw2DSpec equipotentialDraw2DSpec = new EquipotentialDraw2DSpec(
                    new double[] {-0.4,-0.3,0},
                    new OpenCvSharp.Vec3b[] {new OpenCvSharp.Vec3b(128,128,0), new OpenCvSharp.Vec3b(60,60,60) , new OpenCvSharp.Vec3b(0, 255, 0) }
                );

            BenderMath.RenderMat(electrostaticGrid2D, voltageColorGen, equipotentialDraw2DSpec).SaveImage(Path.Combine(resultsFolder, "1.png"));

            Thread.Sleep(1000);
        }

        static void RelaxFactorExplore()
        {
            DBFileManager1D1D dBFileManager1D1D = new DBFileManager1D1D(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bender", "Results", "relaxationFactor.csv"));

            var relaxFactors = new HashSet<double>();
            relaxFactors.Add(1.5);
            for (int i = 1; i <= 9; i++)
            {
                relaxFactors.Add(Math.Round(1 + 0.1 * i,3));
            }

            for (int i = 1; i <= 9; i++)
            {
                relaxFactors.Add(Math.Round(1.9 + 0.01 * i, 3));
            }

            for (int i = 1; i <= 9; i++)
            {
                relaxFactors.Add(Math.Round(1.99 + 0.001 * i, 3));
            }

            for (int i = 1; i <= 9; i++)
            {
                relaxFactors.Add(Math.Round(1.999 + 0.0001 * i, 4));
            }

            relaxFactors.Add(1.01);
            relaxFactors.Add(1.001);
            relaxFactors.Add(1.0001);
            relaxFactors.Add(1.00001);
            relaxFactors.Add(1.000001);
            relaxFactors.Add(1.0000001);

            for (double x = 1.94; x <= 1.985; x += 0.00001)
            {
                relaxFactors.Add(Math.Round(x, 5));
            }

            for (double x = 1.001; x <= 1.999; x += 0.001)
            {
                relaxFactors.Add(Math.Round(x, 3));
            }

            foreach (double relaxFactor in relaxFactors)
            {
                if (dBFileManager1D1D.ContainsKey(relaxFactor))
                {
                    continue;
                }

                double xMin = 0;
                double yMin = 0;
                int nx = 100;
                int ny = 100;
                double pixelSize = 0.01;

                var voltagesAndRegions = new List<(double V, Region2D ElectrodeRegion2D)>();
                voltagesAndRegions.Add((0, new Circle(0.1, 0.2, 0.02)));
                voltagesAndRegions.Add((1, new Circle(0.1, 0.4, 0.02)));
                voltagesAndRegions.Add((-0.5, new Rectangle(0.5, 0.5, 0.6, 0.7)));

                ElectrostaticGrid2D electrostaticGrid2D = ElectrostaticGrid2DFactory.Gen1(xMin, yMin, nx, ny, pixelSize, voltagesAndRegions.ToArray());

                var solve1Var = BenderMath.SolveField(electrostaticGrid2D, relaxFactor, 1E-9, 1_000_000_000);

                dBFileManager1D1D.Add(relaxFactor, solve1Var.MeanAbsChangeArray.Length);
            }
        }
    }
}
