using System.Reflection;
using Castle.Core.DynamicProxy.Generators.Emitters;
using Castle.Core.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Castle.Core.DynamicProxy.Contributors
{
	public delegate Expression GetTargetExpressionDelegate(ClassEmitter @class, MethodInfo method);
}