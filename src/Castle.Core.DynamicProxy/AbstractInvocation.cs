// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Castle.DynamicProxy
{
	public abstract class AbstractInvocation : IInvocation
	{
		private readonly IInterceptor[] interceptors;
		protected readonly object proxyObject;
		private int currentInterceptorIndex = -1;

		protected AbstractInvocation(
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments)
		{
			Debug.Assert(proxiedMethod != null);
			proxyObject = proxy;
			this.interceptors = interceptors;
			Method = proxiedMethod;
			Arguments = arguments;
		}

		public abstract object InvocationTarget { get; }

		public abstract Type TargetType { get; }

		public abstract MethodInfo MethodInvocationTarget { get; }

		public Type[] GenericArguments { get; private set; }

		public object Proxy
		{
			get { return proxyObject; }
		}

		public MethodInfo Method { get; }

		public MethodInfo GetConcreteMethod()
		{
			return EnsureClosedMethod(Method);
		}

		public MethodInfo GetConcreteMethodInvocationTarget()
		{
			// it is ensured by the InvocationHelper that method will be closed
			var method = MethodInvocationTarget;
			Debug.Assert(method == null || method.IsGenericMethodDefinition == false,
				"method == null || method.IsGenericMethodDefinition == false");
			return method;
		}

		public object ReturnValue { get; set; }

		public object[] Arguments { get; }

		public void SetArgumentValue(int index, object value)
		{
			Arguments[index] = value;
		}

		public object GetArgumentValue(int index)
		{
			return Arguments[index];
		}

		public void Proceed()
		{
			if (interceptors == null)
				// not yet fully initialized? probably, an intercepted method is called while we are being deserialized
			{
				InvokeMethodOnTarget();
				return;
			}

			currentInterceptorIndex++;
			try
			{
				if (currentInterceptorIndex == interceptors.Length)
				{
					InvokeMethodOnTarget();
				}
				else if (currentInterceptorIndex > interceptors.Length)
				{
					string interceptorsCount;
					if (interceptors.Length > 1)
						interceptorsCount = " each one of " + interceptors.Length + " interceptors";
					else
						interceptorsCount = " interceptor";

					var message = "This is a DynamicProxy2 error: invocation.Proceed() has been called more times than expected." +
					              "This usually signifies a bug in the calling code. Make sure that" + interceptorsCount +
					              " selected for the method '" + Method + "'" +
					              "calls invocation.Proceed() at most once.";
					throw new InvalidOperationException(message);
				}
				else
				{
					interceptors[currentInterceptorIndex].Intercept(this);
				}
			}
			finally
			{
				currentInterceptorIndex--;
			}
		}

		public void SetGenericMethodArguments(Type[] arguments)
		{
			GenericArguments = arguments;
		}

		protected abstract void InvokeMethodOnTarget();

		protected void ThrowOnNoTarget()
		{
			// let's try to build as friendly message as we can
			string interceptorsMessage;
			if (interceptors.Length == 0)
				interceptorsMessage = "There are no interceptors specified";
			else
				interceptorsMessage = "The interceptor attempted to 'Proceed'";

			string methodKindIs;
			string methodKindDescription;
			if (Method.DeclaringType.GetTypeInfo().IsClass && Method.IsAbstract)
			{
				methodKindIs = "is abstract";
				methodKindDescription = "an abstract method";
			}
			else
			{
				methodKindIs = "has no target";
				methodKindDescription = "method without target";
			}

			var message = string.Format("This is a DynamicProxy2 error: {0} for method '{1}' which {2}. " +
			                            "When calling {3} there is no implementation to 'proceed' to and " +
			                            "it is the responsibility of the interceptor to mimic the implementation " +
			                            "(set return value, out arguments etc)",
				interceptorsMessage, Method, methodKindIs, methodKindDescription);

			throw new NotImplementedException(message);
		}

		private MethodInfo EnsureClosedMethod(MethodInfo method)
		{
			if (method.ContainsGenericParameters)
			{
				Debug.Assert(GenericArguments != null);
				return method.GetGenericMethodDefinition().MakeGenericMethod(GenericArguments);
			}
			return method;
		}
	}
}