﻿using System;
using System.Collections.Generic;

namespace server.src {

	public class NodeWrapper {
		public Node node { get; set; }
	}

	public class Node : IComparable<Node> {
		public string hash;
		public Node cameFrom;
		public Board state;
		public float g;
		public float f;

		public Node(ref Board board) {
			this.state = board;
			this.cameFrom = null;
			g = float.PositiveInfinity;
			f = float.PositiveInfinity;
			this.hash = state.ToString();
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
			this.hash = hash;
		}

		public int CompareTo(Node other) {
			if (this.hash.Equals(other.hash)) {
				return 0;
			}
			return ((this.f - other.f > 0) ? 1 : -1);
		}

		public List<string> GetNeighborHashes() {
			return this.state.GetNeighborHashes();
		}

	}
}
