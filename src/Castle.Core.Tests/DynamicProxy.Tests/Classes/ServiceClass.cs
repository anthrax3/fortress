// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Tests.DynamicProxy.Tests.Classes
{
	public class ServiceClass
	{
		public virtual bool Valid
		{
			get { return false; }
		}

		public bool NonVirtualProp
		{
			get { return false; }
		}

		public virtual int Sum(int b1, int b2)
		{
			return b1 + b2;
		}

		public virtual byte Sum(byte b1, byte b2)
		{
			return Convert.ToByte(b1 + b2);
		}

		public virtual long Sum(long b1, long b2)
		{
			return b1 + b2;
		}

		public virtual short Sum(short b1, short b2)
		{
			return (short) (b1 + b2);
		}

		public virtual float Sum(float b1, float b2)
		{
			return b1 + b2;
		}

		public virtual double Sum(double b1, double b2)
		{
			return b1 + b2;
		}

		public virtual ushort Sum(ushort b1, ushort b2)
		{
			return (ushort) (b1 + b2);
		}

		public virtual uint Sum(uint b1, uint b2)
		{
			return b1 + b2;
		}

		public virtual ulong Sum(ulong b1, ulong b2)
		{
			return b1 + b2;
		}

		public void NonVirtualMethod()
		{
		}

		public class InernalClass
		{
		}
	}
}