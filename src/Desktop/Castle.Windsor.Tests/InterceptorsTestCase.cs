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
using System.Threading;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Tests.Interceptors;
using Xunit;

namespace Castle.Windsor.Tests
{
	public class InterceptorsTestCase : IDisposable
	{
		public InterceptorsTestCase()
		{
			container = new WindsorContainer();
			container.AddFacility<MyInterceptorGreedyFacility>();
		}

		public void Dispose()
		{
			container.Dispose();
		}

		private IWindsorContainer container;
		private readonly ManualResetEvent startEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);
		private CalculatorService service;

		private void ExecuteMethodUntilSignal()
		{
			startEvent.WaitOne(int.MaxValue);

			while (!stopEvent.WaitOne(1))
			{
				Assert.Equal(5, service.Sum(2, 2));
				Assert.Equal(6, service.Sum(3, 2));
				Assert.Equal(8, service.Sum(3, 4));
			}
		}

		[Fact]
		public void AutomaticallyOmitTarget()
		{
			container.Register(
				Component.For<ICalcService>()
					.Interceptors(InterceptorReference.ForType<ReturnDefaultInterceptor>()).Last,
				Component.For<ReturnDefaultInterceptor>()
			);

			var calcService = container.Resolve<ICalcService>();
			Assert.Equal(0, calcService.Sum(1, 2));
		}

		[Fact]
		public void ClassProxy()
		{
			container.Register(Component.For(typeof(ResultModifierInterceptor)).Named("interceptor"));
			container.Register(Component.For(typeof(CalculatorService)).Named("key"));

			service = container.Resolve<CalculatorService>("key");

			Assert.NotNull(service);
			Assert.Equal(5, service.Sum(2, 2));
		}

		[Fact]
		public void ClassProxyWithAttributes()
		{
			container = new WindsorContainer(); // So we wont use the facilities

			container.Register(Component.For<ResultModifierInterceptor>(),
				Component.For<CalculatorServiceWithAttributes>());

			var service = container.Resolve<CalculatorServiceWithAttributes>();

			Assert.NotNull(service);
			Assert.Equal(5, service.Sum(2, 2));
		}

		[Fact]
		public void ClosedGenericInterceptor()
		{
			container.Register(Component.For(typeof(GenericInterceptor<object>)));
			container.Register(Component.For(typeof(CalculatorService)).Interceptors<GenericInterceptor<object>>());

			var service = container.Resolve<CalculatorService>();

			Assert.NotNull(service);
			Assert.Equal(4, service.Sum(2, 2));
		}

		[Fact]
		public void Interface_that_depends_on_service_it_is_intercepting()
		{
			container.Register(Component.For<InterceptorThatCauseStackOverflow>(),
				Component.For<ICameraService>().ImplementedBy<CameraService>().Interceptors<InterceptorThatCauseStackOverflow>(),
				//because it has no interceptors, it is okay to resolve it...
				Component.For<ICameraService>().ImplementedBy<CameraService>().Named("okay to resolve"));

			container.Resolve<ICameraService>();
		}

		[Fact]
		public void InterfaceProxy()
		{
			container.Register(Component.For(typeof(ResultModifierInterceptor)).Named("interceptor"));
			container.Register(Component.For(typeof(ICalcService)).ImplementedBy(typeof(CalculatorService)).Named("key"));

			var service = container.Resolve<ICalcService>("key");

			Assert.NotNull(service);
			Assert.Equal(5, service.Sum(2, 2));
		}

		[Fact]
		public void InterfaceProxyWithLifecycle()
		{
			container.Register(Component.For(typeof(ResultModifierInterceptor)).Named("interceptor"));
			container.Register(Component.For(typeof(ICalcService)).ImplementedBy(typeof(CalculatorServiceWithLifecycle)).Named("key"));

			var service = container.Resolve<ICalcService>("key");

			Assert.NotNull(service);
			Assert.True(service.Initialized);
			Assert.Equal(5, service.Sum(2, 2));

			Assert.False(service.Disposed);

			container.Release(service);

			Assert.True(service.Disposed);
		}

		[Fact]
		public void Multithreaded()
		{
			container.Register(Component.For(typeof(ResultModifierInterceptor)).Named("interceptor"));
			container.Register(Component.For(typeof(CalculatorService)).Named("key"));

			service = container.Resolve<CalculatorService>("key");

			const int threadCount = 10;

			var threads = new Thread[threadCount];

			for (var i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(ExecuteMethodUntilSignal);
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(2000);

			stopEvent.Set();
		}

		[Fact]
		public void OnBehalfOfTest()
		{
			container.Register(Component.For(typeof(InterceptorWithOnBehalf)).Named("interceptor"));
			container.Register(Component.For(typeof(CalculatorService)).Named("key"));

			var service = container.Resolve<CalculatorService>("key");

			Assert.NotNull(service);
			Assert.Equal(4, service.Sum(2, 2));
			Assert.NotNull(InterceptorWithOnBehalf.Model);
			Assert.Equal("key", InterceptorWithOnBehalf.Model.Name);
			Assert.Equal(typeof(CalculatorService),
				InterceptorWithOnBehalf.Model.Implementation);
		}

		[Fact]
		public void OpenGenericInterceporIsUsedAsClosedGenericInterceptor()
		{
			container.Register(Component.For(typeof(GenericInterceptor<>)));
			container.Register(Component.For(typeof(CalculatorService)).Interceptors<GenericInterceptor<object>>());

			var service = container.Resolve<CalculatorService>();

			Assert.NotNull(service);
			Assert.Equal(4, service.Sum(2, 2));
		}
	}
}