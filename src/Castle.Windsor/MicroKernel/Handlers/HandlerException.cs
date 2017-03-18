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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Runtime.Serialization;

	using Castle.Core;
	using Castle.Core.Internal;

	[Serializable]
	public class HandlerException : Exception
	{
		public HandlerException(string message, ComponentName name) : base(message)
		{
			ExceptionHelper.SetUp(this);
			Name = name;
		}

		public HandlerException(string message, ComponentName name, Exception innerException)
			: base(message, innerException)
		{
			ExceptionHelper.SetUp(this);
			Name = name;
		}

		public HandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			ExceptionHelper.SetUp(this);
		}

		public ComponentName Name { get; private set; }
	}
}
