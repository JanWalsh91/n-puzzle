﻿using System;
namespace server.src {
	public class Node {
		Node cameFrom;
		public Board state;
		public float g;
		public float f;

		public Node(ref Board board) {
			this.state = board;
			this.cameFrom = null;
			g = float.PositiveInfinity;
			f = float.PositiveInfinity;
		}
	}
}
