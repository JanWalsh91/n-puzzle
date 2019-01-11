using System;
namespace server.src {
	public class Node {
		Node previous;
		Board state;
		float g;
		float f;

		public Node(ref Board board) {
			this.state = board;	
		}
	}
}
