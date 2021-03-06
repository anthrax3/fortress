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
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.SubSystems.Conversion;

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	public class LifestyleModelInspector : IContributeComponentModelConstruction
	{
		private readonly IConversionManager converter;

		public LifestyleModelInspector(IConversionManager converter)
		{
			this.converter = converter;
		}

		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (!ReadLifestyleFromConfiguration(model))
				ReadLifestyleFromType(model);
		}

		protected virtual bool ReadLifestyleFromConfiguration(ComponentModel model)
		{
			if (model.Configuration == null)
				return false;

			var lifestyleRaw = model.Configuration.Attributes["lifestyle"];
			if (lifestyleRaw != null)
			{
				var lifestyleType = converter.PerformConversion<LifestyleType>(lifestyleRaw);
				model.LifestyleType = lifestyleType;
				switch (lifestyleType)
				{
					case LifestyleType.Singleton:
					case LifestyleType.Transient:
					case LifestyleType.PerWebRequest:
					case LifestyleType.Thread:
						return true;
					case LifestyleType.Pooled:
						ExtractPoolConfig(model);
						return true;
					case LifestyleType.Custom:
						var lifestyle = GetMandatoryTypeFromAttribute(model, "customLifestyleType", lifestyleType);
						ValidateTypeFromAttribute(lifestyle, typeof(ILifestyleManager), "customLifestyleType");
						model.CustomLifestyle = lifestyle;

						return true;
                    // This is not a thing anymore
					//case LifestyleType.Scoped:
					//	var scopeAccessorType1 = GetTypeFromAttribute(model, "scopeAccessorType");
					//	if (scopeAccessorType1 != null)
					//	{
					//		ValidateTypeFromAttribute(scopeAccessorType1, typeof(IScopeAccessor), "scopeAccessorType");
					//		model.ExtendedProperties[Constants.ScopeAccessorType] = scopeAccessorType1;
					//	}
					//	return true;
					case LifestyleType.Bound:
						var binderType1 = GetTypeFromAttribute(model, "scopeRootBinderType");
						if (binderType1 != null)
						{
							var binder = ExtractBinder(binderType1, model.Name);
							model.ExtendedProperties[Constants.ScopeRootSelector] = binder;
						}
						return true;
					default:
						throw new InvalidOperationException(string.Format("Component {0} has {1} lifestyle. This is not a valid value.", model.Name, lifestyleType));
				}
			}
			// type was not present, but we might figure out the lifestyle based on presence of some attributes related to some lifestyles
			var binderType = GetTypeFromAttribute(model, "scopeRootBinderType");
			if (binderType != null)
			{
				var binder = ExtractBinder(binderType, model.Name);
				model.ExtendedProperties[Constants.ScopeRootSelector] = binder;
				model.LifestyleType = LifestyleType.Bound;
				return true;
			}
            // This is not a thing anymore
			//var scopeAccessorType = GetTypeFromAttribute(model, "scopeAccessorType");
			//if (scopeAccessorType != null)
			//{
			//	ValidateTypeFromAttribute(scopeAccessorType, typeof(IScopeAccessor), "scopeAccessorType");
			//	model.ExtendedProperties[Constants.ScopeAccessorType] = scopeAccessorType;
			//	model.LifestyleType = LifestyleType.Scoped;
			//	return true;
			//}
			var customLifestyleType = GetTypeFromAttribute(model, "customLifestyleType");
			if (customLifestyleType != null)
			{
				ValidateTypeFromAttribute(customLifestyleType, typeof(ILifestyleManager), "customLifestyleType");
				model.CustomLifestyle = customLifestyleType;
				model.LifestyleType = LifestyleType.Custom;
				return true;
			}
			return false;
		}

		protected virtual void ReadLifestyleFromType(ComponentModel model)
		{
			var attributes = AttributesUtil.GetAttributes<LifestyleAttribute>(model.Implementation).ToArray();
			if (attributes.Length == 0)
				return;
			var attribute = attributes[0];
			model.LifestyleType = attribute.Lifestyle;

			if (model.LifestyleType == LifestyleType.Custom)
			{
				var custom = (CustomLifestyleAttribute) attribute;
				ValidateTypeFromAttribute(custom.CustomLifestyleType, typeof(ILifestyleManager), "CustomLifestyleType");
				model.CustomLifestyle = custom.CustomLifestyleType;
			}
			else if (model.LifestyleType == LifestyleType.Pooled)
			{
				var pooled = (PooledAttribute) attribute;
				model.ExtendedProperties[ExtendedPropertiesConstants.Pool_InitialPoolSize] = pooled.InitialPoolSize;
				model.ExtendedProperties[ExtendedPropertiesConstants.Pool_MaxPoolSize] = pooled.MaxPoolSize;
			}
			else if (model.LifestyleType == LifestyleType.Bound)
			{
				var binder = ExtractBinder(((BoundToAttribute) attribute).ScopeRootBinderType, model.Name);
				model.ExtendedProperties[Constants.ScopeRootSelector] = binder;
			}
            // This is not a thing anymore
			//else if (model.LifestyleType == LifestyleType.Scoped)
			//{
			//	var scoped = (ScopedAttribute) attribute;
			//	if (scoped.ScopeAccessorType != null)
			//	{
			//		ValidateTypeFromAttribute(scoped.ScopeAccessorType, typeof(IScopeAccessor), "ScopeAccessorType");
			//		model.ExtendedProperties[Constants.ScopeAccessorType] = scoped.ScopeAccessorType;
			//	}
			//}
		}

		protected virtual void ValidateTypeFromAttribute(Type typeFromAttribute, Type expectedInterface, string attribute)
		{
			if (expectedInterface.IsAssignableFrom(typeFromAttribute))
				return;
			throw new InvalidOperationException(string.Format("The Type '{0}' specified in the '{2}' attribute must implement {1}", typeFromAttribute.FullName, expectedInterface.FullName, attribute));
		}

		private Func<IHandler[], IHandler> ExtractBinder(Type scopeRootBinderType, string name)
		{
			var filterMethod = scopeRootBinderType.GetTypeInfo().FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public, IsBindMethod, null).FirstOrDefault();

            if (filterMethod == null)
				throw new InvalidOperationException(
					string.Format(
						"Type {0} which was designated as 'scopeRootBinderType' for component {1} does not have any public instance method matching signature of 'IHandler Method(IHandler[] pickOne)' and can not be used as scope root binder.",
						scopeRootBinderType.Name, name));

            var instance = scopeRootBinderType.CreateInstance<object>();

		    var actualFilterMethod = instance.GetType().GetTypeInfo().DeclaredMethods.FirstOrDefault(x => x.Name == filterMethod.Name);

            if (actualFilterMethod == null)
                throw new InvalidOperationException(
                    string.Format(
                        "Could not find declared method {0} on {1} this might be a bug in Windsor.",
                        filterMethod.Name, instance.GetType().Name));

		    Func<IHandler[], IHandler> filterInvoke = 
                handlers => (IHandler) actualFilterMethod.Invoke(instance, handlers);

		    return filterInvoke;
		}

		private void ExtractPoolConfig(ComponentModel model)
		{
			var initialRaw = model.Configuration.Attributes["initialPoolSize"];
			var maxRaw = model.Configuration.Attributes["maxPoolSize"];

			if (initialRaw != null)
			{
				var initial = converter.PerformConversion<int>(initialRaw);
				model.ExtendedProperties[ExtendedPropertiesConstants.Pool_InitialPoolSize] = initial;
			}
			if (maxRaw != null)
			{
				var max = converter.PerformConversion<int>(maxRaw);
				model.ExtendedProperties[ExtendedPropertiesConstants.Pool_MaxPoolSize] = max;
			}
		}

		private Type GetMandatoryTypeFromAttribute(ComponentModel model, string attribute, LifestyleType lifestyleType)
		{
			var rawAttribute = model.Configuration.Attributes[attribute];
			if (rawAttribute == null)
				throw new InvalidOperationException(string.Format("Component {0} has {1} lifestyle, but its configuration doesn't have mandatory '{2}' attribute.", model.Name, lifestyleType, attribute));
			return converter.PerformConversion<Type>(rawAttribute);
		}

		private Type GetTypeFromAttribute(ComponentModel model, string attribute)
		{
			var rawAttribute = model.Configuration.Attributes[attribute];
			if (rawAttribute == null)
				return null;
			return converter.PerformConversion<Type>(rawAttribute);
		}

		private bool IsBindMethod(MemberInfo methodMember, object _)
		{
			var method = (MethodInfo) methodMember;
			if (method.ReturnType != typeof(IHandler))
				return false;
			var parameters = method.GetParameters();
			if (parameters.Length != 1)
				return false;
			if (parameters[0].ParameterType != typeof(IHandler[]))
				return false;
			return true;
		}
	}
}