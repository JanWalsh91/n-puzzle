using System;
using System.Collections.Generic;

namespace server.src {
	public class AStar {

		Board input;
		Board solution;
		HeuristicFunction heuristicFunction;
		List<Node> openSet;
		List<Node> closedSet;

		public AStar(ref Board input, ref Board solution) {
			this.input = input;
			this.solution = solution;
			heuristicFunction = new HeuristicFunction(ref solution);
		}
		
		public void SetHeuristicFunction(HeuristicFunction.Types type) {
			heuristicFunction.SetHeuristic(type);
		}

		//void ReconstructPath(Node cameFrom, Node current) {
		//	List<Node> totalPath = new List<Node>();

		//}

		public void Resolve() {
			// algo	
			closedSet = new List<Node>();

			openSet = new List<Node>();
			openSet.Add(new Node(ref input));

			openSet[0].g = 0;
			openSet[0].f = heuristicFunction.heuristic(openSet[0].state);

			while (openSet.Count > 0) {
				//Node current = openSet.
			}

			//float h = heuristicFunction.heuristic(this.input);
			//Console.WriteLine("H: " + h);
		}

		//float CalculateH(Node node) {
		//	return heuristicFunction.heuristic(Node.board);
		//}


		public static void Main() {

			List<List<int>> input = new List<List<int>>();

			input.Add(new List<int>(new int[] { 1, 2, 3, 4 }));
			input.Add(new List<int>(new int[] { 5, 6, 7, 8 }));
			input.Add(new List<int>(new int[] { 9, 10, 11, 12 }));
			input.Add(new List<int>(new int[] { 13, 14, 15, 16 }));


			Board b2 = Board.GetSnailSolution(input.Count);
			Board b1 = new Board(input);

			b1.PrintBoard();
			b2.PrintBoard();

			AStar aStar = new AStar(ref b1, ref b2);

			aStar.Resolve();
		}

	}
}
