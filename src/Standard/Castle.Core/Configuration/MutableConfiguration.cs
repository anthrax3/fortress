// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Configuration
{
	public class MutableConfiguration : AbstractConfiguration
	{
		public MutableConfiguration(string name) : this(name, null)
		{
		}

		public MutableConfiguration(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public new string Value
		{
			get { return base.Value; }
			set { base.Value = value; }
		}

		public static MutableConfiguration Create(string name)
		{
			return new MutableConfiguration(name);
		}

		public MutableConfiguration Attribute(string name, string value)
		{
			Attributes[name] = value;
			return this;
		}

		public MutableConfiguration CreateChild(string name)
		{
			var child = new MutableConfiguration(name);
			Children.Add(child);
			return child;
		}

		public MutableConfiguration CreateChild(string name, string value)
		{
			var child = new MutableConfiguration(name, value);
			Children.Add(child);
			return child;
		}
	}
}