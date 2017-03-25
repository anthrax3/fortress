// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Xunit;


namespace Castle.Core.Tests.DynamicProxy.Tests
{
	public class AttributesToAvoidReplicatingTestCase : CoreBaseTestCase
	{
		[NonInheritable]
		public class AttributedClass_NonInheritable
		{
		}

		[Inheritable]
		public class AttributedClass_Inheritable
		{
		}

		public class AttributedClass_SecurityPermission
		{
		}

		public class AttributedClass_ReflectionPermission
		{
		}

		private int AttributeCount<TAttribute>(object proxy)
			where TAttribute : Attribute
		{
			return proxy.GetType().GetTypeInfo().GetCustomAttributes(typeof(TAttribute), false).Count();
		}

		[Fact]
		public void InheritableAttribute_should_not_be_replicated_as_it_is_inherited_by_the_runtime()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_Inheritable>();
			Assert.Equal(0, AttributeCount<InheritableAttribute>(proxy));
		}

		[Fact]
		public void NonInheritableAttribute_should_be_replicated_as_it_is_not_inherited()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_NonInheritable>();
			Assert.Equal(1, AttributeCount<NonInheritableAttribute>(proxy));
		}
	}
}