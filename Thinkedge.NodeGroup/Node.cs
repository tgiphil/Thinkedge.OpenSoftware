namespace Thinkedge.NodeGroup
{
	public class Node
	{
		public Group Group { get; set; }

		public virtual string ID { get { return null; } }
		public virtual string Type { get { return GetType().Name; } }

		public virtual string GraphwizName { get { return null; } }
		public virtual string GraphwizLabel { get { return null; } }
		public virtual string GraphwizConnector1 { get { return null; } }
		public virtual string GraphwizConnector2 { get { return null; } }
	}
}