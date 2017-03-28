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
using System.Globalization;

namespace Castle.Core.Configuration
{
	public abstract class AbstractConfiguration : IConfiguration
	{
		public virtual ConfigurationAttributeCollection Attributes { get; } = new ConfigurationAttributeCollection();

		public virtual ConfigurationCollection Children { get; } = new ConfigurationCollection();

		public string Name { get; protected set; }

		public string Value { get; protected set; }

		public virtual object GetValue(Type type, object defaultValue)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			try
			{
				return Convert.ChangeType(Value, type, CultureInfo.CurrentCulture);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}
	}
}