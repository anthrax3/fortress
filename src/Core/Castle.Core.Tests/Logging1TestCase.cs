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
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class Logging1TestCase
	{
		public interface IEmptyInterface
		{
		}

		public interface ISingleMethodInterface
		{
			void InterfaceMethod();
		}

		public class NonVirtualMethodClass : ISingleMethodInterface
		{
			public void InterfaceMethod()
			{
			}

			public void ClassMethod()
			{
			}
		}

		public class ClassWithInterfaceMethodExplicitlyImplemented : ISingleMethodInterface
		{
			void ISingleMethodInterface.InterfaceMethod()
			{
			}
		}

		public class EmptyHook : IProxyGenerationHook
		{
			public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
			{
				return true;
			}

			public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
			{
			}

			public void MethodsInspected()
			{
			}
		}

		[Fact]
		public void CacheHitClassProxy()
		{
			// Arrange
			var logger = new CollectingLogger();
			var generator = new ProxyGenerator {Logger = logger};

			// Act
			generator.CreateClassProxy<DynamicProxy.Tests.Classes.EmptyClass>();
			generator.CreateClassProxy<DynamicProxy.Tests.Classes.EmptyClass>();

			// Assert
			Assert.True(logger.RecordedMessage(LoggerLevel.Debug, "Found cached proxy type Castle.Proxies.EmptyClassProxy " +
			                                                      "for target type Castle.Core.Tests.DynamicProxy.Tests.Classes.EmptyClass."));
		}

		[Fact]
		public void CacheHitInterfaceProxy()
		{
			// Arrange
			var logger = new CollectingLogger();
			var generator = new ProxyGenerator {Logger = logger};

			// Act
			generator.CreateInterfaceProxyWithoutTarget<IEmptyInterface>();
			generator.CreateInterfaceProxyWithoutTarget<IEmptyInterface>();

			// Assert
			Assert.True(logger.RecordedMessage(LoggerLevel.Debug, "Found cached proxy type Castle.Proxies.IEmptyInterfaceProxy " +
			                                                      "for target type Castle.Core.Tests.Logging1TestCase+IEmptyInterface."));
		}

		[Fact]
		public void CacheMiss()
		{
			// Arrange
			var logger = new CollectingLogger();
			var generator = new ProxyGenerator {Logger = logger};

			// Act
			generator.CreateClassProxy<DynamicProxy.Tests.Classes.EmptyClass>();

			// Assert
			Assert.True(logger.RecordedMessage(LoggerLevel.Debug, "No cached proxy type was found for target type " +
			                                                      "Castle.Core.Tests.DynamicProxy.Tests.Classes.EmptyClass."));
		}

		[Fact]
		public void ExcludedNonVirtualMethods()
		{
			// Arrange
			var logger = new CollectingLogger();
			var generator = new ProxyGenerator {Logger = logger};

			// Act
			generator.CreateClassProxy<NonVirtualMethodClass>();

			// Assert
			Assert.True(logger.RecordedMessage(LoggerLevel.Debug, "Excluded non-overridable method ClassMethod on " +
			                                                      "Castle.Core.Tests.Logging1TestCase+NonVirtualMethodClass because it cannot be intercepted."));
			Assert.True(logger.RecordedMessage(LoggerLevel.Debug, "Excluded non-overridable method InterfaceMethod on " +
			                                                      "Castle.Core.Tests.Logging1TestCase+NonVirtualMethodClass because it cannot be intercepted."));
		}

		[Fact]
		public void ProxyGenerationOptionsEqualsAndGetHashCodeNotOverriden()
		{
			// Arrange
			var logger = new CollectingLogger();
			var generator = new ProxyGenerator {Logger = logger};

			// Act
			var options = new ProxyGenerationOptions
			{
				Hook = new EmptyHook()
			};
			generator.CreateClassProxy(typeof(DynamicProxy.Tests.Classes.EmptyClass), options);

			// Assert
			Assert.True(logger.RecordedMessage(LoggerLevel.Warn, "The IProxyGenerationHook type " +
			                                                     "Castle.Core.Tests.Logging1TestCase+EmptyHook does not override both Equals and GetHashCode. " +
			                                                     "If these are not correctly overridden caching will fail to work causing performance problems."));
		}
	}
}