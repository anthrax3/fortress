using System;

namespace Castle.Core.Core.Internal
{
    public class TypeConverterAttribute : Attribute
    {
        public string ConverterTypeName { get; set; }
    }
}