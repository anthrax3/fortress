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
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
	public class BaseTestCaseTestCase : CoreBaseTestCase
	{
		public override void TearDown()
		{
			ResetGeneratorAndBuilder(); // we call TearDown ourselves in these test cases
			base.TearDown();
		}

		private void FindVerificationErrors()
		{
			var moduleBuilder = generator.ProxyBuilder.ModuleScope.ObtainDynamicModule(true);
			var invalidType = moduleBuilder.DefineType("InvalidType");
			var invalidMethod = invalidType.DefineMethod("InvalidMethod", MethodAttributes.Public);
			invalidMethod.GetILGenerator().Emit(OpCodes.Ldnull); // missing RET statement

			invalidType.CreateType();
			invalidType.CreateTypeInfo().AsType();

			if (!IsVerificationDisabled)
				Console.WriteLine("This next test case is expected to yield a verification error.");

			base.TearDown();
		}

		[Test]
		public void DisableVerification_DisablesVerificationForTestCase()
		{
			DisableVerification();

			FindVerificationErrors();
		}

		[Test]
		public void DisableVerification_ResetInNextTestCase1()
		{
			Assert.IsFalse(IsVerificationDisabled);
			DisableVerification();
			Assert.IsTrue(IsVerificationDisabled);
		}

		[Test]
		public void DisableVerification_ResetInNextTestCase2()
		{
			Assert.IsFalse(IsVerificationDisabled);
			DisableVerification();
			Assert.IsTrue(IsVerificationDisabled);
		}

		[Test]
		public void TearDown_DoesNotSaveAnything_IfNoProxyGenerated()
		{
			var path = ModuleScopeAssemblyNaming.GetCurrentFileName();

			if (File.Exists(path))
				File.Delete(path);

			base.TearDown();

			Assert.IsFalse(File.Exists(path));
		}
	}
}