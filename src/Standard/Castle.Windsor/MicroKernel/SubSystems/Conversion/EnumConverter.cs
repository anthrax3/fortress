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
using Castle.Core.Core.Configuration;

namespace Castle.Windsor.MicroKernel.SubSystems.Conversion
{
	public class EnumConverter : AbstractTypeConverter
	{
		public override bool CanHandleType(Type type)
		{
			return type.GetTypeInfo().IsEnum;
		}

		public override object PerformConversion(string value, Type targetType)
		{
			try
			{
				return Enum.Parse(targetType, value, true);
			}
			catch (ConverterException)
			{
				throw;
			}
			catch (Exception ex)
			{
				var message = string.Format(
					"Could not convert from '{0}' to {1}.",
					value, targetType.FullName);

				throw new ConverterException(message, ex);
			}
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			return PerformConversion(configuration.Value, targetType);
		}
	}
}