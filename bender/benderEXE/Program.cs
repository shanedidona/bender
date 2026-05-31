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

            var solve1Var = BenderMath.SolveField(electrostaticGrid2D, 1.5, 1E-9, 1_000_000_000);

            string resultsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bender", "Results");
            Directory.CreateDirectory(resultsFolder);

            IVoltageColorGen voltageColorGen = new VoltageColorGen2Color(-1, 1, 255, 0, 0, 0, 0, 255);

            BenderMath.RenderMat(electrostaticGrid2D, voltageColorGen).SaveImage(Path.Combine(resultsFolder, "1.png"));
        }
    }
}
