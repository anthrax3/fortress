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
using System.Linq;
using Castle.Windsor.Core;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel.LifecycleConcerns;

namespace Castle.Windsor.MicroKernel.ModelBuilder.Inspectors
{
	public class LifecycleModelInspector : IContributeComponentModelConstruction
	{
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			if (IsLateBoundComponent(model))
			{
				ProcessLateBoundModel(model);
				return;
			}
			ProcessModel(model);
		}

		private bool IsLateBoundComponent(ComponentModel model)
		{
			return typeof(LateBoundComponent) == model.Implementation;
		}

		private void ProcessLateBoundModel(ComponentModel model)
		{
			var commission = new LateBoundCommissionConcerns();
			if (model.Services.Any(s => s.Is<IInitializable>()))
				model.Lifecycle.Add(InitializationConcern.Instance);
			else
				commission.AddConcern<IInitializable>(InitializationConcern.Instance);
			if (model.Services.Any(s => s.Is<ISupportInitialize>()))
				model.Lifecycle.Add(SupportInitializeConcern.Instance);
			else
				commission.AddConcern<ISupportInitialize>(SupportInitializeConcern.Instance);
			if (commission.HasConcerns)
				model.Lifecycle.Add(commission);

			if (model.Services.Any(s => s.Is<IDisposable>()))
			{
				model.Lifecycle.Add(DisposalConcern.Instance);
			}
			else
			{
				var decommission = new LateBoundDecommissionConcerns();
				decommission.AddConcern<IDisposable>(DisposalConcern.Instance);
				model.Lifecycle.Add(decommission);
			}
		}

		private void ProcessModel(ComponentModel model)
		{
			if (model.Implementation.Is<IInitializable>())
				model.Lifecycle.Add(InitializationConcern.Instance);
			if (model.Implementation.Is<ISupportInitialize>())
				model.Lifecycle.Add(SupportInitializeConcern.Instance);
			if (model.Implementation.Is<IDisposable>())
				model.Lifecycle.Add(DisposalConcern.Instance);
		}
	}
}