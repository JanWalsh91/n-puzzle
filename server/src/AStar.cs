using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace server.src {
	public class AStar {

		Board input;
		Board solution;
		HeuristicFunction heuristicFunction;
		//List<Node> openSet;
		//List<Node> closedSet;

		//SortedList<String, Node> o2 = new SortedList<string, Node>(new Comparer());

		Hashtable openSet;
		Hashtable closedSet;

		//internal class Comparer : IComparer<Node> {
			//public int Compare(Node x, Node y) {
			//	return (int)(x.f - y.f);
			//}
		//}

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
			closedSet = new Hashtable();

			openSet = new Hashtable();

			Node n = new Node(ref input);
			openSet.Add(n.hash, n);

			

			n.g = 0;
			n.f = heuristicFunction.heuristic(n.state);

			while (openSet.Count > 0) {

				Node current = GetSmallestValue();
				//Console.WriteLine("set current to : " + current.hash);

				if (current.state.Equals(this.solution)) {
					Console.WriteLine("END");
					return ReconstructPath(current);
				}
				openSet.Remove(current.hash);

				closedSet.Add(current.hash, current);

				List<Node> neighbors = GetNeighbors(ref current);

				foreach (var neighbor in neighbors) {

					// If neighbor in closedSet
					//if (closedSet.Find(o => (o.state.Equals(neighbor.state))) != null) {
					if (closedSet[neighbor.hash] != null) {
						//Console.WriteLine("continue");
						continue;
					}

					float tentativeGScore = current.g + 1;
					//Console.WriteLine("tentativeGScore: " + tentativeGScore);
					// If neighbor not in openSet
					if (openSet[neighbor.hash] == null) {
						openSet.Add(neighbor.hash, neighbor);
					} else {
						// Find n in closedSet
						Node n2 = closedSet[neighbor.hash] as Node;
						if (n2 != null && tentativeGScore >= n2.g) {
							// DOES NOT PASS HERE
							Console.WriteLine("continue");
							continue;
						}	
					}


					neighbor.cameFrom = current;
					neighbor.g = tentativeGScore;
					neighbor.f = neighbor.g + heuristicFunction.heuristic(neighbor.state);

				}

				AStar.pouet++;

				//if (AStar.pouet > 50000) {
				//	Console.WriteLine("End of algo, earlier " + AStar.pouet);
				//	return null;
				//}

				if (AStar.pouet % 500 == 0) {
					Console.WriteLine("pouet: " + AStar.pouet + ". openSet.Count: " + this.openSet.Count + ". closedSet.Count: " + this.closedSet.Count);
					current.state.PrintBoard();
				}

			}
			return null;
			//float h = heuristicFunction.heuristic(this.input);
			//Console.WriteLine("H: " + h);
		}

		Node GetSmallestValue() {
			string key = "";
			float fValue = float.PositiveInfinity;

			foreach (DictionaryEntry item in openSet) {
				if ((item.Value as Node).f < fValue) {
					key = item.Key as string;
					fValue = (item.Value as Node).f;
				}
			}

			return openSet[key] as Node;
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

		public static void Main(string[] argv) {

			Parser parser = new Parser();

			//foreach (var str in argv) {
			//List<List<int>> input = parser.SolveFromFile(argv[0]);

			//Board board = new Board(input);
			//board.PrintBoard();

			//}
			//return;

			List<List<int>> input = new List<List<int>>();

			//input.Add(new List<int>(new int[] { 3, 2 }));
			//input.Add(new List<int>(new int[] { 1, 0 }));

			// Unsolvable
			//input.Add(new List<int>(new int[] { 7, 4, 0 }));
			//input.Add(new List<int>(new int[] { 6, 8, 1 }));
			//input.Add(new List<int>(new int[] { 5, 3, 2 }));

			// Solvable
			//input.Add(new List<int>(new int[] { 2, 4, 1 }));
			//input.Add(new List<int>(new int[] { 6, 5, 3 }));
			//input.Add(new List<int>(new int[] { 8, 7, 0 }));

			// Solvable
			//input.Add(new List<int>(new int[] { 1, 8, 7 }));
			//input.Add(new List<int>(new int[] { 5, 0, 2 }));
			//input.Add(new List<int>(new int[] { 6, 4, 3 }));

			// Solvable
			//input.Add(new List<int>(new int[] { 7, 1, 0 }));
			//input.Add(new List<int>(new int[] { 6, 8, 5 }));
			//input.Add(new List<int>(new int[] { 2, 3, 4 }));

			// Solvable
			//input.Add(new List<int>(new int[] { 1, 8, 5 }));
			//input.Add(new List<int>(new int[] { 4, 0, 2 }));
			//input.Add(new List<int>(new int[] { 3, 6, 7 }));

			// Solvable
			input.Add(new List<int>(new int[] { 3, 7, 4, 13 }));
			input.Add(new List<int>(new int[] { 0, 8, 2, 12 }));
			input.Add(new List<int>(new int[] { 11, 1, 9, 5 }));
			input.Add(new List<int>(new int[] { 15, 6, 14, 10 }));

			// Unsolvable
			//input.Add(new List<int>(new int[] { 5, 3, 2 }));
			//input.Add(new List<int>(new int[] { 6, 7, 1 }));
			//input.Add(new List<int>(new int[] { 8, 4, 0 }));

			// Unsolvable
			//input.Add(new List<int>(new int[] { 5, 15, 10, 11 }));
			//input.Add(new List<int>(new int[] { 9, 1, 0, 12 }));
			//input.Add(new List<int>(new int[] { 2, 6, 7, 13 }));
			//input.Add(new List<int>(new int[] { 8, 3, 4, 14 }));

			// Unsolvable
			//input.Add(new List<int>(new int[] { 14, 10, 6, 15, 8 }));
			//input.Add(new List<int>(new int[] { 12, 20, 19, 23, 21 }));
			//input.Add(new List<int>(new int[] { 9, 5, 17, 7, 24 }));
			//input.Add(new List<int>(new int[] { 1, 0, 13, 4, 3 }));
			//input.Add(new List<int>(new int[] { 11, 18, 2, 16, 22 }));


			// Not Solvable
			//input.Add(new List<int>(new int[] { 2, 1, 3 }));
			//input.Add(new List<int>(new int[] { 8, 0, 4 }));
			//input.Add(new List<int>(new int[] { 7, 6, 5 }));

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
			//try {
				validator.Validate();
			//} catch (Exception e) {
			//	Console.WriteLine(e.Message);
			//	return;
			//}
			//return;

			AStar aStar = new AStar(ref b1, ref b2);

			List<Node> solution = aStar.Resolve();
			if (solution != null) {
				aStar.PrintSolution(solution);
			}


			//Console.WriteLine("OpenSet.Count: " + aStar.openSet.Count);
			//Console.WriteLine("ClosedSet.Count: " + aStar.closedSet.Count);
			//if (solution != null) {
			//	Console.WriteLine("Nunber of Moves to solution: " + solution.Count);
			//}
		}

	}
}
