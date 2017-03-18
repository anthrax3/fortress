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

using System;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel.Util;

namespace Castle.Windsor.Core
{
	[Serializable]
	public class DependencyModel
	{
		private readonly Type targetItemType;
		private readonly Type targetType;
		protected ParameterModel parameterModel;
		protected string reference;

		protected bool initialized;

		public DependencyModel(String dependencyKey, Type targetType, bool isOptional)
			: this(dependencyKey, targetType, isOptional, false, null)
		{
		}

		// TODO: add configuration so that information about override is attached to the dependency
		public DependencyModel(string dependencyKey, Type targetType, bool isOptional, bool hasDefaultValue, object defaultValue)
		{
			this.targetType = targetType;
			if (targetType != null && targetType.IsByRef)
			{
				targetItemType = targetType.GetElementType();
			}
			else
			{
				targetItemType = targetType;
			}
			DependencyKey = dependencyKey;
			IsOptional = isOptional;
			HasDefaultValue = hasDefaultValue;
			DefaultValue = defaultValue;
		}

		public object DefaultValue { get; set; }

		public string DependencyKey { get; set; }

		public bool HasDefaultValue { get; set; }

		public bool IsOptional { get; set; }

		public bool IsPrimitiveTypeDependency
		{
			get { return targetItemType.IsPrimitiveTypeOrCollection(); }
		}

		public ParameterModel Parameter
		{
			get
			{
				if (!initialized)
				{
					throw new InvalidOperationException("Not initialized!");
				}
				return parameterModel;
			}
			set
			{
				parameterModel = value;
				if (parameterModel != null)
				{
					reference = ReferenceExpressionUtil.ExtractComponentName(parameterModel.Value);
				}
			}
		}

		public string ReferencedComponentName
		{
			get
			{
				if (!initialized)
				{
					throw new InvalidOperationException("Not initialized!");
				}
				return reference;
			}
		}

		public Type TargetItemType
		{
			get { return targetItemType; }
		}

		public Type TargetType
		{
			get { return targetType; }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			var other = obj as DependencyModel;
			if (other == null)
			{
				return false;
			}
			return other.targetType == targetType &&
			       Equals(other.DependencyKey, DependencyKey);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = (targetType != null ? targetType.GetHashCode() : 0);
				result = (result*397) ^ (DependencyKey != null ? DependencyKey.GetHashCode() : 0);
				return result;
			}
		}

		public virtual void Init(ParameterModelCollection parameters)
		{
			initialized = true;
			if (parameters == null)
			{
				return;
			}
			Parameter = ObtainParameterModelByName(parameters) ?? ObtainParameterModelByType(parameters);
		}

		public override string ToString()
		{
			return string.Format("Dependency '{0}' type '{1}'", DependencyKey, TargetType);
		}

		private ParameterModel GetParameterModelByType(Type type, ParameterModelCollection parameters)
		{
			var assemblyQualifiedName = type.AssemblyQualifiedName;
			if (assemblyQualifiedName == null)
			{
				return null;
			}

			return parameters[assemblyQualifiedName];
		}

		private ParameterModel ObtainParameterModelByName(ParameterModelCollection parameters)
		{
			if (DependencyKey == null)
			{
				return null;
			}

			return parameters[DependencyKey];
		}

		private ParameterModel ObtainParameterModelByType(ParameterModelCollection parameters)
		{
			var type = TargetItemType;
			if (type == null)
			{
				// for example it's an interceptor
				return null;
			}
			var found = GetParameterModelByType(type, parameters);
			if (found == null && type.IsGenericType)
			{
				found = GetParameterModelByType(type.GetGenericTypeDefinition(), parameters);
			}
			return found;
		}
	}
}
