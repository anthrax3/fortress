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
using Castle.Core.Internal;

namespace Castle.MicroKernel.SubSystems.Conversion
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class ConvertibleAttribute : Attribute
	{
		public ConvertibleAttribute() : this(typeof(DefaultComplexConverter))
		{
		}

		public ConvertibleAttribute(Type converterType)
		{
			if (converterType.Is<ITypeConverter>() == false)
				throw new ArgumentException(
					string.Format("ConverterType {0} does not implement {1} interface", converterType.FullName,
						typeof(ITypeConverter).FullName), "converterType");

			ConverterType = converterType;
		}

		public Type ConverterType { get; }
	}
}