using OpenCvSharp;

namespace Bender.Lib.NET
{
    public sealed class EquipotentialDraw2DSpec
    {
        public double[] Vs;
        public Vec3b[] BGRArray;

        public EquipotentialDraw2DSpec(double[] vs, Vec3b[] bgrArray)
        {
            if (vs.Length != bgrArray.Length) { throw new NotSupportedException("vs.Length != bgrArray.Length"); }

            if (vs.ToHashSet().Count != vs.Length) { throw new NotSupportedException("Duplicate vs Detected"); }

            Vs = vs;
            BGRArray = bgrArray;
        }
    }
}
