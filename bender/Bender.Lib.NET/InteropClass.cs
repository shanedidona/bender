using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bender.Lib.NET.Interop
{
    public static unsafe class InteropClass
    {
        [DllImport("Bender.Lib.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Add(int a, int b);

        [DllImport("Bender.Lib.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Solve1(
                double* v,
                ushort* id,
                int nx,
                int ny,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries,
                double** outMeanAbsChangeArray,
                int* outMeanAbsChangeArrayLen);

        [DllImport("Bender.Lib.Native", CallingConvention = CallingConvention.Cdecl)]
        static extern void DeleteDoubleArray(double* pointer);

        public static (double[] MeanAbsChangeArray, bool Finished) Solve2DFieldSingleStageCPP(
                double[,] v,
                ushort[,] id,
                double relaxationFactor,
                double meanAbsChangeStop,
                int maxTries
            )
        {
            Stopwatch sw1 = Stopwatch.StartNew();

            int nx = v.GetLength(0);
            int ny = v.GetLength(1);

            double* meanAbsChangeArrayPtr = null;
            int meanAbsChangeArrayLen = 0;
            int finishedInt;


            fixed (double* pv = v)
            {
                fixed (ushort* pid = id)
                {
                    finishedInt = Solve1(pv, pid, nx, ny, relaxationFactor, meanAbsChangeStop, maxTries, &meanAbsChangeArrayPtr, &meanAbsChangeArrayLen);
                }
            }

            var meanAbsChangeArray = new double[meanAbsChangeArrayLen];
            Marshal.Copy((IntPtr)meanAbsChangeArrayPtr, meanAbsChangeArray, 0, meanAbsChangeArrayLen);
            DeleteDoubleArray(meanAbsChangeArrayPtr);

            Serilog.Log.Information("Solve2DFieldSingleStage took {timeMS} ms", sw1.ElapsedMilliseconds);

            return (meanAbsChangeArray, finishedInt != 0);
        }
    }
}
