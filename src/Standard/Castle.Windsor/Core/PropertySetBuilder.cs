using System.Reflection;

namespace Castle.Core
{
	public delegate PropertySet PropertySetBuilder(PropertyInfo property, bool isOptional);
}