using System;
using System.Collections;
using System.Collections.Generic;

namespace Castle.Windsor.Tests.Registration
{
	public class ReadOnlyDictionary : Dictionary<object, object>, IDictionary
	{
		public bool IsReadOnly
		{
			get { return true; }
		}

		public new void Add(object key, object value)
		{
			throw new NotSupportedException();
		}
	}
}