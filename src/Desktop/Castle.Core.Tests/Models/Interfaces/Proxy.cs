using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;

namespace Castle.Core.Tests.GenInterfaces
{
	public class Proxy : GenInterfaceWithGenericTypes
	{
		private readonly IInterceptor[] interceptors;
		private readonly GenInterfaceWithGenericTypesImpl target;

		public Proxy(IInterceptor[] interceptors, GenInterfaceWithGenericTypesImpl target)
		{
			this.interceptors = interceptors;
			this.target = target;
		}

		public void Populate<T>(IList<T> list)
		{
			var inv = new Find3Invo<T>(target, interceptors, typeof(Proxy),
				null, null, new object[] {list});
			inv.Proceed();
		}

		public IList Find(string[,] query)
		{
			var inv = new Find1Invo(target, interceptors, typeof(Proxy),
				null, null, new object[] {query});
			inv.Proceed();

			return (IList) inv.ReturnValue;
		}

		public IList<T> Find<T>(string query)
		{
			var inv = new Find2Invo<T>(target, interceptors, typeof(Proxy),
				null, null, new object[] {query});
			inv.Proceed();

			return (IList<T>) inv.ReturnValue;
		}

		public IList<string> FindStrings(string query)
		{
			throw new NotImplementedException();
		}

		public IList Find(string query)
		{
			var inv = new Find1InvoA(target, interceptors, typeof(Proxy),
				null, null, new object[] {query});
			inv.Proceed();

			return (IList) inv.ReturnValue;
		}

		public class Find2Invo<T> : AbstractInvocation
		{
			private readonly GenInterfaceWithGenericTypesImpl target;

			public Find2Invo(GenInterfaceWithGenericTypesImpl target, IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments)
				: base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				ReturnValue = target.Find<T>((string) GetArgumentValue(0));
			}
		}

		public class Find1Invo : AbstractInvocation
		{
			private readonly GenInterfaceWithGenericTypesImpl target;

			public Find1Invo(GenInterfaceWithGenericTypesImpl target, IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments) :
				base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				ReturnValue = target.Find((string[,]) GetArgumentValue(0));
			}
		}

		public class Find1InvoA : AbstractInvocation
		{
			private readonly GenInterfaceWithGenericTypesImpl target;

			public Find1InvoA(GenInterfaceWithGenericTypesImpl target, IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments)
				: base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				ReturnValue = target.Find((string) GetArgumentValue(0));
			}
		}

		public class Find3Invo<T> : AbstractInvocation
		{
			private readonly GenInterfaceWithGenericTypesImpl target;

			public Find3Invo(GenInterfaceWithGenericTypesImpl target,
				IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments)
				: base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				target.Populate((List<T>) GetArgumentValue(0));
			}
		}
	}
}