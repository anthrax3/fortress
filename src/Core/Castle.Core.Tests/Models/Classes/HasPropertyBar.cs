namespace Castle.Core.Tests
{
	public class HasPropertyBar : IHasPropertyBar
	{
		#region IHasPropertyBar Members

		public int Prop { get; set; }
		public string Bar { get; set; }

		#endregion
	}
}