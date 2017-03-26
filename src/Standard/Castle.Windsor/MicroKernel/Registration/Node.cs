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

using Castle.Core.Core.Configuration;

namespace Castle.Windsor.MicroKernel.Registration
{

	#region Node

	public abstract class Node
	{
		protected Node(string name)
		{
			Name = name;
		}

		protected string Name { get; }

		public abstract void ApplyTo(IConfiguration configuration);
	}

	#endregion

	#region Attribute

	#endregion

	#region NamedChild

	#endregion

	#region Child 

	#endregion

	#region NamedChild

	#endregion

	#region SimpleChild

	#endregion

	#region ComplexChild

	#endregion

	#region CompoundChild

	#endregion
}