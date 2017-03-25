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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Castle.Core.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class BaseTestCaseTestCase : CoreBaseTestCase
	{
		public override void Dispose()
		{
			ResetGeneratorAndBuilder(); // we call TearDown ourselves in these test cases
			base.Dispose();
		}

		private void FindVerificationErrors()
		{
			var moduleBuilder = generator.ProxyBuilder.ModuleScope.ObtainDynamicModule(true);
			var invalidType = moduleBuilder.DefineType("InvalidType");
			var invalidMethod = invalidType.DefineMethod("InvalidMethod", MethodAttributes.Public);
			invalidMethod.GetILGenerator().Emit(OpCodes.Ldnull); // missing RET statement

			invalidType.CreateTypeInfo().AsType();

			if (!IsVerificationDisabled)
				Console.WriteLine("This next test case is expected to yield a verification error.");

			base.Dispose();
		}

		[Fact]
		public void DisableVerification_DisablesVerificationForTestCase()
		{
			DisableVerification();

			FindVerificationErrors();
		}

		[Fact]
		public void DisableVerification_ResetInNextTestCase1()
		{
			Assert.False(IsVerificationDisabled);
			DisableVerification();
			Assert.True(IsVerificationDisabled);
		}

		[Fact]
		public void DisableVerification_ResetInNextTestCase2()
		{
			Assert.False(IsVerificationDisabled);
			DisableVerification();
			Assert.True(IsVerificationDisabled);
		}

		[Fact]
		public void TearDown_DoesNotSaveAnything_IfNoProxyGenerated()
		{
			var path = ModuleScopeAssemblyNaming.GetCurrentFileName();

			if (File.Exists(path))
				File.Delete(path);

			base.Dispose();

			Assert.False(File.Exists(path));
		}
	}
}