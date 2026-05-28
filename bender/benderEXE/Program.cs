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
            int nx = 1000;
            int ny = 1000;
            double pixelSize = 0.001;

            var voltagesAndRegions = new List<(double V, Region2D ElectrodeRegion2D)>();
            voltagesAndRegions.Add((0, new Circle(0.1, 0.2, 0.02)));

            var wooo = ElectrostaticGrid2DFactory.Gen1(xMin, yMin, nx, ny, pixelSize, voltagesAndRegions.ToArray());











        }
    }
}
