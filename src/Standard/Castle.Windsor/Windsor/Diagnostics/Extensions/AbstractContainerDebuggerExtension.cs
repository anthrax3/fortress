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

using System.Collections.Generic;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.Windsor.Diagnostics.DebuggerViews;

namespace Castle.Windsor.Windsor.Diagnostics.Extensions
{
	public abstract class AbstractContainerDebuggerExtension : IContainerDebuggerExtension
	{
		public abstract IEnumerable<DebuggerViewItem> Attach();

		public abstract void Init(IKernel kernel, IDiagnosticsHost diagnosticsHost);

		protected ComponentDebuggerView DefaultComponentView(IHandler handler)
		{
			return DefaultComponentView(handler, null);
		}

		protected ComponentDebuggerView DefaultComponentView(IHandler handler, string description)
		{
			return ComponentDebuggerView.BuildFor(handler, description);
		}
	}
}