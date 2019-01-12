using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

		List<Node> ReconstructPath(Node current) {
			List<Node> totalPath = new List<Node>();
			totalPath.Add(current);
			while (current.cameFrom != null) {
				totalPath.Add(current.cameFrom);
				current = current.cameFrom;
			}
			totalPath.Reverse();
			return totalPath;
		}

		static int pouet = 0;

		//internal class Comparer : IComparer<Node> {
		//	public int Compare(Node a, Node b) {
		//		return (int)(a.f - b.f);
		//	}
		//}

		public List<Node> Resolve() {
			// algo	
			closedSet = new List<Node>();

			openSet = new List<Node>();

			openSet.Add(new Node(ref input));

			openSet[0].g = 0;
			openSet[0].f = heuristicFunction.heuristic(openSet[0].state);

			while (openSet.Count > 0) {
				openSet = openSet.OrderBy(o => o.f).ToList();
				//for (int i = 0; i < 10 && i < openSet.Count; i++) {
				//	Console.WriteLine(openSet[i].f);
				//}
				//Console.WriteLine("--------");
				Node current = openSet[0];

				if (current.state.Equals(this.solution)) {
					Console.WriteLine("END");
					return ReconstructPath(current);
				}

				openSet.RemoveAt(0);
				closedSet.Add(current);

				List<Node> neighbors = GetNeighbors(ref current);

				foreach (var n in neighbors) {
					//n.state.PrintBoard();

					// If neighbor not in closedSet
					if (closedSet.Find(o => (o.state.Equals(n.state))) != null) {
						//Console.WriteLine("continue");
						continue;
					}

					float tentativeGScore = current.g + 1;
					//Console.WriteLine("tentativeGScore: " + tentativeGScore);
					// If neighbor not in openSet
					if (openSet.Find(o => (o.state.Equals(n.state))) == null) {
						openSet.Add(n);
					} else {
						// Find n in closedSet
						Node n2 = closedSet.Find(o => (o.state.Equals(n.state)));
						if (n2 != null && tentativeGScore >= n2.g) {
							// DOES NOT PASS HERE
							Console.WriteLine("continue");
							continue;
						}	
					}


					n.cameFrom = current;
					n.g = tentativeGScore;
					n.f = n.g + heuristicFunction.heuristic(n.state);

				}

				AStar.pouet++;

				//if (AStar.pouet > 50000) {
				//	Console.WriteLine("End of algo, earlier " + AStar.pouet);
				//	return null;
				//}

				if (AStar.pouet % 500 == 0) {
					Console.WriteLine("pouet: " + AStar.pouet + ". openSet.Count: " + this.openSet.Count + ". closedSet.Count: " + this.closedSet.Count + ". openSet[0].f: " + openSet[0].f);
					current.state.PrintBoard();
				}

			}
			return null;
			//float h = heuristicFunction.heuristic(this.input);
			//Console.WriteLine("H: " + h);
		}

		//float CalculateH(Node node) {
		//	return heuristicFunction.heuristic(Node.board);
		//}	

		List<Node> GetNeighbors(ref Node current) {
			List<Board> boards = Board.GetNeighbors(current.state);
			List<Node> nodes = new List<Node>();

			for (int i = 0; i < boards.Count; i++) {
				Board b = boards[i];
				nodes.Add(new Node(ref b));
			}

			return nodes;
		}

		public void PrintSolution(List<Node> moves) {
			if (moves == null) {
				Console.WriteLine("No Solution Found");
			} else {
				foreach (var move in moves) {
					move.state.PrintBoard();
				}
			}
		}

		public static void Main() {

			List<List<int>> input = new List<List<int>>();

			//input.Add(new List<int>(new int[] { 3, 2 }));
			//input.Add(new List<int>(new int[] { 1, 4 }));

			// Unsolvable. Bug: infinite loop?
			//input.Add(new List<int>(new int[] { 7, 4, 9 }));
			//input.Add(new List<int>(new int[] { 6, 8, 1 }));
			//input.Add(new List<int>(new int[] { 5, 3, 2 }));

			// Solvable
			//input.Add(new List<int>(new int[] { 2, 4, 1 }));
			//input.Add(new List<int>(new int[] { 6, 5, 3 }));
			//input.Add(new List<int>(new int[] { 8, 7, 9 }));

			// Solvable
			//input.Add(new List<int>(new int[] { 10, 14, 15, 13 }));
			//input.Add(new List<int>(new int[] { 5, 7, 3, 9 }));
			//input.Add(new List<int>(new int[] { 11, 1, 6, 4  }));
			//input.Add(new List<int>(new int[] { 12, 2, 16, 8 }));

			// Not Solvable
			input.Add(new List<int>(new int[] { 1, 2, 3 }));
			input.Add(new List<int>(new int[] { 8, 9, 4 }));
			input.Add(new List<int>(new int[] { 7, 6, 5 }));

			Board b2 = Board.GetSnailSolution(input.Count);
			Board b1 = new Board(input);



			//Console.WriteLine("B1:");
			//b1.PrintBoard();
			//Console.WriteLine("B2:");
			//b2.PrintBoard();
			//List<Board> boards = Board.GetNeighbors(b1);
			//foreach (var board in boards) {
			//board.PrintBoard();
			//}

			Validator validator = new Validator(input);
			validator.Validate();


			AStar aStar = new AStar(ref b1, ref b2);

			//List<Node> solution = aStar.Resolve();
			//if (solution != null) {
			//	aStar.PrintSolution(solution);
			//}


			//Console.WriteLine("OpenSet.Count: " + aStar.openSet.Count);
			//Console.WriteLine("ClosedSet.Count: " + aStar.closedSet.Count);
			//if (solution != null) {
			//	Console.WriteLine("Nunber of Moves to solution: " + solution.Count);
			//}
		}

	}
}
