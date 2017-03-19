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
using Castle.Windsor.MicroKernel.Handlers;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.Tests.ClassComponents;
using NUnit.Framework;

namespace Castle.Windsor.Tests
{
	[TestFixture]
	public class DecoratorsTestCase : AbstractContainerTestCase
	{
		[Test]
		public void Should_ignore_reference_to_itself()
		{
			Kernel.Register(
				Component.For<IRepository>().ImplementedBy<Repository1>(),
				Component.For<IRepository>().ImplementedBy<DecoratedRepository>()
				);
			var repos = (Repository1)Kernel.Resolve<IRepository>();
			Assert.IsInstanceOf(typeof(DecoratedRepository), repos.InnerRepository);
		}

		
		[Test]
		public void Will_give_good_error_message_if_cannot_resolve_service_that_is_likely_decorated_when_there_are_multiple_service()
		{
			Assert.Throws<HandlerException>(() =>
			{
				Kernel.Register(
					Component.For<IRepository>().ImplementedBy<Repository1>(),
					Component.For<IRepository>().ImplementedBy<DecoratedRepository2>().Named("foo"),
					Component.For<IRepository>().ImplementedBy<Repository1>().Named("bar")
					);
				Kernel.Resolve<IRepository>();
			});
		}
	}
}
