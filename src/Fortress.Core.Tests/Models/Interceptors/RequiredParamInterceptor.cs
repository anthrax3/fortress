// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

using System.Linq;
using System.Reflection;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.DynamicProxy;

namespace Castle.Core.Tests.Interceptors
{
	public class RequiredParamInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			var parameters = invocation.Method.GetParameters();

			var args = invocation.Arguments;

			for (var i = 0; i < parameters.Length; i++)
				if (parameters[i].IsDefined(typeof(RequiredAttribute), false))
				{
					var required =
						parameters[i].GetCustomAttributes(typeof(RequiredAttribute), false).First() as RequiredAttribute;

					if (required.BadValue == null && args[i] == null ||
					    required.BadValue != null && required.BadValue.Equals(args[i]))
						args[i] = required.DefaultValue;
				}

			invocation.Proceed();
		}
	}
}