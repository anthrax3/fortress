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
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.DynamicProxy.Generators.Emitters;
using Castle.Core.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Castle.Core.DynamicProxy.Contributors
{
	internal class ClassProxyWithTargetInstanceContributor : ClassProxyInstanceContributor
	{
		public ClassProxyWithTargetInstanceContributor(Type targetType, IList<MethodInfo> methodsToSkip,
			Type[] interfaces, string typeId)
			: base(targetType, methodsToSkip, interfaces, typeId)
		{
		}

		protected override Expression GetTargetReferenceExpression(ClassEmitter emitter)
		{
			return emitter.GetField("__target").ToExpression();
		}
	}
}