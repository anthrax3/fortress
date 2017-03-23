using System;
using Castle.Windsor.Core;

namespace Castle.Windsor.MicroKernel.Registration
{
	public class PropertyKey
	{
		internal PropertyKey(object key)
		{
			Key = key;
		}

		public object Key { get; }

		public Property Eq(object value)
		{
			return new Property(Key, value);
		}

		public ServiceOverride Is(string componentName)
		{
			return GetServiceOverrideKey().Eq(componentName);
		}

		public ServiceOverride Is(Type componentImplementation)
		{
			if (componentImplementation == null)
				throw new ArgumentNullException("componentImplementation");
			return GetServiceOverrideKey().Eq(ComponentName.DefaultNameFor(componentImplementation));
		}

		public ServiceOverride Is<TComponentImplementation>()
		{
			return Is(typeof(TComponentImplementation));
		}

		private ServiceOverrideKey GetServiceOverrideKey()
		{
			if (Key is Type)
				return ServiceOverride.ForKey((Type) Key);
			return ServiceOverride.ForKey((string) Key);
		}
	}
}