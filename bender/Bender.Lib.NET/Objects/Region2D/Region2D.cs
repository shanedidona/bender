namespace Bender.Lib.NET
{
    public abstract class Region2D
    {
        public abstract bool IsIn(double epsilon);//Errs on the side of being in vs out.
    }
}
