using System;
using Castle.Core.Core.Configuration;

namespace Castle.Windsor
{
    public class ZeroConfiguration : IConfiguration
    {
        public static ZeroConfiguration Instance = new ZeroConfiguration();

        public string Name { get; }
        public string Value { get; }
        public ConfigurationCollection Children { get; }
        public ConfigurationAttributeCollection Attributes { get; }
        public object GetValue(Type type, object defaultValue)
        {
            return defaultValue;
        }
    }
}