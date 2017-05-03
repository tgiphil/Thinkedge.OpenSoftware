using System.Collections.Generic;

namespace Thinkedge.NodeGroupV2
{
	public class Group
	{
		public HashSet<Node> Nodes = new HashSet<Node>();

		public int Id { get; private set; }

		public Group(int id)
		{
			Id = id;
		}

		public void Add(Node node)
		{
			Nodes.Add(node);
		}
	}
}