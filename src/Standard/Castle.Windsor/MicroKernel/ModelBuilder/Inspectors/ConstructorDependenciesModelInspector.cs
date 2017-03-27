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

using System.Linq;
using System.Reflection;
using Castle.Compatibility;
using Castle.Core;

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	public class ConstructorDependenciesModelInspector : IContributeComponentModelConstruction
	{
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			var targetType = model.Implementation;
			var constructors = targetType.GetTypeInfo().GetConstructors(BindingFlags.Public | BindingFlags.Instance)
				.Where(IsVisibleToContainer);

			foreach (var constructor in constructors)
				model.AddConstructor(CreateConstructorCandidate(model, constructor));
		}

		protected virtual ConstructorCandidate CreateConstructorCandidate(ComponentModel model, ConstructorInfo constructor)
		{
			var parameters = constructor.GetParameters();
			var dependencies = parameters.ConvertAll(BuildParameterDependency);
			return new ConstructorCandidate(constructor, dependencies);
		}

		private static ConstructorDependencyModel BuildParameterDependency(ParameterInfo parameter)
		{
			return new ConstructorDependencyModel(parameter);
		}

		protected virtual bool IsVisibleToContainer(ConstructorInfo constructor)
		{
			return constructor.GetCustomAttributes<DoNotSelectAttribute>().Any() == false;
		}
	}
}