using OpenCvSharp;

namespace Bender.Lib.NET
{
    public sealed class VoltageColorGen2Color : IVoltageColorGen
    {
        readonly double _vMin;
        readonly double _vMax;
        readonly double _oneOverVRange;

        readonly byte _rMin;
        readonly byte _gMin;
        readonly byte _bMin;

        readonly byte _rMax;
        readonly byte _gMax;
        readonly byte _bMax;

        public VoltageColorGen2Color(double vMin, double vMax, byte rMin, byte gMin, byte bMin, byte rMax, byte gMax, byte bMax)
        {
            if (vMax <= vMin)
            {
                throw new NotSupportedException("vMax <= vMin");
            }

            _vMin = vMin;
            _vMax = vMax;
            _oneOverVRange = 1.0 / (vMax - vMin);

            _rMin = rMin;
            _gMin = gMin;
            _bMin = bMin;

            _rMax = rMax;
            _gMax = gMax;
            _bMax = bMax;
        }

        public Vec3b GenColor(double v)
        {
            if (v <= _vMin) { return new Vec3b(_bMin, _gMin, _rMin); }
            if (_vMax <= v) { return new Vec3b(_bMax, _gMax, _rMax); }

            double vRel = _oneOverVRange * (v - _vMin);

            byte r = Convert.ToByte((1 - vRel) * _rMin + vRel * _rMax);
            byte g = Convert.ToByte((1 - vRel) * _gMin + vRel * _gMax);
            byte b = Convert.ToByte((1 - vRel) * _bMin + vRel * _bMax);

            return new Vec3b(b, g, r);
        }
    }
}
