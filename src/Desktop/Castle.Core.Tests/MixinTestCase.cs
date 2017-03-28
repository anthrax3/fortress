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
using System.Reflection;
using Castle.Core.Tests.InterClasses;
using Castle.Core.Tests.Mixins;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class MixinTestCase : CoreBaseTestCase
	{
		private class AssertInvocationInterceptor : StandardInterceptor
		{
			public bool Invoked;
			public object mixin;
			public object proxy;

			protected override void PreProceed(IInvocation invocation)
			{
				Invoked = true;
				mixin = invocation.InvocationTarget;
				proxy = invocation.Proxy;
				base.PreProceed(invocation);
			}
		}

		public interface IMyInterface
		{
			string Name { get; set; }

			bool Started { get; set; }

			int Calc(int x, int y);

			int Calc(int x, int y, int z, float k);
		}

		public class MyInterfaceImpl : IMyInterface
		{
			private string _name;
			private bool _started;

			public virtual string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			public virtual bool Started
			{
				get { return _started; }
				set { _started = value; }
			}

			public virtual int Calc(int x, int y)
			{
				return x + y;
			}

			public virtual int Calc(int x, int y, int z, float k)
			{
				return x + y + z + (int) k;
			}
		}

		[Fact]
		public void CanCreateSimpleMixinWithoutGettingExecutionEngineExceptionsOrBadImageExceptions()
		{
			var proxyGenerationOptions = new ProxyGenerationOptions();
			proxyGenerationOptions.AddMixinInstance(new SimpleMixin());
			var proxy = generator.CreateClassProxy(
				typeof(object), proxyGenerationOptions, new AssertInvocationInterceptor());

			Assert.True(proxy is ISimpleMixin);

			((ISimpleMixin) proxy).DoSomething();
		}

		[Fact]
		public void MixinForInterfaces()
		{
			var proxyGenerationOptions = new ProxyGenerationOptions();

			var mixin_instance = new SimpleMixin();
			proxyGenerationOptions.AddMixinInstance(mixin_instance);

			var interceptor = new AssertInvocationInterceptor();

			var target = new MyInterfaceImpl();

			var proxy = generator.CreateInterfaceProxyWithTarget(
				typeof(IMyInterface), target, proxyGenerationOptions, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IMyInterface).GetTypeInfo().IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			var mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void MixinImplementingMoreThanOneInterface()
		{
			var proxyGenerationOptions = new ProxyGenerationOptions();

			var mixin_instance = new ComplexMixin();
			proxyGenerationOptions.AddMixinInstance(mixin_instance);

			var interceptor = new AssertInvocationInterceptor();

			var proxy = generator.CreateClassProxy(
				typeof(SimpleClass), proxyGenerationOptions, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(SimpleClass).GetTypeInfo().IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			var inter3 = proxy as IThird;
			Assert.NotNull(inter3);
			inter3.DoThird();

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);

			var inter2 = proxy as ISecond;
			Assert.NotNull(inter2);
			inter2.DoSecond();

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);

			var inter1 = proxy as IFirst;
			Assert.NotNull(inter1);
			inter1.DoFirst();

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void MixinWithSameInterface_InterfaceWithTarget_AdditionalInterfaces()
		{
			var options = new ProxyGenerationOptions();
			var mixin1 = new SimpleMixin();
			options.AddMixinInstance(mixin1);

			var interceptor = new StandardInterceptor();
			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new[] {typeof(ISimpleMixin)}, new ServiceImpl(), options, interceptor);
			Assert.Equal(1, (proxy as ISimpleMixin).DoSomething());
		}

		[Fact]
		public void MixinWithSameInterface_InterfaceWithTarget_AdditionalInterfaces_Derived()
		{
			var options = new ProxyGenerationOptions();
			var mixin1 = new SimpleMixin();
			options.AddMixinInstance(mixin1);

			var interceptor = new StandardInterceptor();
			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new[] {typeof(IDerivedSimpleMixin)}, new ServiceImpl(), options, interceptor);
			Assert.Equal(1, (proxy as ISimpleMixin).DoSomething());
		}

		[Fact]
		public void SimpleMixin_ClassProxy()
		{
			var options = new ProxyGenerationOptions();
			var mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			var interceptor = new AssertInvocationInterceptor();

			var proxy = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(SimpleClass).GetTypeInfo().IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			var mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void SimpleMixin_InterfaceProxy_WithoutTarget()
		{
			var options = new ProxyGenerationOptions();
			var mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			var interceptor = new AssertInvocationInterceptor();

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IService), new Type[0],
				options, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IService).GetTypeInfo().IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			var mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void SimpleMixin_InterfaceProxy_WithTarget()
		{
			var options = new ProxyGenerationOptions();
			var mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			var interceptor = new AssertInvocationInterceptor();

			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new ServiceImpl(),
				options, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IService).GetTypeInfo().IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			var mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void SimpleMixin_InterfaceProxy_WithtTargetInterface()
		{
			var options = new ProxyGenerationOptions();
			var mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			var interceptor = new AssertInvocationInterceptor();

			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), new ServiceImpl(), options,
				interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IService).GetTypeInfo().IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			var mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void TestTypeCachingWithMixins()
		{
			var options = new ProxyGenerationOptions();
			var mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			var interceptor = new AssertInvocationInterceptor();

			var proxy1 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			options = new ProxyGenerationOptions();
			mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			interceptor = new AssertInvocationInterceptor();

			var proxy2 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			Assert.True(proxy1.GetType().Equals(proxy2.GetType()));
		}

		[Fact]
		public void TestTypeCachingWithMultipleMixins()
		{
			var options = new ProxyGenerationOptions();
			var mixin_instance1 = new SimpleMixin();
			var mixin_instance2 = new ComplexMixin();
			options.AddMixinInstance(mixin_instance1);
			options.AddMixinInstance(mixin_instance2);

			var interceptor = new AssertInvocationInterceptor();

			var proxy1 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			options = new ProxyGenerationOptions();
			mixin_instance1 = new SimpleMixin();
			mixin_instance2 = new ComplexMixin();
			options.AddMixinInstance(mixin_instance2);
			options.AddMixinInstance(mixin_instance1);

			interceptor = new AssertInvocationInterceptor();

			var proxy2 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			Assert.True(proxy1.GetType().Equals(proxy2.GetType()));
		}

		[Fact]
		public void TwoMixins()
		{
			var proxyGenerationOptions = new ProxyGenerationOptions();

			var mixin1 = new SimpleMixin();
			var mixin2 = new OtherMixin();

			proxyGenerationOptions.AddMixinInstance(mixin1);
			proxyGenerationOptions.AddMixinInstance(mixin2);

			var interceptor = new AssertInvocationInterceptor();

			var proxy = generator.CreateClassProxy(
				typeof(SimpleClass), proxyGenerationOptions, interceptor);

			Assert.False(interceptor.Invoked);

			Assert.NotNull(proxy);
			Assert.True(typeof(SimpleClass).GetTypeInfo().IsAssignableFrom(proxy.GetType()));

			var mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin1, interceptor.mixin);

			var other = proxy as IOtherMixin;
			Assert.NotNull(other);
			Assert.Equal(3, other.Sum(1, 2));
			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin2, interceptor.mixin);
		}

		[Fact]
		public void TwoMixinsWithSameInterface()
		{
			var options = new ProxyGenerationOptions();
			var mixin1 = new SimpleMixin();
			var mixin2 = new OtherMixinImplementingISimpleMixin();
			options.AddMixinInstance(mixin1);
			options.AddMixinInstance(mixin2);

			var interceptor = new StandardInterceptor();

			Assert.Throws<InvalidMixinConfigurationException>(() =>
				generator.CreateClassProxy(typeof(SimpleClass), options, interceptor)
			);
		}
	}
}