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

using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	[DebuggerDisplay("{Fieldbuilder.Name} ({fieldbuilder.FieldType})")]
	public class FieldReference : Reference
	{
		private readonly bool isStatic;

		public FieldReference(FieldInfo field)
		{
			Reference = field;
			if ((field.Attributes & FieldAttributes.Static) != 0)
			{
				isStatic = true;
				owner = null;
			}
		}

		public FieldReference(FieldBuilder fieldbuilder)
		{
			Fieldbuilder = fieldbuilder;
			Reference = fieldbuilder;
			if ((fieldbuilder.Attributes & FieldAttributes.Static) != 0)
			{
				isStatic = true;
				owner = null;
			}
		}

		public FieldBuilder Fieldbuilder { get; }

		public FieldInfo Reference { get; }

		public override void LoadAddressOfReference(ILGenerator gen)
		{
			if (isStatic)
				gen.Emit(OpCodes.Ldsflda, Reference);
			else
				gen.Emit(OpCodes.Ldflda, Reference);
		}

		public override void LoadReference(ILGenerator gen)
		{
			if (isStatic)
				gen.Emit(OpCodes.Ldsfld, Reference);
			else
				gen.Emit(OpCodes.Ldfld, Reference);
		}

		public override void StoreReference(ILGenerator gen)
		{
			if (isStatic)
				gen.Emit(OpCodes.Stsfld, Reference);
			else
				gen.Emit(OpCodes.Stfld, Reference);
		}
	}
}