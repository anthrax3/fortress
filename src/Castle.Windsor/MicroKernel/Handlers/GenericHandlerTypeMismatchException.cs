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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;

	using Castle.Core;

	[Serializable]
	public class GenericHandlerTypeMismatchException : HandlerException
	{
		public GenericHandlerTypeMismatchException(string message, ComponentName name) : base(message, name)
		{
		}

		public GenericHandlerTypeMismatchException(string message, ComponentName name, Exception innerException)
			: base(message, name, innerException)
		{
		}

		public GenericHandlerTypeMismatchException(IEnumerable<Type> argumentsUsed, ComponentModel componentModel, DefaultGenericHandler handler)
			: base(BuildMessage(argumentsUsed.Select(a => a.FullName).ToArray(), componentModel, handler), componentModel.ComponentName)
		{
		}

		public GenericHandlerTypeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private static string BuildMessage(string[] argumentsUsed, ComponentModel componentModel, DefaultGenericHandler handler)
		{
			var message = string.Format(
				"Types {0} don't satisfy generic constraints of implementation type {1} of component '{2}'.",
				string.Join(", ", argumentsUsed), componentModel.Implementation.FullName, handler.ComponentModel.Name);
			if (handler.ImplementationMatchingStrategy == null)
			{
				return message + " This is most likely a bug in your code.";
			}
			return message + string.Format("this is likely a bug in the {0} used ({1})", typeof(IGenericImplementationMatchingStrategy).Name,
			                               handler.ImplementationMatchingStrategy);
		}
	}
}
