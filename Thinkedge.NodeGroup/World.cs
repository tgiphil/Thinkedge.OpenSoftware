using System.Collections.Generic;
using Thinkedge.Common;

namespace Thinkedge.NodeGroup
{
	public class World
	{
		public List<Node> Nodes = new List<Node>();
		public HashSet<Group> Groups = new HashSet<Group>();

		private KeyedList<string, Node> lookupByType = new KeyedList<string, Node>();
		private KeyedList<string, Node> lookupByKey = new KeyedList<string, Node>();

		private int groupID = 0;

		public bool Add(Node node)
		{
			Nodes.Add(node);

			var group = new Group(++groupID);
			group.Add(node);
			Groups.Add(group);
			node.Group = group;

			lookupByType.Add(node.Type, node);

			if (node.ID != null)
			{
				lookupByKey.Add(node.ID, node);
			}

			return true;
		}

		public bool AddIfNew(Node node)
		{
			if (node.ID != null)
			{
				if (GetFirstByKey(node.ID, node.Type) != null)
					return false;
			}

			return Add(node);
		}

		public void Related(Node node1, Node node2)
		{
			var group1 = node1.Group;
			var group2 = node2.Group;

			if (group1 == group2)
				return;

			// move all node from old group to new group
			foreach (var node in group2.Nodes)
			{
				node.Group = group1;
				group1.Add(node);
			}

			// remove old group
			Groups.Remove(group2);
		}

		public List<Node> GetNodesOfType(string type)
		{
			return lookupByType.Get(type);
		}

		public List<Node> GetNodesOfValueType<T>()
		{
			return lookupByType.Get(typeof(T).Name);
		}

		public Node GetFirstByKey(string key, string type)
		{
			var nodes = lookupByKey.Get(key);

			if (nodes == null)
				return null;

			foreach (var node in nodes)
			{
				if (node.Type == type)
					return node;
			}

			return null;
		}

		public Node GetFirstByKey<T>(string key)
		{
			var nodes = lookupByKey.Get(key);
			string type = typeof(T).Name;

			if (nodes == null)
				return null;

			foreach (var node in nodes)
			{
				if (node.Type == type)
					return node;
			}

			return null;
		}
	}
}