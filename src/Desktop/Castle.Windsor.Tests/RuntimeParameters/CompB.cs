using Castle.Windsor.Core;

namespace Castle.Windsor.Tests.RuntimeParameters
{
	[Transient]
	public class CompB
	{
		public CompB(CompA ca, CompC cc, string myArgument)
		{
			Compc = cc;
			MyArgument = myArgument;
		}

		public CompC Compc { get; set; }

		public string MyArgument { get; } = string.Empty;
	}
}