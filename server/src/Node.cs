using System;
using System.Collections.Generic;

namespace server.src {
	public class Node /* IComparable<Node> */ {
		public string hash;
		public Node cameFrom;
		public Board state;
		public float g;
		public float f;
		public bool isInOpenSet; 
		public bool isInClosedSet; 

		public Node(ref Board board) {
			this.state = board;
			this.cameFrom = null;
			g = float.PositiveInfinity;
			f = float.PositiveInfinity;
			this.hash = state.ToString();
			isInOpenSet = false;
			isInClosedSet = false;
		}

		public Node(string hash) {
			string[] t = hash.Split(';');
			List<int> t2 = new List<int>(t.Length);
			for (int i = 0; i < t.Length; i++) {
				t2.Add(Int32.Parse(t[i]));
			}
			this.state = new Board(t2);
			this.cameFrom = null;
			g = float.PositiveInfinity;
			f = float.PositiveInfinity;
			isInOpenSet = false;
			isInClosedSet = false;
			this.hash = hash;
		}

		//public int CompareTo(Node other) {
		//	return (int)(other.f - this.f);
		//}

		//public override bool Equals(object obj) {
		//	if (obj.GetType() != typeof(Node)) {
		//		return false;
		//	}
		//	Console.WriteLine("IRE");
		//	return this.hash.Equals((obj as Node).hash);
		//}

		//public override int GetHashCode() {
		//	return base.GetHashCode();
		//}

		public List<string> GetNeighborHashes() {
			return this.state.GetNeighborHashes();
		}

	}
}
