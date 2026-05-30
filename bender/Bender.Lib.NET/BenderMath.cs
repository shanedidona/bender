using OpenCvSharp;

namespace Bender.Lib.NET
{
    public static class BenderMath
    {
        public static double Sq(double x)
        {
            return x * x;
        }

        public static Mat RenderMat(ElectrostaticGrid2D electrostaticGrid2D, IVoltageColorGen voltageColorGen)
        {
            var vec3BArray = new Vec3b[electrostaticGrid2D.NY, electrostaticGrid2D.NX];
            for (int i = 0; i < electrostaticGrid2D.NY; i++)
            {
                for (int j = 0; j < electrostaticGrid2D.NX; j++)
                {
                    if (electrostaticGrid2D.ID[j, electrostaticGrid2D.NY - i - 1] == 0)
                    {
                        vec3BArray[i, j] = voltageColorGen.GenColor(electrostaticGrid2D.V[j, electrostaticGrid2D.NY - i - 1]);
                    }
                    else
                    {
                        vec3BArray[i, j] = new Vec3b(0, 0, 0);
                    }
                }
            }

            return Mat.FromPixelData(electrostaticGrid2D.NY, electrostaticGrid2D.NX, MatType.CV_8UC3, vec3BArray);
        }
    }
}
