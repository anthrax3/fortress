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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.Core.Core.Internal;

namespace Castle.Core.DynamicProxy.Internal
{
	public static class InternalsUtil
	{
		private static readonly IDictionary<Assembly, bool> internalsToDynProxy = new Dictionary<Assembly, bool>();
		private static readonly Lock internalsToDynProxyLock = Lock.Create();

		public static bool IsInternal(this MethodBase method)
		{
			return method.IsAssembly || method.IsFamilyAndAssembly
			       && !method.IsFamilyOrAssembly;
		}

		public static bool IsInternalToDynamicProxy(this Assembly asm)
		{
			using (var locker = internalsToDynProxyLock.ForReadingUpgradeable())
			{
				if (internalsToDynProxy.ContainsKey(asm))
					return internalsToDynProxy[asm];

				locker.Upgrade();

				if (internalsToDynProxy.ContainsKey(asm))
					return internalsToDynProxy[asm];

				var internalsVisibleTo = asm.GetCustomAttributes<InternalsVisibleToAttribute>();
				var found = internalsVisibleTo.Any(VisibleToDynamicProxy);

				internalsToDynProxy.Add(asm, found);
				return found;
			}
		}

		private static bool VisibleToDynamicProxy(InternalsVisibleToAttribute attribute)
		{
			return attribute.AssemblyName.Contains(ModuleScope.DEFAULT_ASSEMBLY_NAME);
		}

		public static bool IsAccessible(this MethodBase method)
		{
			// Accessibility supported by the full framework and CoreCLR
			if (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly)
				return true;

			if (method.IsFamilyAndAssembly)
				return true;

			if (method.DeclaringType.GetTypeInfo().Assembly.IsInternalToDynamicProxy() && method.IsAssembly)
				return true;
			return false;
		}
	}
}