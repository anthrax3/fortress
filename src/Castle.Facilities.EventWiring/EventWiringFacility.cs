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

using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Facilities;

namespace Castle.Facilities.EventWiring
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Castle.Core;

	public class EventWiringFacility : AbstractFacility
	{
		internal const string SubscriberList = "evts.subscriber.list";

		protected override void Init()
		{
			Kernel.ComponentModelBuilder.AddContributor(new EventWiringInspector());
			Kernel.ComponentCreated += OnComponentCreated;
			Kernel.ComponentDestroyed += OnComponentDestroyed;
		}

		private bool IsPublisher(ComponentModel model)
		{
			return model.ExtendedProperties[SubscriberList] != null;
		}

		private void OnComponentCreated(ComponentModel model, object instance)
		{
			if (IsPublisher(model))
			{
				WirePublisher(model, instance);
			}
		}

		private void OnComponentDestroyed(ComponentModel model, object instance)
		{
			// TODO: Remove Listener
		}

		private void StartAndWirePublisherSubscribers(ComponentModel model, object publisher)
		{
			var subscribers = (IDictionary)model.ExtendedProperties[SubscriberList];

			if (subscribers == null)
			{
				return;
			}

			foreach (DictionaryEntry subscriberInfo in subscribers)
			{
				var subscriberKey = (string)subscriberInfo.Key;

				var wireInfoList = (IList)subscriberInfo.Value;

				var handler = Kernel.GetHandler(subscriberKey);

				AssertValidHandler(handler, subscriberKey);

				object subscriberInstance;

				try
				{
					subscriberInstance = Kernel.Resolve<object>(subscriberKey);
				}
				catch (Exception ex)
				{
					throw new EventWiringException("Failed to start subscriber " + subscriberKey, ex);
				}

				var publisherType = publisher.GetType();

				foreach (WireInfo wireInfo in wireInfoList)
				{
					var eventName = wireInfo.EventName;

					//TODO: Caching of EventInfos.
					var eventInfo = publisherType.GetEvent(eventName,
					                                       BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if (eventInfo == null)
					{
						throw new EventWiringException(
							string.Format("Could not find event '{0}' on component '{1}'. Make sure you didn't misspell the name.", eventName, model.Name));
					}

					var handlerMethod = subscriberInstance.GetType().GetMethod(wireInfo.Handler,
					                                                           BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if (handlerMethod == null)
					{
						throw new EventWiringException(
							string.Format(
								"Could not find method '{0}' on component '{1}' to handle event '{2}' published by component '{3}'. Make sure you didn't misspell the name.",
								wireInfo.Handler,
								subscriberKey,
								eventName,
								model.Name));
					}

					var delegateHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, subscriberInstance, wireInfo.Handler);

					eventInfo.AddEventHandler(publisher, delegateHandler);
				}
			}
		}

		private void WirePublisher(ComponentModel model, object publisher)
		{
			StartAndWirePublisherSubscribers(model, publisher);
		}

		private static void AssertValidHandler(IHandler handler, string subscriberKey)
		{
			if (handler == null)
			{
				throw new EventWiringException("Publisher tried to start subscriber " + subscriberKey + " that was not found");
			}

			if (handler.CurrentState == HandlerState.WaitingDependency)
			{
				throw new EventWiringException("Publisher tried to start subscriber " + subscriberKey + " that is waiting for a dependency");
			}
		}
	}

	internal class WireInfo
	{
		private readonly String eventName;

		private readonly String handler;

		public WireInfo(string eventName, string handler)
		{
			this.eventName = eventName;
			this.handler = handler;
		}

		public string EventName
		{
			get { return eventName; }
		}

		public string Handler
		{
			get { return handler; }
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}

			var wireInfo = obj as WireInfo;
			if (wireInfo == null)
			{
				return false;
			}

			if (!Equals(eventName, wireInfo.eventName))
			{
				return false;
			}

			if (!Equals(handler, wireInfo.handler))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return eventName.GetHashCode() + 29*handler.GetHashCode();
		}
	}
}
