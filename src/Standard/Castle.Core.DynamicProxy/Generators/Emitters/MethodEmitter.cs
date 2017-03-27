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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Generators.Emitters
{
	[DebuggerDisplay("{MethodBuilder.Name}")]
	public class MethodEmitter : IMemberEmitter
	{
		private MethodCodeBuilder codebuilder;

		protected internal MethodEmitter(MethodBuilder builder)
		{
			MethodBuilder = builder;
		}

		internal MethodEmitter(AbstractTypeEmitter owner, string name, MethodAttributes attributes)
			: this(owner.TypeBuilder.DefineMethod(name, attributes))
		{
		}

		internal MethodEmitter(AbstractTypeEmitter owner, string name,
			MethodAttributes attributes, Type returnType,
			params Type[] argumentTypes)
			: this(owner, name, attributes)
		{
			SetParameters(argumentTypes);
			SetReturnType(returnType);
		}

		internal MethodEmitter(AbstractTypeEmitter owner, string name,
			MethodAttributes attributes, MethodInfo methodToUseAsATemplate)
			: this(owner, name, attributes)
		{
			var name2GenericType = GenericUtil.GetGenericArgumentsMap(owner);

			var returnType = GenericUtil.ExtractCorrectType(methodToUseAsATemplate.ReturnType, name2GenericType);
			var baseMethodParameters = methodToUseAsATemplate.GetParameters();
			var parameters = GenericUtil.ExtractParametersTypes(baseMethodParameters, name2GenericType);

			GenericTypeParams = GenericUtil.CopyGenericArguments(methodToUseAsATemplate, MethodBuilder, name2GenericType);
			SetParameters(parameters);
			SetReturnType(returnType);
			SetSignature(returnType, methodToUseAsATemplate.ReturnParameter, parameters, baseMethodParameters);
			DefineParameters(baseMethodParameters);
		}

		public ArgumentReference[] Arguments { get; private set; }

		public virtual MethodCodeBuilder CodeBuilder
		{
			get
			{
				if (codebuilder == null)
					codebuilder = new MethodCodeBuilder(MethodBuilder.GetILGenerator());
				return codebuilder;
			}
		}

		public GenericTypeParameterBuilder[] GenericTypeParams { get; }

		public MethodBuilder MethodBuilder { get; }

		private bool ImplementedByRuntime
		{
			get
			{
				var attributes = MethodBuilder.GetMethodImplementationFlags();
				return (attributes & MethodImplAttributes.Runtime) != 0;
			}
		}

		public MemberInfo Member
		{
			get { return MethodBuilder; }
		}

		public Type ReturnType
		{
			get { return MethodBuilder.ReturnType; }
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (ImplementedByRuntime == false && CodeBuilder.IsEmpty)
			{
				CodeBuilder.AddStatement(new NopStatement());
				CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		public virtual void Generate()
		{
			if (ImplementedByRuntime)
				return;

			codebuilder.Generate(this, MethodBuilder.GetILGenerator());
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			MethodBuilder.SetCustomAttribute(attribute);
		}

		public void SetParameters(Type[] paramTypes)
		{
			MethodBuilder.SetParameters(paramTypes);
			Arguments = ArgumentsUtil.ConvertToArgumentReference(paramTypes);
			ArgumentsUtil.InitializeArgumentsByPosition(Arguments, MethodBuilder.IsStatic);
		}

		private void DefineParameters(ParameterInfo[] parameters)
		{
			foreach (var parameter in parameters)
			{
				var parameterBuilder = MethodBuilder.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
				foreach (var attribute in parameter.GetNonInheritableAttributes())
					parameterBuilder.SetCustomAttribute(attribute.Builder);
			}
		}

		private void SetReturnType(Type returnType)
		{
			MethodBuilder.SetReturnType(returnType);
		}

		private void SetSignature(Type returnType, ParameterInfo returnParameter, Type[] parameters,
			ParameterInfo[] baseMethodParameters)
		{
			MethodBuilder.SetSignature(
				returnType,
				returnParameter.GetRequiredCustomModifiers(),
				returnParameter.GetOptionalCustomModifiers(),
				parameters,
				baseMethodParameters.Select(x => x.GetRequiredCustomModifiers()).ToArray(),
				baseMethodParameters.Select(x => x.GetOptionalCustomModifiers()).ToArray()
			);
		}
	}
}