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
	public sealed class LdindOpCodesDictionary : Dictionary<Type, OpCode>
	{
		// has to be assigned explicitly to suppress compiler warning

		private LdindOpCodesDictionary()
		{
			Add(typeof(bool), OpCodes.Ldind_I1);
			Add(typeof(char), OpCodes.Ldind_I2);
			Add(typeof(sbyte), OpCodes.Ldind_I1);
			Add(typeof(short), OpCodes.Ldind_I2);
			Add(typeof(int), OpCodes.Ldind_I4);
			Add(typeof(long), OpCodes.Ldind_I8);
			Add(typeof(float), OpCodes.Ldind_R4);
			Add(typeof(double), OpCodes.Ldind_R8);
			Add(typeof(byte), OpCodes.Ldind_U1);
			Add(typeof(ushort), OpCodes.Ldind_U2);
			Add(typeof(uint), OpCodes.Ldind_U4);
			Add(typeof(ulong), OpCodes.Ldind_I8);
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

		public static LdindOpCodesDictionary Instance { get; } = new LdindOpCodesDictionary();
	}
}