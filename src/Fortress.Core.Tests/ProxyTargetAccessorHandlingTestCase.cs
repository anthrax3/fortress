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

using System;
using Castle.Core.Tests.InterClasses;
using Castle.Core.Tests.Interfaces;
using Castle.DynamicProxy;
using Xunit;

namespace Castle.Core.Tests
{
	public class ProxyTargetAccessorHandlingTestCase : CoreBaseTestCase
	{
		private ProxyGenerationOptions MixIn(object mixin)
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(mixin);
			return options;
		}

		[Fact]
		public void ClassProxy_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateClassProxy(typeof(object), new[] {typeof(IProxyTargetAccessor)}));
			Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void ClassProxy_base()
		{
			var ex = Assert.Throws(typeof(ArgumentException), () =>
				generator.CreateClassProxy<ImplementsProxyTargetAccessor>());
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void ClassProxy_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateClassProxy(typeof(object), MixIn(new ImplementsProxyTargetAccessor())));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithoutTarget_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), new[] {typeof(IProxyTargetAccessor)}));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithoutTarget_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), new[] {typeof(IProxyTargetAccessor)},
					MixIn(new ImplementsProxyTargetAccessor())));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithoutTarget_TargetInterface()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IProxyTargetAccessor)));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithoutTarget_TargetInterface_derived()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IProxyTargetAccessorDerived)));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IOne), new[] {typeof(IProxyTargetAccessor)}, new One()));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IOne), new[] {typeof(IProxyTargetAccessor)}, new One(),
					MixIn(new ImplementsProxyTargetAccessor())));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_Target()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IProxyTargetAccessor), new ImplementsProxyTargetAccessor()));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_Target_derived()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IProxyTargetAccessorDerived), new ImplementsProxyTargetAccessorDerived()));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}


		[Fact]
		public void InterfaceProxyWithTargetInterface_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne), new[] {typeof(IProxyTargetAccessor)}, new One()));
            Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne), new[] {typeof(IProxyTargetAccessor)}, new One(),
					MixIn(new ImplementsProxyTargetAccessor())));
			Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_Target()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IProxyTargetAccessor), new ImplementsProxyTargetAccessor()));
			Assert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_Target_derived()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IProxyTargetAccessorDerived),
					new ImplementsProxyTargetAccessorDerived()));
			Assert.Contains("IProxyTargetAccessor", ex.Message);
		}
	}
}