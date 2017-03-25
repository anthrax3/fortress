// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

using Castle.Core.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class MethodEquivalenceTestCase
	{
		[Fact]
		public void CanProxyTypesWithMethodsOnlyDifferentByGenericArguments()
		{
			var generator = new ProxyGenerator();

			var target1 = (IMyService) generator.CreateInterfaceProxyWithTarget(
				typeof(IMyService), new MyServiceImpl(), new StandardInterceptor());
			Assert.NotNull(target1.CreateSomething<int>("aa"));

			var target2 = (IMyService) generator.CreateClassProxy(
				typeof(MyServiceImpl), new StandardInterceptor());
			Assert.NotNull(target2.CreateSomething<int>("aa"));
		}
	}
}