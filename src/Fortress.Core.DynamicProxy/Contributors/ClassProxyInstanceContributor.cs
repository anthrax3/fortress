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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Contributors
{
	public class ClassProxyInstanceContributor : ProxyInstanceContributor
	{

		public ClassProxyInstanceContributor(Type targetType, IList<MethodInfo> methodsToSkip, Type[] interfaces,
			string typeId)
			: base(targetType, interfaces, typeId)
		{
		}

		protected override Expression GetTargetReferenceExpression(ClassEmitter emitter)
		{
			return SelfReference.Self.ToExpression();
		}

		public override void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			var interceptors = @class.GetField("__interceptors");
			ImplementProxyTargetAccessor(@class, interceptors);
			foreach (var attribute in targetType.GetTypeInfo().GetNonInheritableAttributes())
				@class.DefineCustomAttribute(attribute.Builder);
		}
	}
}