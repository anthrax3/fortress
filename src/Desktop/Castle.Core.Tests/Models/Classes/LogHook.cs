using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class LogHook : IProxyGenerationHook
	{
		private readonly bool screeningEnabled;
		private readonly Type targetTypeToAssert;

		public LogHook(Type targetTypeToAssert, bool screeningEnabled = false)
		{
			this.targetTypeToAssert = targetTypeToAssert;
			this.screeningEnabled = screeningEnabled;
		}

		public IList<MemberInfo> NonVirtualMembers { get; } = new List<MemberInfo>();

		public IList<MemberInfo> AskedMembers { get; } = new List<MemberInfo>();

		public bool Completed { get; private set; }

		public bool ShouldInterceptMethod(Type type, MethodInfo memberInfo)
		{
			Assert.Equal(targetTypeToAssert, type);

			AskedMembers.Add(memberInfo);

			if (screeningEnabled && memberInfo.Name.StartsWith("Sum"))
				return false;

			return true;
		}

		public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
		{
			Assert.Equal(targetTypeToAssert, type);

			NonVirtualMembers.Add(memberInfo);
		}

		public void MethodsInspected()
		{
			Completed = true;
		}
	}
}