namespace Castle.Compatibility
{
#if !FEATURE_SYSTEM_CONVERTER
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
#endif
}