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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using Castle.Core.DynamicProxy.Generators;

namespace Castle.Core.DynamicProxy.Serialization
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class CacheMappingsAttribute : Attribute
	{
		private static readonly ConstructorInfo constructor =
			typeof(CacheMappingsAttribute).GetConstructor(new[] {typeof(byte[])});

		public CacheMappingsAttribute(byte[] serializedCacheMappings)
		{
			SerializedCacheMappings = serializedCacheMappings;
		}

		public byte[] SerializedCacheMappings { get; }

		public Dictionary<CacheKey, string> GetDeserializedMappings()
		{
			using (var stream = new MemoryStream(SerializedCacheMappings))
			{
				var formatter = new BinaryFormatter();
				return (Dictionary<CacheKey, string>) formatter.Deserialize(stream);
			}
		}

		public static void ApplyTo(AssemblyBuilder assemblyBuilder, Dictionary<CacheKey, string> mappings)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, mappings);
				var bytes = stream.ToArray();
				var attributeBuilder = new CustomAttributeBuilder(constructor, new object[] {bytes});
				assemblyBuilder.SetCustomAttribute(attributeBuilder);
			}
		}
	}
}