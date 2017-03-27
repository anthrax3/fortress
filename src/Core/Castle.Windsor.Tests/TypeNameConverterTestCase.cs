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
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Tests.ClassComponents;
using Xunit;

namespace Castle.Windsor.Tests
{
	public class TypeNameConverterTestCase
	{
		public TypeNameConverterTestCase()
		{
			converter = new TypeNameConverter(new TypeNameParser());
		}

		private TypeNameConverter converter;

		private class TestCaseSensitivity
		{
		}

		private class TESTCASESENSITIVITY
		{
		}

		[Fact]
		public void Can_resolve_exact_match_if_two_classes_exist_that_differ_only_by_case()
		{
			var type = typeof(IGeneric<TestCaseSensitivity>);
			var name = type.AssemblyQualifiedName;
			var result = converter.PerformConversion(name, typeof(Type));
			Assert.Equal(type, result);

			var type2 = typeof(IGeneric<TESTCASESENSITIVITY>);
			var name2 = type2.AssemblyQualifiedName;
			var result2 = converter.PerformConversion(name2, typeof(Type));
			Assert.Equal(type2, result2);
		}
	}
}