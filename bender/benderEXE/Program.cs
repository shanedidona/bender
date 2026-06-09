using Bender.Lib.NET;
using Serilog;

namespace benderEXE
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
