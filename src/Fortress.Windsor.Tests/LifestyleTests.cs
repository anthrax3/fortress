using System;
using System.Diagnostics;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Xunit;

namespace Castle.Windsor.Tests
{
	public class LifestyleTests
	{
		[Fact]
		public void TestForSerivces()
		{
			using (var container = new WindsorContainer())
			{
				container.Register(Component.For<IInterface>().ImplementedBy<InterfaceImpl>());
				IInterface childInterface;
				using (var childContainer = new WindsorContainer())
				{
					container.AddChildContainer(childContainer);
					childInterface = container.Resolve<IInterface>();
				} // childIhterface is NOT disposing here
				var @interface = container.Resolve<IInterface>();
				Assert.Same(@interface, childInterface);
				@interface.Do();
			} // but is disposing here and this is right behavior
		}

		[Fact]
		public void TestForTypedFactories()
		{
			using (var container = new WindsorContainer())
			{
				container.AddFacility<TypedFactoryFacility>();
				container.Register(Component.For<IFactory>().AsFactory(),
					Component.For(typeof(IInterface)).ImplementedBy(typeof(InterfaceImpl)).LifeStyle.Transient);

				IFactory childFactory;
				using (var childContainer = new WindsorContainer())
				{
					container.AddChildContainer(childContainer);
					childFactory = childContainer.Resolve<IFactory>();
				} // childFactory is disposing here
				var factory = container.Resolve<IFactory>();
				Assert.Same(factory, childFactory);
				factory.Create(); // throws an ObjectDisposedException exception
			} // but should be disposed here
		}

		public interface IFactory
		{
			IInterface Create();
		}

		public interface IInterface
		{
			void Do();
		}

		public class InterfaceImpl : IInterface, IDisposable
		{
			private bool disposed;

			public void Dispose()
			{
				disposed = true;
			}

			public void Do()
			{
				if (disposed)
					throw new NotSupportedException();
			}
		}
	}
}