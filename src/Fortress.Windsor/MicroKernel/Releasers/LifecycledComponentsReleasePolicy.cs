// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
using System.Linq;
using System.Threading;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Windsor.Diagnostics;

namespace Castle.MicroKernel.Releasers
{
	public class LifecycledComponentsReleasePolicy : IReleasePolicy
	{
		private static int instanceId;

		private readonly Dictionary<object, Burden> instance2Burden =
			new Dictionary<object, Burden>(ReferenceEqualityComparer<object>.Instance);

		private readonly Lock @lock = Lock.Create();
		private readonly ITrackedComponentsPerformanceCounter perfCounter;
		private ITrackedComponentsDiagnostic trackedComponentsDiagnostic;

		public LifecycledComponentsReleasePolicy(IKernel kernel)
			: this(GetTrackedComponentsDiagnostic(kernel), null)
		{
		}

		public LifecycledComponentsReleasePolicy(ITrackedComponentsDiagnostic trackedComponentsDiagnostic,
			ITrackedComponentsPerformanceCounter trackedComponentsPerformanceCounter)
		{
			this.trackedComponentsDiagnostic = trackedComponentsDiagnostic;
			perfCounter = trackedComponentsPerformanceCounter ?? NullPerformanceCounter.Instance;

			if (trackedComponentsDiagnostic != null)
				trackedComponentsDiagnostic.TrackedInstancesRequested += trackedComponentsDiagnostic_TrackedInstancesRequested;
		}

		private LifecycledComponentsReleasePolicy(LifecycledComponentsReleasePolicy parent)
			: this(parent.trackedComponentsDiagnostic, parent.perfCounter)
		{
		}

		private Burden[] TrackedObjects
		{
			get
			{
				using (var holder = @lock.ForReading(false))
				{
					if (holder.LockAcquired == false)
					{
						// TODO: that's sad... perhaps we should have waited...? But what do we do now? We're in the debugger. If some thread is keeping the lock
						// we could wait indefinatelly. I guess the best way to proceed is to add a 200ms timepout to accquire the lock, and if not succeeded
						// assume that the other thread just waits and is not going anywhere and go ahead and read this anyway...
					}
					var array = instance2Burden.Values.ToArray();
					return array;
				}
			}
		}

		public void Dispose()
		{
			KeyValuePair<object, Burden>[] burdens;
			using (@lock.ForWriting())
			{
				if (trackedComponentsDiagnostic != null)
				{
					trackedComponentsDiagnostic.TrackedInstancesRequested -= trackedComponentsDiagnostic_TrackedInstancesRequested;
					trackedComponentsDiagnostic = null;
				}
				burdens = instance2Burden.ToArray();
				instance2Burden.Clear();
			}
			// NOTE: This is relying on a undocumented behavior that order of items when enumerating Dictionary<> will be oldest --> latest
			foreach (var burden in burdens.Reverse())
			{
				burden.Value.Released -= OnInstanceReleased;
				perfCounter.DecrementTrackedInstancesCount();
				burden.Value.Release();
			}
		}

		public IReleasePolicy CreateSubPolicy()
		{
			var policy = new LifecycledComponentsReleasePolicy(this);
			return policy;
		}

		public bool HasTrack(object instance)
		{
			if (instance == null)
				return false;

			using (@lock.ForReading())
			{
				return instance2Burden.ContainsKey(instance);
			}
		}

		public void Release(object instance)
		{
			if (instance == null)
				return;

			Burden burden;
			using (@lock.ForWriting())
			{
				// NOTE: we don't physically remove the instance from the instance2Burden collection here.
				// we do it in OnInstanceReleased event handler
				if (instance2Burden.TryGetValue(instance, out burden) == false)
					return;
			}
			burden.Release();
		}

		public virtual void Track(object instance, Burden burden)
		{
			if (burden.RequiresPolicyRelease == false)
			{
				var lifestyle = (object) burden.Model.CustomLifestyle ?? burden.Model.LifestyleType;
				throw new ArgumentException(
					string.Format(
						"Release policy was asked to track object '{0}', but its burden has 'RequiresPolicyRelease' set to false. If object is to be tracked the flag must be true. This is likely a bug in the lifetime manager '{1}'.",
						instance, lifestyle));
			}
			try
			{
				using (@lock.ForWriting())
				{
					instance2Burden.Add(instance, burden);
				}
			}
			catch (ArgumentNullException)
			{
				//eventually we should probably throw something more useful here too
				throw;
			}
			catch (ArgumentException)
			{
				throw HelpfulExceptionsUtil.TrackInstanceCalledMultipleTimes(instance, burden);
			}
			burden.Released += OnInstanceReleased;
			perfCounter.IncrementTrackedInstancesCount();
		}

		private void OnInstanceReleased(Burden burden)
		{
			using (@lock.ForWriting())
			{
				if (instance2Burden.Remove(burden.Instance) == false)
					return;
			}
			burden.Released -= OnInstanceReleased;
			perfCounter.DecrementTrackedInstancesCount();
		}

		private void trackedComponentsDiagnostic_TrackedInstancesRequested(object sender, TrackedInstancesEventArgs e)
		{
			e.AddRange(TrackedObjects);
		}

		public static ITrackedComponentsDiagnostic GetTrackedComponentsDiagnostic(IKernel kernel)
		{
			var diagnosticsHost = (IDiagnosticsHost) kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
			if (diagnosticsHost == null)
				return null;
			return diagnosticsHost.GetDiagnostic<ITrackedComponentsDiagnostic>();
		}

		public static ITrackedComponentsPerformanceCounter GetTrackedComponentsPerformanceCounter(IPerformanceMetricsFactory perfMetricsFactory)
		{
			//var process = Process.GetCurrentProcess();
			var name = string.Format("Instance {0} | process {1} (id:{2})", Interlocked.Increment(ref instanceId), "Unkown Process Name", "Unkown Process Id");
			return perfMetricsFactory.CreateInstancesTrackedByReleasePolicyCounter(name);
		}
	}
}