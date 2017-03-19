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
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Castle.Core.DynamicProxy.Generators.Emitters
{
	public sealed class StindOpCodesDictionary : Dictionary<Type, OpCode>
	{
		// has to be assigned explicitly to suppress compiler warning

		private StindOpCodesDictionary()
		{
			Add(typeof(bool), OpCodes.Stind_I1);
			Add(typeof(char), OpCodes.Stind_I2);
			Add(typeof(sbyte), OpCodes.Stind_I1);
			Add(typeof(short), OpCodes.Stind_I2);
			Add(typeof(int), OpCodes.Stind_I4);
			Add(typeof(long), OpCodes.Stind_I8);
			Add(typeof(float), OpCodes.Stind_R4);
			Add(typeof(double), OpCodes.Stind_R8);
			Add(typeof(byte), OpCodes.Stind_I1);
			Add(typeof(ushort), OpCodes.Stind_I2);
			Add(typeof(uint), OpCodes.Stind_I4);
			Add(typeof(ulong), OpCodes.Stind_I8);
		}

		public new OpCode this[Type type]
		{
			get
			{
				if (ContainsKey(type))
					return base[type];
				return EmptyOpCode;
			}
		}

		public static OpCode EmptyOpCode { get; } = new OpCode();

		public static StindOpCodesDictionary Instance { get; } = new StindOpCodesDictionary();
	}
}