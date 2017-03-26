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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Core.Core;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.MicroKernel.SubSystems.Resource;
using Castle.Windsor.Windsor.Configuration;
using Castle.Windsor.Windsor.Configuration.Interpreters;
using Castle.Windsor.Windsor.Diagnostics;
using Castle.Windsor.Windsor.Installer;
using Castle.Windsor.Windsor.Proxy;

namespace Castle.Windsor.Windsor
{
	[DebuggerDisplay("{Name,nq}")]
	[DebuggerTypeProxy(typeof(KernelDebuggerProxy))]
	public partial class WindsorContainer : IWindsorContainer
	{
		private const string CastleUnicode = " \uD83C\uDFF0 ";
		private static int instanceCount;
		private readonly Dictionary<string, IWindsorContainer> childContainers = new Dictionary<string, IWindsorContainer>(StringComparer.OrdinalIgnoreCase);
		private readonly object childContainersLocker = new object();

		private readonly IKernel kernel;


		private IWindsorContainer parent;

		public WindsorContainer() : this(new DefaultKernel(), new DefaultComponentInstaller())
		{
		}

        public WindsorContainer(IConfigurationStore store) : this()
        {
            kernel.ConfigurationStore = store;

            RunInstaller();
        }

        public WindsorContainer(IConfigurationInterpreter interpreter) : this()
		{
			if (interpreter == null)
				throw new ArgumentNullException("interpreter");

            interpreter.ProcessResource(interpreter.Source, kernel.ConfigurationStore, kernel);

			RunInstaller();
		}

		public WindsorContainer(IConfigurationInterpreter interpreter, IEnvironmentInfo environmentInfo) : this()
		{
			if (interpreter == null)
				throw new ArgumentNullException("interpreter");
			if (environmentInfo == null)
				throw new ArgumentNullException("environmentInfo");

			interpreter.EnvironmentName = environmentInfo.GetEnvironmentName();
			interpreter.ProcessResource(interpreter.Source, kernel.ConfigurationStore, kernel);

			RunInstaller();
		}

		public WindsorContainer(IKernel kernel, IComponentsInstaller installer)
			: this(CastleUnicode + ++instanceCount, kernel, installer)
		{
		}

		public WindsorContainer(string name, IKernel kernel, IComponentsInstaller installer)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (kernel == null)
				throw new ArgumentNullException("kernel");
			if (installer == null)
				throw new ArgumentNullException("installer");

			Name = name;
			this.kernel = kernel;
			this.kernel.ProxyFactory = new DefaultProxyFactory();
			Installer = installer;
		}

		public WindsorContainer(IProxyFactory proxyFactory)
		{
			if (proxyFactory == null)
				throw new ArgumentNullException("proxyFactory");

			kernel = new DefaultKernel(proxyFactory);

			Installer = new DefaultComponentInstaller();
		}

		public WindsorContainer(IWindsorContainer parent) : this()
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			parent.AddChildContainer(this);

			RunInstaller();
		}

		public WindsorContainer(string name, IWindsorContainer parent, IConfigurationInterpreter interpreter) : this()
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (interpreter == null)
				throw new ArgumentNullException("interpreter");

			Name = name;

			parent.AddChildContainer(this);

			interpreter.ProcessResource(interpreter.Source, kernel.ConfigurationStore, kernel);

			RunInstaller();
		}

		public IComponentsInstaller Installer { get; }

		public virtual IKernel Kernel
		{
			get { return kernel; }
		}

		public string Name { get; }

		public virtual IWindsorContainer Parent
		{
			get { return parent; }
			set
			{
				if (value == null)
				{
					if (parent != null)
					{
						parent.RemoveChildContainer(this);
						parent = null;
					}
				}
				else
				{
					if (value != parent)
					{
						parent = value;
						parent.AddChildContainer(this);
					}
				}
			}
		}

		public virtual void Dispose()
		{
			Parent = null;
			childContainers.Clear();
			kernel.Dispose();
		}

		public virtual void AddChildContainer(IWindsorContainer childContainer)
		{
			if (childContainer == null)
				throw new ArgumentNullException("childContainer");

			if (!childContainers.ContainsKey(childContainer.Name))
				lock (childContainersLocker)
				{
					if (!childContainers.ContainsKey(childContainer.Name))
					{
						kernel.AddChildKernel(childContainer.Kernel);
						childContainers.Add(childContainer.Name, childContainer);
						childContainer.Parent = this;
					}
				}
		}

		public IWindsorContainer AddFacility(IFacility facility)
		{
			kernel.AddFacility(facility);
			return this;
		}

		public IWindsorContainer AddFacility<T>() where T : IFacility, new()
		{
			kernel.AddFacility<T>();
			return this;
		}

		public IWindsorContainer AddFacility<T>(Action<T> onCreate)
			where T : IFacility, new()
		{
			kernel.AddFacility(onCreate);
			return this;
		}

		public IWindsorContainer GetChildContainer(string name)
		{
			lock (childContainersLocker)
			{
				IWindsorContainer windsorContainer;
				childContainers.TryGetValue(name, out windsorContainer);
				return windsorContainer;
			}
		}

		public IWindsorContainer Install(params IWindsorInstaller[] installers)
		{
			if (installers == null)
				throw new ArgumentNullException("installers");

			if (installers.Length == 0)
				return this;

			var scope = new DefaultComponentInstaller();

			var internalKernel = kernel as IKernelInternal;
			if (internalKernel == null)
			{
				Install(installers, scope);
			}
			else
			{
				var token = internalKernel.OptimizeDependencyResolution();
				Install(installers, scope);
				if (token != null)
					token.Dispose();
			}

			return this;
		}

		public IWindsorContainer Register(params IRegistration[] registrations)
		{
			Kernel.Register(registrations);
			return this;
		}

		public virtual void Release(object instance)
		{
			kernel.ReleaseComponent(instance);
		}

		public virtual void RemoveChildContainer(IWindsorContainer childContainer)
		{
			if (childContainer == null)
				throw new ArgumentNullException("childContainer");

			if (childContainers.ContainsKey(childContainer.Name))
				lock (childContainersLocker)
				{
					if (childContainers.ContainsKey(childContainer.Name))
					{
						kernel.RemoveChildKernel(childContainer.Kernel);
						childContainers.Remove(childContainer.Name);
						childContainer.Parent = null;
					}
				}
		}

		public virtual object Resolve(Type service, IDictionary arguments)
		{
			return kernel.Resolve(service, arguments);
		}

		public virtual object Resolve(Type service, object argumentsAsAnonymousType)
		{
			return Resolve(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		public virtual object Resolve(Type service)
		{
			return kernel.Resolve(service, null);
		}

		public virtual object Resolve(string key, Type service)
		{
			return kernel.Resolve(key, service, null);
		}

		public virtual object Resolve(string key, Type service, IDictionary arguments)
		{
			return kernel.Resolve(key, service, arguments);
		}

		public virtual object Resolve(string key, Type service, object argumentsAsAnonymousType)
		{
			return kernel.Resolve(key, service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		public T Resolve<T>(IDictionary arguments)
		{
			return (T) kernel.Resolve(typeof(T), arguments);
		}

		public T Resolve<T>(object argumentsAsAnonymousType)
		{
			return (T) kernel.Resolve(typeof(T), new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		public virtual T Resolve<T>(string key, IDictionary arguments)
		{
			return (T) kernel.Resolve(key, typeof(T), arguments);
		}

		public virtual T Resolve<T>(string key, object argumentsAsAnonymousType)
		{
			return (T) kernel.Resolve(key, typeof(T), new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		public T Resolve<T>()
		{
			return (T) kernel.Resolve(typeof(T), null);
		}

		public virtual T Resolve<T>(string key)
		{
			return (T) kernel.Resolve(key, typeof(T), null);
		}

		public T[] ResolveAll<T>()
		{
			return (T[]) ResolveAll(typeof(T));
		}

		public Array ResolveAll(Type service)
		{
			return kernel.ResolveAll(service);
		}

		public Array ResolveAll(Type service, IDictionary arguments)
		{
			return kernel.ResolveAll(service, arguments);
		}

		public Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			return ResolveAll(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		public T[] ResolveAll<T>(IDictionary arguments)
		{
			return (T[]) ResolveAll(typeof(T), arguments);
		}

		public T[] ResolveAll<T>(object argumentsAsAnonymousType)
		{
			return ResolveAll<T>(new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		protected virtual void RunInstaller()
		{
			if (Installer != null)
				Installer.SetUp(this, kernel.ConfigurationStore);
		}

		private void Install(IWindsorInstaller[] installers, DefaultComponentInstaller scope)
		{
			using (var store = new PartialConfigurationStore((IKernelInternal) kernel))
			{
				foreach (var windsorInstaller in installers)
					windsorInstaller.Install(this, store);

				scope.SetUp(this, store);
			}
		}
	}
}