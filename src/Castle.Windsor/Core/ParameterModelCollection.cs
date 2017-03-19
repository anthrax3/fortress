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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Core.Core.Configuration;

namespace Castle.Windsor.Core
{
	[Serializable]
	[DebuggerDisplay("Count = {dictionary.Count}")]
	public class ParameterModelCollection : IEnumerable<ParameterModel>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)] private readonly IDictionary<string, ParameterModel> dictionary =
			new Dictionary<string, ParameterModel>(StringComparer.OrdinalIgnoreCase);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Count
		{
			get { return dictionary.Count; }
		}

		public ParameterModel this[string key]
		{
			get
			{
				ParameterModel result;
				dictionary.TryGetValue(key, out result);
				return result;
			}
		}

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return dictionary.Values.GetEnumerator();
		}

		[DebuggerStepThrough]
		IEnumerator<ParameterModel> IEnumerable<ParameterModel>.GetEnumerator()
		{
			return dictionary.Values.GetEnumerator();
		}

		public void Add(string name, string value)
		{
			Add(name, new ParameterModel(name, value));
		}

		public void Add(string name, IConfiguration configNode)
		{
			Add(name, new ParameterModel(name, configNode));
		}

		private void Add(string key, ParameterModel value)
		{
			try
			{
				dictionary.Add(key, value);
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException(string.Format("Parameter '{0}' already exists.", key), e);
			}
		}
	}
}