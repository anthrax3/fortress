// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.Collections.Generic;

	public abstract class FromDescriptor : IRegistration
	{
		private readonly Predicate<Type> additionalFilters;
		private readonly IList<BasedOnDescriptor> criterias;
		private bool allowMultipleMatches;

		protected FromDescriptor(Predicate<Type> additionalFilters)
		{
			this.additionalFilters = additionalFilters;
			allowMultipleMatches = false;
			criterias = new List<BasedOnDescriptor>();
		}

		protected abstract IEnumerable<Type> SelectedTypes(IKernel kernel);

		public FromDescriptor AllowMultipleMatches()
		{
			allowMultipleMatches = true;
			return this;
		}

		public BasedOnDescriptor BasedOn<T>()
		{
			return BasedOn(typeof(T));
		}

		public BasedOnDescriptor BasedOn(Type basedOn)
		{
			return BasedOn((IEnumerable<Type>)new[] { basedOn });
		}

		public BasedOnDescriptor BasedOn(params Type[] basedOn)
		{
			return BasedOn((IEnumerable<Type>)basedOn);
		}

		public BasedOnDescriptor BasedOn(IEnumerable<Type> basedOn)
		{
			var descriptor = new BasedOnDescriptor(basedOn, this, additionalFilters);
			criterias.Add(descriptor);
			return descriptor;
		}

		public BasedOnDescriptor InNamespace(string @namespace)
		{
			return Where(Component.IsInNamespace(@namespace, false));
		}

		public BasedOnDescriptor InNamespace(string @namespace, bool includeSubnamespaces)
		{
			return Where(Component.IsInNamespace(@namespace, includeSubnamespaces));
		}

		public BasedOnDescriptor InSameNamespaceAs(Type type)
		{
			return Where(Component.IsInSameNamespaceAs(type));
		}

		public BasedOnDescriptor InSameNamespaceAs(Type type, bool includeSubnamespaces)
		{
			return Where(Component.IsInSameNamespaceAs(type, includeSubnamespaces));
		}

		public BasedOnDescriptor InSameNamespaceAs<T>()
		{
			return Where(Component.IsInSameNamespaceAs<T>());
		}

		public BasedOnDescriptor InSameNamespaceAs<T>(bool includeSubnamespaces) where T : class
		{
			return Where(Component.IsInSameNamespaceAs<T>(includeSubnamespaces));
		}

		public BasedOnDescriptor Pick()
		{
			return BasedOn<object>();
		}

		public BasedOnDescriptor Where(Predicate<Type> accepted)
		{
			var descriptor = new BasedOnDescriptor(new[] { typeof(object) }, this, additionalFilters).If(accepted);
			criterias.Add(descriptor);
			return descriptor;
		}

		void IRegistration.Register(IKernelInternal kernel)
		{
			if (criterias.Count == 0)
			{
				return;
			}

			foreach (var type in SelectedTypes(kernel))
			{
				foreach (var criteria in criterias)
				{
					if (criteria.TryRegister(type, kernel) && !allowMultipleMatches)
					{
						break;
					}
				}
			}
		}
	}
}
