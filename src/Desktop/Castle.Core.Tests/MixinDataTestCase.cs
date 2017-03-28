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
using System.Collections.Generic;
using Castle.Core.Tests.Mixins;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class MixinDataTestCase
	{
		public MixinDataTestCase()
		{
			simpleMixin = new SimpleMixin();
			otherMixin = new OtherMixin();
			complexMixin = new ComplexMixin();
		}

		private SimpleMixin simpleMixin;
		private OtherMixin otherMixin;
		private ComplexMixin complexMixin;

		[Fact]
		public void ContainsMixinWithInterface()
		{
			var mixinData = new MixinData(new object[] {simpleMixin});
			Assert.True(mixinData.ContainsMixin(typeof(ISimpleMixin)));
			Assert.False(mixinData.ContainsMixin(typeof(IOtherMixin)));
		}

		[Fact]
		public void Equals_False_WithDifferentInstances()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			var mixinData2 = new MixinData(new object[] {simpleMixin, complexMixin});
			Assert.NotEqual(mixinData1, mixinData2);
		}

		[Fact]
		public void Equals_False_WithInstanceCount()
		{
			var mixinData1 = new MixinData(new object[] {otherMixin});
			var mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.NotEqual(mixinData1, mixinData2);
		}

		[Fact]
		public void Equals_True_WithDifferentInstances()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			var mixinData2 = new MixinData(new object[] {new SimpleMixin(), new OtherMixin()});
			Assert.Equal(mixinData1, mixinData2);
		}

		[Fact]
		public void Equals_True_WithDifferentOrder()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			var mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.Equal(mixinData1, mixinData2);
		}

		[Fact]
		public void GetHashCode_Equal_WithDifferentInstances()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			var mixinData2 = new MixinData(new object[] {new SimpleMixin(), new OtherMixin()});
			Assert.Equal(mixinData1.GetHashCode(), mixinData2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_Equal_WithDifferentOrder()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			var mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.Equal(mixinData1.GetHashCode(), mixinData2.GetHashCode());
		}

		[Fact]
		public void GetMixinPosition()
		{
			var mixinData = new MixinData(new object[] {simpleMixin});
			Assert.Equal(0, mixinData.GetMixinPosition(typeof(ISimpleMixin)));
		}

		[Fact]
		public void GetMixinPosition_MatchesMixinInstances()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			Assert.Equal(0, mixinData1.GetMixinPosition(typeof(IOtherMixin)));
			Assert.Equal(1, mixinData1.GetMixinPosition(typeof(ISimpleMixin)));

			var mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.Equal(0, mixinData2.GetMixinPosition(typeof(IOtherMixin)));
			Assert.Equal(1, mixinData2.GetMixinPosition(typeof(ISimpleMixin)));
		}

		[Fact]
		public void GetMixinPosition_MatchesMixinInstances_WithMultipleInterfacesPerMixin()
		{
			var mixinData = new MixinData(new object[] {complexMixin, simpleMixin});
			Assert.Equal(0, mixinData.GetMixinPosition(typeof(IFirst)));
			Assert.Equal(1, mixinData.GetMixinPosition(typeof(ISecond)));
			Assert.Equal(2, mixinData.GetMixinPosition(typeof(ISimpleMixin)));
			Assert.Equal(3, mixinData.GetMixinPosition(typeof(IThird)));

			var mixins = new List<object>(mixinData.Mixins);
			Assert.Same(complexMixin, mixins[0]);
			Assert.Same(complexMixin, mixins[1]);
			Assert.Same(simpleMixin, mixins[2]);
			Assert.Same(complexMixin, mixins[3]);
		}

		[Fact]
		public void MixinInterfaces()
		{
			var mixinData = new MixinData(new object[] {simpleMixin});
			var mixinInterfaces = new List<Type>(mixinData.MixinInterfaces);
			Assert.Equal(1, mixinInterfaces.Count);
			Assert.Same(mixinInterfaces[0], typeof(ISimpleMixin));
		}

		[Fact]
		public void MixinInterfaces_SortedLikeMixins()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			var mixinInterfaces1 = new List<Type>(mixinData1.MixinInterfaces);
			Assert.Equal(2, mixinInterfaces1.Count);
			Assert.Same(typeof(IOtherMixin), mixinInterfaces1[0]);
			Assert.Same(typeof(ISimpleMixin), mixinInterfaces1[1]);

			var mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			var mixinInterfaces2 = new List<Type>(mixinData2.MixinInterfaces);
			Assert.Equal(2, mixinInterfaces2.Count);
			Assert.Same(typeof(IOtherMixin), mixinInterfaces2[0]);
			Assert.Same(typeof(ISimpleMixin), mixinInterfaces2[1]);
		}

		[Fact]
		public void Mixins()
		{
			var mixinData = new MixinData(new object[] {simpleMixin});
			var mixins = new List<object>(mixinData.Mixins);
			Assert.Equal(1, mixins.Count);
			Assert.Same(simpleMixin, mixins[0]);
		}

		[Fact]
		public void MixinsAreSortedByInterface()
		{
			var mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			var mixins1 = new List<object>(mixinData1.Mixins);
			Assert.Equal(2, mixins1.Count);
			Assert.Same(otherMixin, mixins1[0]);
			Assert.Same(simpleMixin, mixins1[1]);

			var mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			var mixins2 = new List<object>(mixinData2.Mixins);
			Assert.Equal(2, mixins2.Count);
			Assert.Same(otherMixin, mixins2[0]);
			Assert.Same(simpleMixin, mixins2[1]);
		}

		[Fact]
		public void MixinsNotImplementingInterfacesAreIgnored()
		{
			var mixinData = new MixinData(new[] {new object()});
			var mixins = new List<object>(mixinData.Mixins);
			Assert.Equal(0, mixins.Count);
		}

		[Fact]
		public void TwoMixinsWithSameInterfaces()
		{
			var mixin1 = new SimpleMixin();
			var mixin2 = new OtherMixinImplementingISimpleMixin();

			Assert.Throws<ArgumentException>(() =>
				new MixinData(new object[] {mixin1, mixin2})
			);
		}
	}
}