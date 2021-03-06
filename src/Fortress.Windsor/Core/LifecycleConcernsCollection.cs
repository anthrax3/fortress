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
using System.Diagnostics;

namespace Castle.Core
{
	public class LifecycleConcernsCollection
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private List<ICommissionConcern> commission;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private List<IDecommissionConcern> decommission;

		public IEnumerable<ICommissionConcern> CommissionConcerns
		{
			get
			{
				if (HasCommissionConcerns == false)
					return new ICommissionConcern[0];
				return commission;
			}
		}

		public IEnumerable<IDecommissionConcern> DecommissionConcerns
		{
			get
			{
				if (HasDecommissionConcerns == false)
					return new IDecommissionConcern[0];
				return decommission;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool HasCommissionConcerns
		{
			get { return commission != null && commission.Count != 0; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool HasDecommissionConcerns
		{
			get { return decommission != null && decommission.Count != 0; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<ICommissionConcern> Commission
		{
			get
			{
				if (commission == null)
					commission = new List<ICommissionConcern>();
				return commission;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<IDecommissionConcern> Decommission
		{
			get
			{
				if (decommission == null)
					decommission = new List<IDecommissionConcern>();
				return decommission;
			}
		}

		public void Add(ICommissionConcern concern)
		{
			if (concern == null)
				throw new ArgumentNullException("concern");
			Commission.Add(concern);
		}

		public void Add(IDecommissionConcern concern)
		{
			if (concern == null)
				throw new ArgumentNullException("concern");

			Decommission.Add(concern);
		}

		public void AddFirst(ICommissionConcern concern)
		{
			if (concern == null)
				throw new ArgumentNullException("concern");
			Commission.Insert(0, concern);
		}

		public void AddFirst(IDecommissionConcern concern)
		{
			if (concern == null)
				throw new ArgumentNullException("concern");

			Decommission.Insert(0, concern);
		}
	}
}