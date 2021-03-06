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

using Castle.Core.Configuration;

namespace Castle.MicroKernel.Registration
{
	public class Parameter
	{
		private readonly object value;

		internal Parameter(string key, string value)
		{
			Key = key;
			this.value = value;
		}

		internal Parameter(string key, IConfiguration configNode)
		{
			Key = key;
			value = configNode;
		}

		public IConfiguration ConfigNode
		{
			get { return value as IConfiguration; }
		}

		public string Key { get; }

		public string Value
		{
			get { return value as string; }
		}

		public static ParameterKey ForKey(string key)
		{
			return new ParameterKey(key);
		}

		public static implicit operator Dependency(Parameter parameter)
		{
			return parameter == null ? null : new Dependency(parameter);
		}
	}
}