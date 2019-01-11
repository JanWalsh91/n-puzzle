using System;
using System.Collections.Generic;

namespace server.src {
	public class AStar {

		Board input;
		Board solution;
		HeuristicFunction heuristicFunction;
		HashSet<Node> openSet = new HashSet<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();

		public AStar(ref Board input, ref Board solution) {
			this.input = input;
			this.solution = solution;
			heuristicFunction = new HeuristicFunction(ref solution);
		}

		public void SetHeuristicFunction(HeuristicFunction.Types type) {
			heuristicFunction.SetHeuristic(type);
		}

		public void Resolve() {
			// algo	

			float h = heuristicFunction.heuristic(this.input);
			Console.WriteLine("H: " + h);
		}

		//float CalculateH(Node node) {
		//	return heuristicFunction.heuristic(Node.board);
		//}


		public static void Main() {

			List<List<int>> input = new List<List<int>>();

			input.Add(new List<int>(new int[] { 1, 2 }));
			input.Add(new List<int>(new int[] { 3, 4 }));

			List<List<int>> solution = new List<List<int>>();

			solution.Add(new List<int>(new int[] { 3, 4 }));
			solution.Add(new List<int>(new int[] { 1, 2 }));


			Board b1 = new Board(input);
			Board b2 = new Board(solution);

			AStar aStar = new AStar(ref b1, ref b2);

			aStar.Resolve();
		}

	}
}
