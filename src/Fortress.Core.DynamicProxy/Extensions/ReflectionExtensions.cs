using System;
using System.Linq;
using System.Reflection;

namespace Castle.DynamicProxy.Extensions
{
    public static class ReflectionExtensions
    {
        public static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingAttr, object binder, Type[] types, object[] modifiers)
        {
            if (binder != null) throw new NotSupportedException("Parameter binder must be null.");
            if (modifiers != null) throw new NotSupportedException("Parameter modifiers must be null.");

            return type.GetTypeInfo()
                .GetConstructors(bindingAttr)
                .SingleOrDefault(ctor => ctor.GetParameters().Select(p => p.ParameterType)
                .SequenceEqual(types));
        }
    }
}