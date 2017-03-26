// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Registration;
using Xunit;

namespace Castle.Windsor.Tests
{
	
	public class TransientMultiConstructorTestCase
	{
		[Fact]
		public void TransientMultiConstructorTest()
		{
			var container = new DefaultKernel();
			((IKernel) container).Register(Component.For(typeof(FooBar)).Named("FooBar"));

			var arguments1 = new Dictionary<object, object>();
			arguments1.Add("integer", 1);

			var arguments2 = new Dictionary<object, object>();
			arguments2.Add("datetime", DateTime.Now.AddDays(1));

			var a = container.Resolve(typeof(FooBar), arguments1);
			var b = container.Resolve(typeof(FooBar), arguments2);

			Assert.NotSame(a, b);
		}

		[Fact]
		public void TransientMultipleConstructorNonValueTypeTest()
		{
			var container = new DefaultKernel();
			((IKernel) container).Register(Component.For(typeof(FooBarNonValue)).Named("FooBar"));
			var bla1 = new Tester1("FOOBAR");
			var bla2 = new Tester2(666);

			var arguments1 = new Dictionary<object, object>();
			arguments1.Add("test1", bla1);

			var arguments2 = new Dictionary<object, object>();
			arguments2.Add("test2", bla2);

			var a = container.Resolve(typeof(FooBarNonValue), arguments1);
			var b = container.Resolve(typeof(FooBarNonValue), arguments2);

			Assert.NotSame(a, b);

			// multi resolve test

			a = container.Resolve(typeof(FooBarNonValue), arguments1);
			b = container.Resolve(typeof(FooBarNonValue), arguments2);

			Assert.NotSame(a, b);
		}
	}
}