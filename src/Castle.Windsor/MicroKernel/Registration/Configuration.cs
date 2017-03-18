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

namespace Castle.MicroKernel.Registration
{
	using System;

	#region Node

	public abstract class Node
	{
		private readonly String name;

		protected Node(String name)
		{
			this.name = name;
		}

		protected string Name
		{
			get { return name; }
		}

		public abstract void ApplyTo(IConfiguration configuration);
	}

	#endregion

	#region Attribute

	public class Attrib : Node
	{
		private readonly String value;

		internal Attrib(String name, String value)
			: base(name)
		{
			this.value = value;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			configuration.Attributes.Add(Name, value);
		}

		public static NamedAttribute ForName(String name)
		{
			return new NamedAttribute(name);
		}
	}

	#endregion

	#region NamedChild

	public class NamedAttribute
	{
		private readonly String name;

		internal NamedAttribute(String name)
		{
			this.name = name;
		}

		public Attrib Eq(String value)
		{
			return new Attrib(name, value);
		}

		public Attrib Eq(object value)
		{
			var valueStr = (value != null) ? value.ToString() : String.Empty;
			return new Attrib(name, valueStr);
		}
	}

	#endregion

	#region Child 

	public abstract class Child
	{
		public static NamedChild ForName(String name)
		{
			return new NamedChild(name);
		}
	}

	#endregion

	#region NamedChild

	public class NamedChild : Node
	{
		internal NamedChild(String name)
			: base(name)
		{
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name);
			configuration.Children.Add(node);
		}

		public SimpleChild Eq(String value)
		{
			return new SimpleChild(Name, value);
		}

		public SimpleChild Eq(object value)
		{
			var valueStr = (value != null) ? value.ToString() : String.Empty;
			return new SimpleChild(Name, valueStr);
		}

		public ComplexChild Eq(IConfiguration configNode)
		{
			return new ComplexChild(Name, configNode);
		}

		public CompoundChild Eq(params Node[] childNodes)
		{
			return new CompoundChild(Name, childNodes);
		}
	}

	#endregion

	#region SimpleChild

	public class SimpleChild : Node
	{
		private readonly String value;

		internal SimpleChild(String name, String value)
			: base(name)
		{
			this.value = value;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name, value);
			configuration.Children.Add(node);
		}
	}

	#endregion

	#region ComplexChild

	public class ComplexChild : Node
	{
		private readonly IConfiguration configNode;

		internal ComplexChild(String name, IConfiguration configNode)
			: base(name)
		{
			this.configNode = configNode;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name);
			node.Children.Add(configNode);
			configuration.Children.Add(node);
		}
	}

	#endregion

	#region CompoundChild

	public class CompoundChild : Node
	{
		private readonly Node[] childNodes;

		internal CompoundChild(String name, Node[] childNodes)
			: base(name)
		{
			this.childNodes = childNodes;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name);
			foreach (var childNode in childNodes)
			{
				childNode.ApplyTo(node);
			}
			configuration.Children.Add(node);
		}
	}

	#endregion
}
