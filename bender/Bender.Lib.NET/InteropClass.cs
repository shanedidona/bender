using System.Runtime.InteropServices;

namespace Bender.Lib.NET.Interop
{
    public static class InteropClass
    {
        [DllImport("Bender.Lib.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Add(int a, int b);
    }
}
