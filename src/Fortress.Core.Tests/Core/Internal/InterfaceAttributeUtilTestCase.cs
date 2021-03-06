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
using Castle.Core.Internal;
using Xunit;


namespace Castle.Core.Tests.Core.Tests.Internal
{
	public class InterfaceAttributeUtilTestCase
	{
		[SingletonPrivate(Id = "Original")]
		[SingletonInherited(Id = "Original")]
		[AdditivePrivate(Id = "Original")]
		[AdditiveInherited(Id = "Original")]
		public interface IDeclaresAll
		{
		}

		[SingletonPrivate(Id = "Original")]
		public interface IDeclareSingletonPrivate
		{
		}

		[SingletonInherited(Id = "Original")]
		public interface IDeclareSingletonInherited
		{
		}

		[AdditivePrivate(Id = "Original")]
		public interface IDeclareAdditivePrivate
		{
		}

		[AdditiveInherited(Id = "Original")]
		public interface IDeclareAdditiveInherited
		{
		}

		public interface IInheritSingletonPrivate : IDeclareSingletonPrivate
		{
		}

		public interface IInheritSingletonInherited : IDeclareSingletonInherited
		{
		}

		public interface IInheritAdditivePrivate : IDeclareAdditivePrivate
		{
		}

		public interface IInheritAdditiveInherited : IDeclareAdditiveInherited
		{
		}

		[SingletonInherited(Id = "Override1")]
		public interface IOverrideSingletonInherited1 : IInheritSingletonInherited
		{
		}

		[SingletonInherited(Id = "Override2")]
		public interface IOverrideSingletonInherited2 : IInheritSingletonInherited
		{
		}

		[AdditiveInherited(Id = "Override1")]
		public interface IOverrideAdditiveInherited1 : IInheritAdditiveInherited
		{
		}

		[AdditiveInherited(Id = "Override2")]
		public interface IOverrideAdditiveInherited2 : IInheritAdditiveInherited
		{
		}

		public interface ISymmetricSingletonInherited : IOverrideSingletonInherited1, IOverrideSingletonInherited2
		{
		}

		public interface ISymmetricAdditiveInherited : IOverrideAdditiveInherited1, IOverrideAdditiveInherited2
		{
		}

		public interface IAsymmetricSingletonInherited : IDeclareSingletonInherited, IOverrideSingletonInherited1
		{
		}

		public interface IAsymmetricAdditiveInherited : IDeclareAdditiveInherited, IOverrideAdditiveInherited1
		{
		}

		[AttributeUsage(AttributeTargets.Interface)]
		public sealed class SingletonPrivateAttribute : MockAttribute
		{
		}

		[AttributeUsage(AttributeTargets.Interface)]
		public sealed class SingletonInheritedAttribute : MockAttribute
		{
		}

		[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
		public sealed class AdditivePrivateAttribute : MockAttribute
		{
		}

		[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
		public sealed class AdditiveInheritedAttribute : MockAttribute
		{
		}

		public abstract class MockAttribute : Attribute, IEquatable<MockAttribute>
		{
			private static readonly StringComparer
				IdComparer = StringComparer.Ordinal;

			public string Id { get; set; }

			public bool Equals(MockAttribute other)
			{
				return null != other
				       && GetType() == other.GetType()
				       && IdComparer.Equals(Id, other.Id);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as MockAttribute);
			}

			public override int GetHashCode()
			{
				return 17
				       + GetType().GetHashCode()
				       + IdComparer.GetHashCode(Id);
			}

			public override string ToString()
			{
				return string.Concat(GetType().Name, " [", Id, "]");
			}
		}

		private static void AssertAttributes(Type interfaceType, params MockAttribute[] expectedAttributes)
		{
			var attributes = InterfaceAttributeUtil.GetAttributes(interfaceType, true);

			Assert.NotNull(attributes);
			Assert.Equal(expectedAttributes, attributes);
		}

		private static void AssertInvalid(Type interfaceType)
		{
			Assert.Throws<InvalidOperationException>(() => InterfaceAttributeUtil.GetAttributes(interfaceType, true));
		}

		[Fact]
		public void Asymmetric_Multiple_Additive_Inherited()
		{
			AssertAttributes(typeof(IAsymmetricAdditiveInherited),
				new AdditiveInheritedAttribute {Id = "Original"},
				new AdditiveInheritedAttribute {Id = "Override1"}
			);
		}

		[Fact]
		public void Asymmetric_Multiple_Singleton_Inherited()
		{
			AssertAttributes(typeof(IAsymmetricSingletonInherited),
				new SingletonInheritedAttribute {Id = "Override1"}
			);
		}

		[Fact]
		public void Declared_All()
		{
			AssertAttributes(typeof(IDeclaresAll),
				new SingletonPrivateAttribute {Id = "Original"},
				new SingletonInheritedAttribute {Id = "Original"},
				new AdditivePrivateAttribute {Id = "Original"},
				new AdditiveInheritedAttribute {Id = "Original"}
			);
		}

		[Fact]
		public void Inherit_Additive_Inherited()
		{
			AssertAttributes(typeof(IInheritAdditiveInherited),
				new AdditiveInheritedAttribute {Id = "Original"}
			);
		}


		[Fact]
		public void Inherit_Singleton_Inherited()
		{
			AssertAttributes(typeof(IInheritSingletonInherited),
				new SingletonInheritedAttribute {Id = "Original"}
			);
		}

		[Fact]
		public void Override_Singleton_Inherited()
		{
			AssertAttributes(typeof(IOverrideSingletonInherited1),
				new SingletonInheritedAttribute {Id = "Override1"}
			);
		}

		[Fact]
		public void Symmetric_Multiple_Additive_Inherited()
		{
			AssertAttributes(typeof(ISymmetricAdditiveInherited),
				new AdditiveInheritedAttribute {Id = "Original"},
				new AdditiveInheritedAttribute {Id = "Override2"},
                new AdditiveInheritedAttribute { Id = "Override1" }
            );
		}

		[Fact]
		public void Symmetric_Multiple_Singleton_Inherited()
		{
			AssertInvalid(typeof(ISymmetricSingletonInherited));
		}
	}
}