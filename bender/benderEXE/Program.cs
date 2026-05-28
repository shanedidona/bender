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









        }
    }
}
