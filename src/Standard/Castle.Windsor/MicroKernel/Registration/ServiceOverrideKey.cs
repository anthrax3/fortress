using System;
using System.Collections.Generic;

namespace Castle.MicroKernel.Registration
{
	public class ServiceOverrideKey
	{
		private readonly object key;

		internal ServiceOverrideKey(string key)
		{
			this.key = key;
		}

		internal ServiceOverrideKey(Type key)
		{
			this.key = key;
		}

		public ServiceOverride Eq(string value)
		{
			return new ServiceOverride(key, value);
		}

		public ServiceOverride Eq(params string[] value)
		{
			return new ServiceOverride(key, value);
		}

		public ServiceOverride Eq<V>(params string[] value)
		{
			return new ServiceOverride(key, value, typeof(V));
		}

		public ServiceOverride Eq(IEnumerable<string> value)
		{
			return new ServiceOverride(key, value);
		}

		public ServiceOverride Eq<V>(IEnumerable<string> value)
		{
			return new ServiceOverride(key, value, typeof(V));
		}

		public ServiceOverride Eq(params Type[] componentTypes)
		{
			return new ServiceOverride(key, componentTypes);
		}

		public ServiceOverride Eq<V>(params Type[] componentTypes)
		{
			return new ServiceOverride(key, componentTypes, typeof(V));
		}
	}
}