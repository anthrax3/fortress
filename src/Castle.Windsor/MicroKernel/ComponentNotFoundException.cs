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
using System.Runtime.Serialization;

namespace Castle.Windsor.MicroKernel
{
	[Serializable]
	public class ComponentNotFoundException : ComponentResolutionException
	{
		public ComponentNotFoundException(string name, Type service, int countOfHandlersForTheService)
			: base(BuildMessage(name, service, countOfHandlersForTheService))
		{
			Name = name;
			Service = service;
		}

		public ComponentNotFoundException(string name, string message)
			: base(message)
		{
			Name = name;
		}

		public ComponentNotFoundException(Type service, string message)
			: base(message)
		{
			Service = service;
		}

		public ComponentNotFoundException(Type service) :
			this(service, String.Format("No component for supporting the service {0} was found", service.FullName))
		{
		}

		public ComponentNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public string Name { get; private set; }
		public Type Service { get; private set; }

		private static string BuildMessage(string name, Type service, int countOfHandlersForTheService)
		{
			var message =
				string.Format("Requested component named '{0}' was not found in the container. Did you forget to register it?{1}",
				              name, Environment.NewLine);
			if (countOfHandlersForTheService == 0)
			{
				return message +
				       string.Format(
				       	"There are no components supporting requested service '{0}'. You need to register components in order to be able to use them.",
				       	service.FullName);
			}
			if (countOfHandlersForTheService == 1)
			{
				return message +
				       string.Format(
				       	"There is one other component supporting requested service '{0}'. Is it what you were looking for?",
				       	service.FullName);
			}
			if (countOfHandlersForTheService > 1)
			{
				return message +
				       string.Format(
				       	"There are {0} other components supporting requested service '{1}'. Were you looking for any of them?",
				       	countOfHandlersForTheService, service.FullName);
			}
			// this should never happen but if someone passes us wrong information we just ignore it
			return message;
		}
	}
}
