using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Castle.DynamicProxy.Contributors
{
	public delegate Reference GetTargetReferenceDelegate(ClassEmitter @class, MethodInfo method);
}