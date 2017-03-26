// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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
using System.Reflection;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel.Registration;

namespace Castle.Windsor.Windsor.Installer
{
	public class FromAssembly
	{
		public static IWindsorInstaller Containing(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			var assembly = type.GetTypeInfo().Assembly;
			return Instance(assembly);
		}

		public static IWindsorInstaller Containing(Type type, InstallerFactory installerFactory)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			var assembly = type.GetTypeInfo().Assembly;
			return Instance(assembly, installerFactory);
		}

		public static IWindsorInstaller Containing<T>()
		{
			return Containing(typeof(T));
		}

		public static IWindsorInstaller Containing<T>(InstallerFactory installerFactory)
		{
			return Containing(typeof(T), installerFactory);
		}

		public static IWindsorInstaller InDirectory(AssemblyFilter filter)
		{
			return InDirectory(filter, new InstallerFactory());
		}

		public static IWindsorInstaller InDirectory(AssemblyFilter filter, InstallerFactory installerFactory)
		{
			var assemblies = new HashSet<Assembly>(ReflectionUtil.GetAssemblies(filter));
			var installer = new CompositeInstaller();
			foreach (var assembly in assemblies)
				installer.Add(Instance(assembly, installerFactory));
			return installer;
		}

		public static IWindsorInstaller Instance(Assembly assembly)
		{
			return Instance(assembly, new InstallerFactory());
		}

		public static IWindsorInstaller Instance(Assembly assembly, InstallerFactory installerFactory)
		{
			return new AssemblyInstaller(assembly, installerFactory);
		}

		public static IWindsorInstaller Named(string assemblyName)
		{
			var assembly = ReflectionUtil.GetAssemblyNamed(assemblyName);
			return Instance(assembly);
		}

		public static IWindsorInstaller Named(string assemblyName, InstallerFactory installerFactory)
		{
			var assembly = ReflectionUtil.GetAssemblyNamed(assemblyName);
			return Instance(assembly, installerFactory);
		}
	}
}