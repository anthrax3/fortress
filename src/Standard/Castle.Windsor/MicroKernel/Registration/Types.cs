// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.Windsor.Core.Internal;

namespace Castle.Windsor.MicroKernel.Registration
{
	public static class Types
	{
		public static FromTypesDescriptor From(IEnumerable<Type> types)
		{
			return new FromTypesDescriptor(types, null);
		}

		public static FromTypesDescriptor From(params Type[] types)
		{
			return new FromTypesDescriptor(types, null);
		}

		public static FromAssemblyDescriptor FromAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			return new FromAssemblyDescriptor(assembly, null);
		}

		public static FromAssemblyDescriptor FromAssemblyContaining(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			return new FromAssemblyDescriptor(type.GetTypeInfo().Assembly, null);
		}

		public static FromAssemblyDescriptor FromAssemblyContaining<T>()
		{
			return FromAssemblyContaining(typeof(T));
		}

		public static FromAssemblyDescriptor FromAssemblyInDirectory(AssemblyFilter filter)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");
			var assemblies = ReflectionUtil.GetAssemblies(filter);
			return new FromAssemblyDescriptor(assemblies, null);
		}

		public static FromAssemblyDescriptor FromAssemblyNamed(string assemblyName)
		{
			var assembly = ReflectionUtil.GetAssemblyNamed(assemblyName);
			return FromAssembly(assembly);
		}
	}
}