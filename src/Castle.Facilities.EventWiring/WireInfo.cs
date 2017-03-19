namespace Castle.Facilities.EventWiring
{
	internal class WireInfo
	{
		public WireInfo(string eventName, string handler)
		{
			EventName = eventName;
			Handler = handler;
		}

		public string EventName { get; }

		public string Handler { get; }

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			var wireInfo = obj as WireInfo;
			if (wireInfo == null)
				return false;

			if (!Equals(EventName, wireInfo.EventName))
				return false;

			if (!Equals(Handler, wireInfo.Handler))
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			return EventName.GetHashCode() + 29 * Handler.GetHashCode();
		}
	}
}