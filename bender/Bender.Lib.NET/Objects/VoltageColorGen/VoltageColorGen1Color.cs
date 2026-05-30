using OpenCvSharp;

namespace Bender.Lib.NET
{
    public sealed class VoltageColorGen1Color : IVoltageColorGen
    {
        readonly byte _r;
        readonly byte _g;
        readonly byte _b;

        public VoltageColorGen1Color(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
        }

        public Vec3b GenColor(double v)
        {
            return new Vec3b(_b, _g, _r);
        }
    }
}
