namespace Thinkedge.NodeGroupV2
{
	public class Node
	{
		public string ID { get; set; }
		public string Type { get; set; }

		public Group Group { get; set; }

		public Node(string id, string type)
		{
			ID = id;
			Type = type;
		}
	}
}