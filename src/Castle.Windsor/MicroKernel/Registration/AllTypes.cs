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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Castle.Windsor.MicroKernel.Registration
{
	[Obsolete("'AllTypes' has been deprecated and will be removed in future releases. Use 'Classes' static class (if you want to just register concrete classes) or 'Types' static class (if you want to register interfaces or abstract classes too) instead. It exposes exactly the same methods.")]
	public static class AllTypes
	{
		public static FromTypesDescriptor From(IEnumerable<Type> types)
		{
			return Classes.From(types);
		}

		public static FromTypesDescriptor From(params Type[] types)
		{
			return Classes.From(types);
		}

		public static FromAssemblyDescriptor FromAssembly(Assembly assembly)
		{
			return Classes.FromAssembly(assembly);
		}

		public static FromAssemblyDescriptor FromAssemblyContaining(Type type)
		{
			return Classes.FromAssemblyContaining(type);
		}

		public static FromAssemblyDescriptor FromAssemblyContaining<T>()
		{
			return Classes.FromAssemblyContaining<T>();
		}

		public static FromAssemblyDescriptor FromAssemblyInDirectory(AssemblyFilter filter)
		{
			return Classes.FromAssemblyInDirectory(filter);
		}

		public static FromAssemblyDescriptor FromAssemblyNamed(string assemblyName)
		{
			return Classes.FromAssemblyNamed(assemblyName);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static FromAssemblyDescriptor FromThisAssembly()
		{
			return Classes.FromAssembly(Assembly.GetCallingAssembly());
		}

		[Obsolete("Use Classes.FromAssembly...BasedOn(basedOn) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static AllTypesOf Of(Type basedOn)
		{
			return new AllTypesOf(basedOn);
		}

		[Obsolete("Use Classes.FromAssembly...BasedOn<T>() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static AllTypesOf Of<T>()
		{
			return new AllTypesOf(typeof(T));
		}

		[Obsolete("Use Classes.FromAssembly...Pick() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static AllTypesOf Pick()
		{
			return Of<object>();
		}

		[Obsolete("Use From(types) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static FromTypesDescriptor Pick(IEnumerable<Type> types)
		{
			return new FromTypesDescriptor(types, Classes.Filter);
		}
	}
}
