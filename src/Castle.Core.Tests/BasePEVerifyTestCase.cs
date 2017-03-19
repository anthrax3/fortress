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
using System.Diagnostics;
using Castle.Core.DynamicProxy;
using NUnit.Framework;

namespace Castle.Core.Tests
{
	public abstract class BasePEVerifyTestCase
	{
		protected ProxyGenerator generator;
		protected IProxyBuilder builder;

		private bool verificationDisabled = true;

		[SetUp]
		public virtual void Init()
		{
			ResetGeneratorAndBuilder();
			verificationDisabled = false;
		}

		public void ResetGeneratorAndBuilder()
		{
			builder = new PersistentProxyBuilder();
			generator = new ProxyGenerator(builder);
		}

		public void DisableVerification()
		{
			verificationDisabled = true;
		}

		public bool IsVerificationDisabled
		{
			get { return verificationDisabled; }
		}

		[TearDown]
		public virtual void TearDown()
		{
			// This is currently causing issue when targetting multiple frameworks, taking this out for now. 
			//if (!IsVerificationDisabled)
			//{
			//	// Note: only supports one generated assembly at the moment
			//	var path = ((PersistentProxyBuilder)builder).SaveAssembly();
			//	if (path != null)
			//	{
			//		RunPEVerifyOnGeneratedAssembly(path);
			//	}
			//}
		}

		public void RunPEVerifyOnGeneratedAssembly(string assemblyPath)
		{
			var process = new Process
			{
				StartInfo =
					{
						FileName = FindPeVerify.PeVerifyPath,
						RedirectStandardOutput = true,
						UseShellExecute = false,
						WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
						Arguments = "\"" + assemblyPath + "\" /VERBOSE",
						CreateNoWindow = true
					}
			};
			process.Start();
			var processOutput = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			var result = process.ExitCode + " code ";

			Console.WriteLine(GetType().FullName + ": " + result);

			if (process.ExitCode != 0)
			{
				Console.WriteLine(processOutput);
				Assert.Fail("PeVerify reported error(s): " + Environment.NewLine + processOutput, result);
			}
		}
		
	}
}
