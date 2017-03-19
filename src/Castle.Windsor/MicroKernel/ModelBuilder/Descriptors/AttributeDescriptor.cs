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
using Castle.Core.Core.Configuration;
using Castle.Windsor.MicroKernel.Registration;

namespace Castle.Windsor.MicroKernel.ModelBuilder.Descriptors
{
	public class AttributeDescriptor<S> : AbstractOverwriteableDescriptor<S>
		where S : class
	{
		private readonly String name;
		private readonly String value;

		public AttributeDescriptor(String name, String value)
		{
			this.name = name;
			this.value = value;
		}

		protected override void ApplyToConfiguration(IKernel kernel, IConfiguration configuration)
		{
			if (configuration.Attributes[name] == null || IsOverWrite)
			{
				configuration.Attributes[name] = value;
			}
		}
	}

	public class AttributeKeyDescriptor<S>
		where S : class
	{
		private readonly ComponentRegistration<S> component;
		private readonly String name;

		public AttributeKeyDescriptor(ComponentRegistration<S> component, String name)
		{
			this.component = component;
			this.name = name;
		}

		public ComponentRegistration<S> Eq(Object value)
		{
			var attribValue = (value != null) ? value.ToString() : "";
			return component.AddAttributeDescriptor(name, attribValue);
		}
	}
}
