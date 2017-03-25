namespace Castle.Core.Tests.GenClasses
{
	public class SubClassWithGenArgs<T, Z, Y> : ClassWithGenArgs<T, Z>
	{
		public override void DoSomething()
		{
#pragma warning disable 219
#pragma warning disable 168
			var x = 1 + 10; // Just something to fool the compiler
#pragma warning restore 168
#pragma warning restore 219
			base.DoSomething();
		}
	}
}