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

namespace Castle.Windsor.Core
{
	[Serializable]
	public class ParameterModel
	{
		private readonly IConfiguration configValue;
		private readonly String name;
		private readonly String value;

		public ParameterModel(String name, String value)
		{
			this.name = name;
			this.value = value;
		}

		public ParameterModel(String name, IConfiguration value)
		{
			this.name = name;
			configValue = value;
		}

		public IConfiguration ConfigValue
		{
			get { return configValue; }
		}

		public String Name
		{
			get { return name; }
		}

		public String Value
		{
			get { return value; }
		}
	}
}
