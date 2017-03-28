namespace Castle.Windsor.Tests
{
	public class CImpl : IC
	{
		private R r;

		public CImpl()
		{
			N = null;
		}

		public R R
		{
			set { r = value; }
		}

		public IN N { get; set; }
	}
}