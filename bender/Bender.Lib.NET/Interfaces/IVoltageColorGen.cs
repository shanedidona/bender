using OpenCvSharp;

namespace Bender.Lib.NET
{
    public interface IVoltageColorGen
    {
        public Vec3b GenColor(double v);
    }
}
