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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.Core.DynamicProxy.Generators;
using Castle.Core.DynamicProxy.Internal;

namespace Castle.Core.DynamicProxy.Serialization
{
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable, IDeserializationCallback
	{
		private readonly Type baseType;

		private readonly StreamingContext context;

		private readonly SerializationInfo info;

		private readonly Type[] interfaces;

		private readonly object proxy;

		private readonly ProxyGenerationOptions proxyGenerationOptions;

		private bool delegateToBase;

		private bool isInterfaceProxy;

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			this.info = info;
			this.context = context;

			baseType = DeserializeTypeFromString("__baseType");

			var _interfaceNames = (string[]) info.GetValue("__interfaces", typeof(string[]));
			interfaces = new Type[_interfaceNames.Length];

			for (var i = 0; i < _interfaceNames.Length; i++)
				interfaces[i] = Type.GetType(_interfaceNames[i]);

			proxyGenerationOptions = (ProxyGenerationOptions) info.GetValue("__proxyGenerationOptions", typeof(ProxyGenerationOptions));
			proxy = RecreateProxy();

			DeserializeProxyState();
		}

		public static ModuleScope ModuleScope { get; private set; } = new ModuleScope();

		public void OnDeserialization(object sender)
		{
			var interceptors = GetValue<IInterceptor[]>("__interceptors");
			SetInterceptors(interceptors);

			DeserializeProxyMembers();

			// Get the proxy state again, to get all those members we couldn't get in the constructor due to deserialization ordering.
			DeserializeProxyState();
			InvokeCallback(proxy);
		}

		public object GetRealObject(StreamingContext context)
		{
			return proxy;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// There is no need to implement this method as 
			// this class would never be serialized.
		}

		public static void ResetScope()
		{
			SetScope(new ModuleScope());
		}

		public static void SetScope(ModuleScope scope)
		{
			if (scope == null)
				throw new ArgumentNullException(nameof(scope));

			ModuleScope = scope;
		}

		private Type DeserializeTypeFromString(string key)
		{
			return Type.GetType(info.GetString(key), true, false);
		}

		protected virtual object RecreateProxy()
		{
			var generatorType = GetValue<string>("__proxyTypeId");
			if (generatorType.Equals(ProxyTypeConstants.Class))
			{
				isInterfaceProxy = false;
				return RecreateClassProxy();
			}
			if (generatorType.Equals(ProxyTypeConstants.ClassWithTarget))
			{
				isInterfaceProxy = false;
				return RecreateClassProxyWithTarget();
			}
			isInterfaceProxy = true;
			return RecreateInterfaceProxy(generatorType);
		}

		private object RecreateClassProxyWithTarget()
		{
			var generator = new ClassProxyWithTargetGenerator(ModuleScope, baseType, interfaces, proxyGenerationOptions);
			var proxyType = generator.GetGeneratedType();
			return InstantiateClassProxy(proxyType);
		}

		public object RecreateInterfaceProxy(string generatorType)
		{
			var @interface = DeserializeTypeFromString("__theInterface");
			var targetType = DeserializeTypeFromString("__targetFieldType");

			InterfaceProxyWithTargetGenerator generator;

			if (generatorType == ProxyTypeConstants.InterfaceWithTarget)
				generator = new InterfaceProxyWithTargetGenerator(ModuleScope, @interface);
			else if (generatorType == ProxyTypeConstants.InterfaceWithoutTarget)
				generator = new InterfaceProxyWithoutTargetGenerator(ModuleScope, @interface);
			else if (generatorType == ProxyTypeConstants.InterfaceWithTargetInterface)
				generator = new InterfaceProxyWithTargetInterfaceGenerator(ModuleScope, @interface);
			else
				throw new InvalidOperationException($"Got value {generatorType} for the interface generator type, which is not known for the purpose of serialization.");

			var proxyType = generator.GenerateCode(targetType, interfaces, proxyGenerationOptions);
			return FormatterServices.GetSafeUninitializedObject(proxyType);
		}

		public object RecreateClassProxy()
		{
			var generator = new ClassProxyGenerator(ModuleScope, baseType);
			var proxyType = generator.GenerateCode(interfaces, proxyGenerationOptions);
			return InstantiateClassProxy(proxyType);
		}

		private object InstantiateClassProxy(Type proxy_type)
		{
			delegateToBase = GetValue<bool>("__delegateToBase");
			if (delegateToBase)
				return Activator.CreateInstance(proxy_type, info, context);
			return FormatterServices.GetSafeUninitializedObject(proxy_type);
		}

		protected void InvokeCallback(object target)
		{
			if (target is IDeserializationCallback)
				(target as IDeserializationCallback).OnDeserialization(this);
		}

		private void DeserializeProxyMembers()
		{
			var proxyType = proxy.GetType();
			var members = FormatterServices.GetSerializableMembers(proxyType);

			var deserializedMembers = new List<MemberInfo>();
			var deserializedValues = new List<object>();
			for (var i = 0; i < members.Length; i++)
			{
				var member = members[i] as FieldInfo;
				if (member.DeclaringType != proxyType)
					continue;

				Debug.Assert(member != null);
				var value = info.GetValue(member.Name, member.FieldType);
				deserializedMembers.Add(member);
				deserializedValues.Add(value);
			}
			FormatterServices.PopulateObjectMembers(proxy, deserializedMembers.ToArray(), deserializedValues.ToArray());
		}

		private void DeserializeProxyState()
		{
			if (isInterfaceProxy)
			{
				var target = GetValue<object>("__target");
				SetTarget(target);
			}
			else if (!delegateToBase)
			{
				var baseMemberData = GetValue<object[]>("__data");
				var members = FormatterServices.GetSerializableMembers(baseType);

				members = TypeUtil.Sort(members);

				FormatterServices.PopulateObjectMembers(proxy, members, baseMemberData);
			}
		}

		private void SetTarget(object target)
		{
			var targetField = proxy.GetType().GetField("__target");
			if (targetField == null)
				throw new SerializationException("The SerializationInfo specifies an invalid interface proxy type, which has no __target field.");

			targetField.SetValue(proxy, target);
		}

		private void SetInterceptors(IInterceptor[] interceptors)
		{
			var interceptorField = proxy.GetType().GetField("__interceptors");
			if (interceptorField == null)
				throw new SerializationException("The SerializationInfo specifies an invalid proxy type, which has no __interceptors field.");

			interceptorField.SetValue(proxy, interceptors);
		}

		private T GetValue<T>(string name)
		{
			return (T) info.GetValue(name, typeof(T));
		}
	}
}