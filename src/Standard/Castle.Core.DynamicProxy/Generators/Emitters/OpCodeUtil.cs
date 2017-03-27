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
using System.Reflection;
using System.Reflection.Emit;

namespace Castle.DynamicProxy.Generators.Emitters
{
	internal abstract class OpCodeUtil
	{
		public static void EmitLoadIndirectOpCodeForType(ILGenerator gen, Type type)
		{
			if (type.GetTypeInfo().IsEnum)
			{
				EmitLoadIndirectOpCodeForType(gen, GetUnderlyingTypeOfEnum(type));
				return;
			}

			if (type.GetTypeInfo().IsByRef)
				throw new NotSupportedException("Cannot load ByRef values");
			if (type.GetTypeInfo().IsPrimitive && type != typeof(IntPtr) && type != typeof(UIntPtr))
			{
				var opCode = LdindOpCodesDictionary.Instance[type];

				if (opCode == LdindOpCodesDictionary.EmptyOpCode)
					throw new ArgumentException("Type " + type + " could not be converted to a OpCode");

				gen.Emit(opCode);
			}
			else if (type.GetTypeInfo().IsValueType)
			{
				gen.Emit(OpCodes.Ldobj, type);
			}
			else if (type.GetTypeInfo().IsGenericParameter)
			{
				gen.Emit(OpCodes.Ldobj, type);
			}
			else
			{
				gen.Emit(OpCodes.Ldind_Ref);
			}
		}

		public static void EmitLoadOpCodeForConstantValue(ILGenerator gen, object value)
		{
			if (value is string)
			{
				gen.Emit(OpCodes.Ldstr, value.ToString());
			}
			else if (value is int)
			{
				var code = LdcOpCodesDictionary.Instance[value.GetType()];
				gen.Emit(code, (int) value);
			}
			else if (value is bool)
			{
				var code = LdcOpCodesDictionary.Instance[value.GetType()];
				gen.Emit(code, Convert.ToInt32(value));
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public static void EmitLoadOpCodeForDefaultValueOfType(ILGenerator gen, Type type)
		{
			if (type.GetTypeInfo().IsPrimitive)
			{
				var opCode = LdcOpCodesDictionary.Instance[type];
				switch (opCode.StackBehaviourPush)
				{
					case StackBehaviour.Pushi:
						gen.Emit(opCode, 0);
						if (Is64BitTypeLoadedAsInt32(type))
							gen.Emit(OpCodes.Conv_I8);
						break;
					case StackBehaviour.Pushr8:
						gen.Emit(opCode, 0D);
						break;
					case StackBehaviour.Pushi8:
						gen.Emit(opCode, 0L);
						break;
					case StackBehaviour.Pushr4:
						gen.Emit(opCode, 0F);
						break;
					default:
						throw new NotSupportedException();
				}
			}
			else
			{
				gen.Emit(OpCodes.Ldnull);
			}
		}

		public static void EmitStoreIndirectOpCodeForType(ILGenerator gen, Type type)
		{
			if (type.GetTypeInfo().IsEnum)
			{
				EmitStoreIndirectOpCodeForType(gen, GetUnderlyingTypeOfEnum(type));
				return;
			}

			if (type.GetTypeInfo().IsByRef)
			{
				throw new NotSupportedException("Cannot store ByRef values");
			}
			if (type.GetTypeInfo().IsPrimitive && type != typeof(IntPtr) && type != typeof(UIntPtr))
			{
				var opCode = StindOpCodesDictionary.Instance[type];

				if (Equals(opCode, StindOpCodesDictionary.EmptyOpCode))
					throw new ArgumentException("Type " + type + " could not be converted to a OpCode");

				gen.Emit(opCode);
			}
			else if (type.GetTypeInfo().IsValueType)
			{
				gen.Emit(OpCodes.Stobj, type);
			}
			else if (type.GetTypeInfo().IsGenericParameter)
			{
				gen.Emit(OpCodes.Stobj, type);
			}
			else
			{
				gen.Emit(OpCodes.Stind_Ref);
			}
		}

		private static Type GetUnderlyingTypeOfEnum(Type enumType)
		{
			var baseType = (IConvertible) Activator.CreateInstance(enumType);
			var code = baseType.GetTypeCode();

			switch (code)
			{
				case TypeCode.SByte:
					return typeof(sbyte);
				case TypeCode.Byte:
					return typeof(byte);
				case TypeCode.Int16:
					return typeof(short);
				case TypeCode.Int32:
					return typeof(int);
				case TypeCode.Int64:
					return typeof(long);
				case TypeCode.UInt16:
					return typeof(ushort);
				case TypeCode.UInt32:
					return typeof(uint);
				case TypeCode.UInt64:
					return typeof(ulong);
				default:
					throw new NotSupportedException();
			}
		}

		private static bool Is64BitTypeLoadedAsInt32(Type type)
		{
			return type == typeof(long) || type == typeof(ulong);
		}
	}
}