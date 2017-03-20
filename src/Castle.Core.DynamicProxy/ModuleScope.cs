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
using Castle.Core.Core.Internal;
using Castle.Core.DynamicProxy.Generators;
using Castle.Core.DynamicProxy.Serialization;

namespace Castle.Core.DynamicProxy
{
	public class ModuleScopeAssemblyNamingOptions
	{
		public static bool UseAutoNamingConventions = false;
	}

	public class ModuleScopeAssemblyNaming
	{
		public static readonly string DEFAULT_FILE_NAME = "CastleDynProxy2.dll";

		public static readonly string DEFAULT_ASSEMBLY_NAME = "DynamicProxyGenAssembly2";

		private static string _currentFileName = DEFAULT_FILE_NAME;

		private static string _currentAssemblyName = DEFAULT_ASSEMBLY_NAME;

		public static string GetFileName()
		{
			if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventions)
			{
				_currentFileName = $"CastleDynProxy2_{Guid.NewGuid():N}.dll";
				return _currentFileName;
			}
			return DEFAULT_FILE_NAME;
		}

		public static string GetCurrentFileName()
		{
			if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventions)
			{
				return _currentFileName;
			}
			return DEFAULT_FILE_NAME;
		}

		public static string GetAssemblyName()
		{
			if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventions)
			{
				_currentAssemblyName = $"DynamicProxyGenAssembly2_{Guid.NewGuid():N}";
				return _currentAssemblyName;
			}
			return DEFAULT_ASSEMBLY_NAME;
		}

		public static string GetCurrentAssemblyName()
		{
			if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventions)
			{
				return _currentAssemblyName;
			}
			return DEFAULT_ASSEMBLY_NAME;
		}
	}

	public class ModuleScope
	{
		private readonly bool disableSignedModule;

		private readonly object moduleLocker = new object();

		private readonly bool savePhysicalAssembly;

		private readonly string strongAssemblyName;

		private readonly string strongModulePath;

		private readonly Dictionary<CacheKey, Type> typeCache = new Dictionary<CacheKey, Type>();

		private readonly string weakAssemblyName;

		private readonly string weakModulePath;

		public ModuleScope() : this(false, false)
		{
		}

		public ModuleScope(bool savePhysicalAssembly)
			: this(savePhysicalAssembly, false)
		{
		}

		public ModuleScope(bool savePhysicalAssembly, bool disableSignedModule)
			: this(savePhysicalAssembly, disableSignedModule, ModuleScopeAssemblyNaming.GetAssemblyName(), ModuleScopeAssemblyNaming.GetFileName(), ModuleScopeAssemblyNaming.GetAssemblyName(), ModuleScopeAssemblyNaming.GetFileName())
		{
		}

		public ModuleScope(bool savePhysicalAssembly, bool disableSignedModule, string strongAssemblyName, string strongModulePath, string weakAssemblyName, string weakModulePath)
			: this(savePhysicalAssembly, disableSignedModule, new NamingScope(), strongAssemblyName, strongModulePath, weakAssemblyName, weakModulePath)
		{
		}

		public ModuleScope(bool savePhysicalAssembly, bool disableSignedModule, INamingScope namingScope, string strongAssemblyName, string strongModulePath, string weakAssemblyName, string weakModulePath)
		{
			this.savePhysicalAssembly = savePhysicalAssembly;
			this.disableSignedModule = disableSignedModule;
			NamingScope = namingScope;
			this.strongAssemblyName = strongAssemblyName;
			this.strongModulePath = strongModulePath;
			this.weakAssemblyName = weakAssemblyName;
			this.weakModulePath = weakModulePath;
		}

		public INamingScope NamingScope { get; }

		public Lock Lock { get; } = Lock.Create();

		public ModuleBuilder StrongNamedModule { get; private set; }

		public string StrongNamedModuleName
		{
			get { return Path.GetFileName(strongModulePath); }
		}

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

		public string WeakNamedModuleName
		{
			get { return Path.GetFileName(weakModulePath); }
		}

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
					throw new MissingManifestResourceException(
						"Should have a Castle.Core.DynamicProxy.DynProxy.snk as an embedded resource, so Dynamic Proxy could sign generated assembly");

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

			lock (moduleLocker)
			{
				if (StrongNamedModule == null)
					StrongNamedModule = CreateModule(true);
				return StrongNamedModule;
			}
		}

		public ModuleBuilder ObtainDynamicModuleWithWeakName()
		{
			lock (moduleLocker)
			{
				if (WeakNamedModule == null)
					WeakNamedModule = CreateModule(false);
				return WeakNamedModule;
			}
		}

		private ModuleBuilder CreateModule(bool signStrongName)
		{
			var assemblyName = GetAssemblyName(signStrongName);
			var moduleName = signStrongName ? StrongNamedModuleName : WeakNamedModuleName;
			if (savePhysicalAssembly)
			{
				AssemblyBuilder assemblyBuilder;
				try
				{
					assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
						assemblyName, AssemblyBuilderAccess.RunAndSave, signStrongName ? StrongNamedModuleDirectory : WeakNamedModuleDirectory);
				}
				catch (ArgumentException e)
				{
					if (signStrongName == false && e.StackTrace.Contains("ComputePublicKey") == false)
						throw;
					var message = $"There was an error creating dynamic assembly for your proxies - you don\'t have permissions required to sign the assembly. To workaround it you can enforce generating non-signed assembly only when creating {GetType()}. Alternatively ensure that your account has all the required permissions.";
					throw new ArgumentException(message, e);
				}
				var module = assemblyBuilder.DefineDynamicModule(moduleName, moduleName, false);
				return module;
			}
			else
			{
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					assemblyName, AssemblyBuilderAccess.Run);

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
				var keyPairStream = GetKeyPair();
				if (keyPairStream != null)
					assemblyName.KeyPair = new StrongNameKeyPair(keyPairStream);
			}

			return assemblyName;
		}

		public string SaveAssembly()
		{
			if (!savePhysicalAssembly)
				return null;

			if (StrongNamedModule != null && WeakNamedModule != null)
				throw new InvalidOperationException("Both a strong-named and a weak-named assembly have been generated.");

			if (StrongNamedModule != null)
				return SaveAssembly(true);

			if (WeakNamedModule != null)
				return SaveAssembly(false);

			return null;
		}

		public string SaveAssembly(bool strongNamed)
		{
			if (!savePhysicalAssembly)
				return null;

			AssemblyBuilder assemblyBuilder;
			string assemblyFileName;
			string assemblyFilePath;

			if (strongNamed)
			{
				if (StrongNamedModule == null)
					throw new InvalidOperationException("No strong-named assembly has been generated.");
				assemblyBuilder = (AssemblyBuilder) StrongNamedModule.Assembly;
				assemblyFileName = StrongNamedModuleName;
				assemblyFilePath = StrongNamedModule.FullyQualifiedName;
			}
			else
			{
				if (WeakNamedModule == null)
					throw new InvalidOperationException("No weak-named assembly has been generated.");
				assemblyBuilder = (AssemblyBuilder) WeakNamedModule.Assembly;
				assemblyFileName = WeakNamedModuleName;
				assemblyFilePath = WeakNamedModule.FullyQualifiedName;
			}

			if (File.Exists(assemblyFilePath))
				File.Delete(assemblyFilePath);

			AddCacheMappings(assemblyBuilder);

			assemblyBuilder.Save(assemblyFileName);
			return assemblyFilePath;
		}

		private void AddCacheMappings(AssemblyBuilder builder)
		{
			Dictionary<CacheKey, string> mappings;

			using (Lock.ForReading())
			{
				mappings = new Dictionary<CacheKey, string>();
				foreach (var cacheEntry in typeCache)
					if (builder.Equals(cacheEntry.Value.Assembly))
						mappings.Add(cacheEntry.Key, cacheEntry.Value.FullName);
			}

			CacheMappingsAttribute.ApplyTo(builder, mappings);
		}

		public void LoadAssemblyIntoCache(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			var cacheMappings =
				(CacheMappingsAttribute[]) assembly.GetCustomAttributes(typeof(CacheMappingsAttribute), false);

			if (cacheMappings.Length == 0)
			{
				var message = string.Format(
					"The given assembly '{0}' does not contain any cache information for generated types.",
					assembly.FullName);
				throw new ArgumentException(message, nameof(assembly));
			}

			foreach (var mapping in cacheMappings[0].GetDeserializedMappings())
			{
				var loadedType = assembly.GetType(mapping.Value);

				if (loadedType != null)
					RegisterInCache(mapping.Key, loadedType);
			}
		}

		public TypeBuilder DefineType(bool inSignedModulePreferably, string name, TypeAttributes flags)
		{
			var module = ObtainDynamicModule(disableSignedModule == false && inSignedModulePreferably);
			return module.DefineType(name, flags);
		}
	}
}