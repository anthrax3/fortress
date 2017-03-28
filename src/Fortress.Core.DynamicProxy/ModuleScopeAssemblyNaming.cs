using System;

namespace Castle.DynamicProxy
{
	public class ModuleScopeAssemblyNaming
	{
		public static readonly string DEFAULT_FILE_NAME = "CastleDynProxy2.dll";

		public static readonly string DEFAULT_ASSEMBLY_NAME = "DynamicProxyGenAssembly2";

		private static string _currentFileName = DEFAULT_FILE_NAME;

		private static string _currentAssemblyName = DEFAULT_ASSEMBLY_NAME;

		public static string GetFileName()
		{
			if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventionsAndDisableFriendAssemblySupport)
			{
				_currentFileName = $"CastleDynProxy2_{Guid.NewGuid():N}.dll";
				return _currentFileName;
			}
			return DEFAULT_FILE_NAME;
		}

		public static string GetCurrentFileName()
		{
            if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventionsAndDisableFriendAssemblySupport)
			{
				return _currentFileName;
			}
			return DEFAULT_FILE_NAME;
		}

		public static string GetAssemblyName()
		{
			if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventionsAndDisableFriendAssemblySupport)
			{
				_currentAssemblyName = $"DynamicProxyGenAssembly2_{Guid.NewGuid():N}";
				return _currentAssemblyName;
			}
			return DEFAULT_ASSEMBLY_NAME;
		}

		public static string GetCurrentAssemblyName()
		{
			if (ModuleScopeAssemblyNamingOptions.UseAutoNamingConventionsAndDisableFriendAssemblySupport)
			{
				return _currentAssemblyName;
			}
			return DEFAULT_ASSEMBLY_NAME;
		}
	}
}