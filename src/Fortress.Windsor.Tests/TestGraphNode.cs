using Castle.Core.Internal;

namespace Castle.Windsor.Tests
{
	public class TestGraphNode : GraphNode
	{
		public TestGraphNode(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}
}