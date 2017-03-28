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
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Installer;
using Xunit;

namespace Castle.Windsor.Tests.Installer
{
	
	public class FromAssemblyInstallersTestCase : AbstractContainerTestCase
	{
		[Fact]
		public void Can_install_from_assembly_by_assembly()
		{
			Container.Install(FromAssembly.Instance(ThisAssembly));
			Container.Resolve<object>("Customer-by-CustomerInstaller");
		}

		[Fact]
		public void Can_install_from_assembly_by_directory_simple()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;
			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location)));
			Container.Resolve<object>("Customer-by-CustomerInstaller");
		}

		[Fact]
		public void Can_install_from_assembly_by_name()
		{
			Container.Install(FromAssembly.Named("Castle.Windsor.Tests"));
		}

		[Fact]
		public void Can_install_from_assembly_by_type()
		{
			Container.Install(FromAssembly.Containing(GetType()));
		}

		[Fact]
		public void Can_install_from_assembly_by_type_generic()
		{
			Container.Install(FromAssembly.Containing<FromAssemblyInstallersTestCase>());
		}

		[Fact]
		public void Install_from_assembly_by_directory_empty_name_searches_currentDirectory()
		{
			var called = false;
			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(string.Empty).FilterByAssembly(a =>
			{
				called = true;
				return true;
			})));

			Assert.True(called);
			Assert.True(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}

		[Fact]
		public void Install_from_assembly_by_directory_executes_assembly_condition()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;
			var called = false;
			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location).FilterByAssembly(a =>
			{
				called = true;
				return true;
			})));

			Assert.True(called);
			Assert.True(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}

		[Fact]
		public void Install_from_assembly_by_directory_executes_name_condition()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;
			var byNameCalled = false;
			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location).FilterByName(a =>
			{
				byNameCalled = true;
				return true;
			})));

			Assert.True(byNameCalled);
			Assert.True(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}


		[Fact]
		public void Install_from_assembly_by_directory_ignores_non_existing_path()
		{
			var location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString("N"));

			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location)));

			Assert.Equal(0, Container.Kernel.GraphNodes.Length);
		}

		[Fact]
		public void Install_from_assembly_by_directory_obeys_assembly_condition()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;
			var called = false;
			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location).FilterByAssembly(a =>
			{
				called = true;
				return false;
			})));

			Assert.True(called);
			Assert.False(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}

		[Fact]
		public void Install_from_assembly_by_directory_obeys_name_condition()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;
			var byNameCalled = false;
			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location).FilterByName(a =>
			{
				byNameCalled = true;
				return false;
			})));

			Assert.True(byNameCalled);
			Assert.False(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}

		[Fact]
		public void Install_from_assembly_by_directory_with_fake_key_as_string_does_not_install()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;

			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location).WithKeyToken("1234123412341234")));
			Assert.False(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}


		[Fact]
		public void Install_from_assembly_by_directory_with_key_installs()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;

			var publicKeyToken = GetType().GetTypeInfo().Assembly.GetName().GetPublicKeyToken();
			if (publicKeyToken == null || publicKeyToken.Length == 0)
				return;

			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location).WithKeyToken(GetType())));
			Assert.True(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}

		[Fact]
		public void Install_from_assembly_by_directory_with_mscorlib_key_does_not_install()
		{
			var location = AppDomain.CurrentDomain.BaseDirectory;

			var publicKeyToken = GetType().GetTypeInfo().Assembly.GetName().GetPublicKeyToken();
			if (publicKeyToken == null || publicKeyToken.Length == 0)
				return;

			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(location).WithKeyToken<object>()));
			Assert.False(Container.Kernel.HasComponent("Customer-by-CustomerInstaller"));
		}
	}
}