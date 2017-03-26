namespace Castle.Windsor.Compatibility
{
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
}