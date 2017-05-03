namespace Thinkedge.NodeGroup
{
	public class Node
	{
		public Group Group { get; set; }

		public virtual string ID { get { return null; } }
		public virtual string Type { get { return GetType().Name; } }
	}
}