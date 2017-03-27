namespace Castle.MicroKernel
{
    // This should come out of the component model but has not been ported to core yet
    public interface ISupportInitialize
    {
        void BeginInit();
        void EndInit();
    }
}