using System;
using System.Runtime.Serialization;

namespace Castle.Core.Tests.DynamicProxy.Tests.Classes
{
	[Serializable]
	public class MySerializableClass2 : MySerializableClass, ISerializable
	{
		public MySerializableClass2()
		{
		}

		public MySerializableClass2(SerializationInfo info, StreamingContext context)
		{
			current = (DateTime) info.GetValue("dt", typeof(DateTime));
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("dt", current);
		}
	}
}