using System.Collections.Generic;

namespace Thinkedge.NodeGroup
{
	public class Group
	{
		public HashSet<Node> Nodes = new HashSet<Node>();

		public int IO { get; private set; }

		public Group(int id)
		{
			IO = id;
		}

		public void Add(Node node)
		{
			Nodes.Add(node);
		}
	}
}