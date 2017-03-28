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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using Castle.Core.Internal;
using Castle.DynamicProxy.Generators;

namespace Castle.DynamicProxy
{
	public class ModuleScope
	{
        private readonly object synchronise = new object();

        private readonly bool disableSignedModule;

		private readonly string strongAssemblyName;

		private readonly string strongModulePath;

		private readonly string weakAssemblyName;

		private readonly string weakModulePath;

	    private readonly Dictionary<CacheKey, Type> typeCache = new Dictionary<CacheKey, Type>();

        public ModuleScope() : this(false, false)
		{
		}

		public ModuleScope(bool savePhysicalAssembly) : this(savePhysicalAssembly, false)
		{
		}

		public ModuleScope(bool savePhysicalAssembly, bool disableSignedModule)
		{
            this.disableSignedModule = disableSignedModule;

            NamingScope = new NamingScope();

            this.strongAssemblyName = ModuleScopeAssemblyNaming.GetAssemblyName();

            this.strongModulePath = ModuleScopeAssemblyNaming.GetFileName();

            this.weakAssemblyName = ModuleScopeAssemblyNaming.GetAssemblyName();

            this.weakModulePath = ModuleScopeAssemblyNaming.GetFileName();
        }

        public INamingScope NamingScope { get; }

		public Lock Lock { get; } = Lock.Create();

		public ModuleBuilder StrongNamedModule { get; private set; }

	    public string StrongNamedModuleName => Path.GetFileName(strongModulePath);

        public string StrongNamedModulePath => strongModulePath;

	    public string StrongAssemblyName => strongAssemblyName;

        public string StrongNamedModuleDirectory
		{
			get
			{
				var directory = Path.GetDirectoryName(strongModulePath);
				if (string.IsNullOrEmpty(directory))
					return null;
				return directory;
			}
		}

		public ModuleBuilder WeakNamedModule { get; private set; }

		public string WeakNamedModuleName => Path.GetFileName(weakModulePath);

        public string WeakNamedModulePath => weakModulePath;

        public string WeakAssemblyName => weakAssemblyName;

        public string WeakNamedModuleDirectory
		{
			get
			{
				var directory = Path.GetDirectoryName(weakModulePath);
				if (directory == string.Empty)
					return null;
				return directory;
			}
		}

		public Type GetFromCache(CacheKey key)
		{
			Type type;
			typeCache.TryGetValue(key, out type);
			return type;
		}

		public void RegisterInCache(CacheKey key, Type type)
		{
			typeCache[key] = type;
		}

		public static byte[] GetKeyPair()
		{
			using (var stream = typeof(ModuleScope).GetTypeInfo().Assembly.GetManifestResourceStream("Castle.Core.DynamicProxy.DynProxy.snk"))
			{
				if (stream == null)
					throw new MissingManifestResourceException("Should have a Castle.Core.DynamicProxy.DynProxy.snk as an embedded resource, so Dynamic Proxy could sign generated assembly");
				var length = (int) stream.Length;
				var keyPair = new byte[length];
				stream.Read(keyPair, 0, length);
				return keyPair;
			}
		}

		public ModuleBuilder ObtainDynamicModule(bool isStrongNamed)
		{
			if (isStrongNamed)
				return ObtainDynamicModuleWithStrongName();

			return ObtainDynamicModuleWithWeakName();
		}

		public ModuleBuilder ObtainDynamicModuleWithStrongName()
		{
			if (disableSignedModule)
				throw new InvalidOperationException("Usage of signed module has been disabled. Use unsigned module or enable signed module.");

			if (StrongNamedModule == null)
				StrongNamedModule = CreateModule(true);

			return StrongNamedModule;
		}

		public ModuleBuilder ObtainDynamicModuleWithWeakName()
		{
			if (WeakNamedModule == null)
				WeakNamedModule = CreateModule(false);
			return WeakNamedModule;
		}

		private ModuleBuilder CreateModule(bool signStrongName)
		{
		    lock (synchronise)
		    {
		        var assemblyName = GetAssemblyName(signStrongName);

		        var moduleName = signStrongName ? StrongNamedModuleName : WeakNamedModuleName;

		        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

		        var module = assemblyBuilder.DefineDynamicModule(moduleName);

		        return module;
		    }
		}

		private AssemblyName GetAssemblyName(bool signStrongName)
		{
			var assemblyName = new AssemblyName
			{
				Name = signStrongName ? strongAssemblyName : weakAssemblyName
			};

			if (signStrongName)
			{
                assemblyName.SetPublicKey(InternalsVisible.DynamicProxyGenAssembly2PublicKey);
            }

			return assemblyName;
		}

		public TypeBuilder DefineType(bool inSignedModulePreferably, string name, TypeAttributes flags)
		{
			var module = ObtainDynamicModule(disableSignedModule == false && inSignedModulePreferably);
			return module.DefineType(name, flags);
		}
	}
}