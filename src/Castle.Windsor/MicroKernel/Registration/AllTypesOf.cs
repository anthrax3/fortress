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

namespace Castle.Windsor.MicroKernel.Registration
{
	[Obsolete("Use Classes.From()... or Classes.FromAssembly() instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class AllTypesOf
	{
		private readonly Type basedOn;

		internal AllTypesOf(Type basedOn)
		{
			this.basedOn = basedOn;
		}

		[Obsolete("Use Classes.From(types).BasedOn(baseType)... instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor From(IEnumerable<Type> types)
		{
			return AllTypes.From(types).BasedOn(basedOn);
		}

		[Obsolete("Use Classes.From(types).BasedOn(baseType)... instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor From(params Type[] types)
		{
			return AllTypes.From(types).BasedOn(basedOn);
		}

		[Obsolete("Use Classes.FromAssembly(assembly).BasedOn(baseType)... instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor FromAssembly(Assembly assembly)
		{
			return AllTypes.FromAssembly(assembly).BasedOn(basedOn);
		}

		[Obsolete("Use Classes.FromAssemblyNamed(assemblyName).BasedOn(baseType)... instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor FromAssemblyNamed(string assemblyName)
		{
			return AllTypes.FromAssemblyNamed(assemblyName).BasedOn(basedOn);
		}

		[Obsolete("Use Classes.FromAssemblyNamed(assemblyName).Pick()... instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor Pick(IEnumerable<Type> types)
		{
			return AllTypes.From(types).BasedOn(basedOn);
		}
	}
}