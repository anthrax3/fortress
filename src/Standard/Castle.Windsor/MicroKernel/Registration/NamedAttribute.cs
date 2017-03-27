namespace Castle.MicroKernel.Registration
{
	public class NamedAttribute
	{
		private readonly string name;

		internal NamedAttribute(string name)
		{
			this.name = name;
		}

		public Attrib Eq(string value)
		{
			return new Attrib(name, value);
		}

		public Attrib Eq(object value)
		{
			var valueStr = value != null ? value.ToString() : string.Empty;
			return new Attrib(name, valueStr);
		}
	}
}