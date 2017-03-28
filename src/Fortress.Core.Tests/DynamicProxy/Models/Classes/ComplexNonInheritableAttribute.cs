using System;
using System.IO;

namespace Castle.Core.Tests.DynamicProxy.Tests.Classes
{
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public class ComplexNonInheritableAttribute : Attribute
	{
		public FileAccess access;
		public int id, num;
		public bool isSomething;
		public string name;

		public ComplexNonInheritableAttribute(int id, int num, string name)
		{
			this.id = id;
			this.num = num;
			this.name = name;
		}

		public ComplexNonInheritableAttribute(int id, int num, bool isSomething, string name, FileAccess access)
		{
			this.id = id;
			this.num = num;
			this.isSomething = isSomething;
			this.name = name;
			this.access = access;
		}

		public int Id
		{
			get { return id; }
		}

		public int Num
		{
			get { return num; }
		}

		public bool IsSomething
		{
			get { return isSomething; }
			set { isSomething = value; }
		}

		public string Name
		{
			get { return name; }
		}

		public FileAccess Access
		{
			get { return access; }
			set { access = value; }
		}
	}
}