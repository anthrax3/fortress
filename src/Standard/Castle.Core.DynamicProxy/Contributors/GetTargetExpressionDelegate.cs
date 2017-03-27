using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Castle.DynamicProxy.Contributors
{
	public delegate Expression GetTargetExpressionDelegate(ClassEmitter @class, MethodInfo method);
}