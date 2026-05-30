namespace Bender.Lib.NET
{
    public abstract class Region2D
    {
        public abstract bool IsIn(double x, double y, double epsilon);//Errs on the side of being in vs out.

        public abstract Rectangle LooseBoundingRectangle { get; }//Can be bigger than true bounding rectangle but not smaller in any way; used to accelerate IsIn.
    }
}
