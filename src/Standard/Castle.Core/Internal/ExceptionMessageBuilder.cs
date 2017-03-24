// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
using System.Linq;
using System.Reflection;
using Castle.Core.DynamicProxy.Generators.Emitters;

namespace Castle.Core.Core.Internal
{
	public static class ExceptionMessageBuilder
	{
		public static string CreateMessageForInaccessibleType(Type inaccessibleType, Type typeToProxy)
		{
			var targetAssembly = typeToProxy.GetTypeInfo().Assembly;

			var strongNamedOrNotIndicator = " not"; // assume not strong-named
			var assemblyToBeVisibleTo = "\"DynamicProxyGenAssembly2\""; // appropriate for non-strong-named

			if (targetAssembly.IsAssemblySigned())
			{
				strongNamedOrNotIndicator = "";
				assemblyToBeVisibleTo = ReferencesCastleCore(targetAssembly)
					? assemblyToBeVisibleTo = "InternalsVisible.ToDynamicProxyGenAssembly2"
					: assemblyToBeVisibleTo = '"' + InternalsVisible.ToDynamicProxyGenAssembly2 + '"';
			}

			var inaccessibleTypeDescription = inaccessibleType == typeToProxy
				? "it"
				: "type " + inaccessibleType.GetBestName();

			var messageFormat =
				"Can not create proxy for type {0} because {1} is not accessible. " +
				"Make it public, or internal and mark your assembly with " +
				"[assembly: InternalsVisibleTo({2})] attribute, because assembly {3} " +
				"is{4} strong-named.";

			return string.Format(messageFormat,
				typeToProxy.GetBestName(),
				inaccessibleTypeDescription,
				assemblyToBeVisibleTo,
				GetAssemblyName(targetAssembly),
				strongNamedOrNotIndicator);
		}

		private static string GetAssemblyName(Assembly targetAssembly)
		{
			return targetAssembly.GetName().Name;
		}

		private static bool ReferencesCastleCore(Assembly inspectedAssembly)
		{
		    return false;
		}
	}
}