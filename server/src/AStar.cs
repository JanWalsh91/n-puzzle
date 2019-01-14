using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

namespace server.src {
	public class AStar {

		static int pouet = 0;

		Board input;
		Board solution;

		HeuristicFunction heuristicFunction;

		Dictionary<string, Node> nodes = new Dictionary<string, Node>();
		Hashtable closedSet;
		OrderedBag<Node> openSet;

		internal class Comparer : IComparer<Node> {
			public int Compare(Node x, Node y) {
				//return (x.f - y.f > 0 ? 1 : -1);
				return (int)(x.f - y.f);
			}
		}

		internal class EqualityComparer : IEqualityComparer<Node> {
			public bool Equals(Node x, Node y) {
				return x.hash.Equals(y.hash);
			}

			public int GetHashCode(Node obj) {
				return obj.GetHashCode();
			}
		}


		public AStar(ref Board input, ref Board solution) {
			this.input = input;
			this.solution = solution;
			heuristicFunction = new HeuristicFunction(ref solution);
		}
		
		public void SetHeuristicFunction(HeuristicFunction.Types type) {
			heuristicFunction.SetHeuristic(type);
		}

		List<Node> ReconstructPath(Node current) {
			List<Node> totalPath = new List<Node> {
				current
			};
			while (current.cameFrom != null) {
				totalPath.Add(current.cameFrom);
				current = current.cameFrom;
			}
			totalPath.Reverse();
			return totalPath;
		}

		public List<Node> Resolve() {
			
			closedSet = new Hashtable();

			openSet = new OrderedBag<Node>(new Comparer());

			EqualityComparer equalityComparer = new EqualityComparer();

			Node n = new Node(ref input);
			nodes.Add(n.hash, n);
			openSet.Add(n);
			n.isInOpenSet = true;

			n.g = 0;
			n.f = heuristicFunction.heuristic(n.state);

			bool evalNeighbor = false;
			Node current = null;

			while (openSet.Count > 0) {
				
				if (!evalNeighbor) {
					//current = GetSmallestValue();
					current = openSet.RemoveFirst();
					current.isInOpenSet = false;
				} else {
					evalNeighbor = false;
				}

				//Console.WriteLine("set current to : " + current.hash);

				if (current.state.Equals(this.solution)) {
					Console.WriteLine("END");
					return ReconstructPath(current);
				}

				//closedSet.Add(current.hash, current);
				closedSet[current.hash] = current;
				current.isInClosedSet = true;

				List<Node> neighbors = GetNeighbors(ref current);
				foreach (var neighbor in neighbors) {
					
					// If neighbor in closedSet
					if (closedSet.ContainsKey(neighbor.hash)) {
						continue;
					}

					float tentativeGScore = current.g + 1;
	
					if (neighbor.isInOpenSet) {
						if (tentativeGScore >= neighbor.g) {
							// DOES NOT PASS HERE
							//Console.WriteLine("continue");
							continue;
						}
					}

					neighbor.cameFrom = current;
					neighbor.g = tentativeGScore;
					neighbor.f = neighbor.g + heuristicFunction.heuristic(neighbor.state);

					if (!neighbor.isInOpenSet) {
						openSet.Add(neighbor);
						neighbor.isInOpenSet = true;
					} else {
						openSet.Remove(neighbor);
						openSet.Add(neighbor);
					}

				}

				//if (neighbors.Count > 0) {
				//	var bestNeighborList = neighbors.Where(nn => Math.Abs(nn.f - neighbors.Min(o => o.f)) < 0.0001).ToList();
				//	if (bestNeighborList.Count > 0) {
				//		var bestNeighbor = bestNeighborList[0];
				//		if (bestNeighbor.f < current.f) {
				//			//Console.WriteLine("Going to Neighbot EXpress town");
				//			current = bestNeighbor;
				//			evalNeighbor = true;
				//		}
				//	}
				//}


				AStar.pouet++;

				if (AStar.pouet % 500 == 0) {
					Console.WriteLine("pouet: " + AStar.pouet + ". openSet.Count: " + this.openSet.Count + ". closedSet.Count: " + this.closedSet.Count + ". f: " + current.f);
					current.state.PrintBoard();
					Console.WriteLine("==========");
					//foreach (var item in openSet) {
					//	//Console.WriteLine(item.f);
					//	Console.WriteLine(item.ToString());
					//}

					//openSet = openSet.Reverse();

					//while (openSet.Count > 0) {
					//	Node tmp = openSet.RemoveFirst();
					//	//Console.WriteLine(tmp.GetHashCode());
					//	Console.WriteLine(tmp.f);
					//	//TypedReference tr = __makeref(tmp);
					//	//Console.WriteLine(tr.);
					//}
					//Console.WriteLine("========== END");

					//return null;
				}
			}
			return null;
		}

		public List<Node> GetNeighbors(ref Node current) {
			List<string> hashes = current.GetNeighborHashes();
			List<Node> neighbors = new List<Node>();
			foreach (var hash in hashes) {
				if (nodes.ContainsKey(hash)) {
					neighbors.Add(nodes[hash]);	
				} else {
					Node node = new Node(hash);
					nodes.Add(node.hash, node);
					neighbors.Add(node);
				}
			}
			return neighbors;
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

		// TEST ==========

		internal class Comparer2 : Comparer<IntWrapper> {
			override public int Compare(IntWrapper x, IntWrapper y) {
				Console.WriteLine("Compare " + x.v + " and " + y.v + ": " + (x.v - y.v > 0 ? 1 : -1));
				return (int)(x.v - y.v);
			}
		}

		public class IntWrapper {
			public float v;

			public IntWrapper(float i) {
				this.v = i;
			}
		}


		public static void PrintBag(OrderedBag<IntWrapper> b) {
			foreach (var item in b) {
				Console.WriteLine(item.v);
			}
		}

		// TEST END ==========
		public static void Main(string[] argv) {


			//OrderedBag<IntWrapper> bag = new OrderedBag<IntWrapper>(new Comparer2());
			//IntWrapper intWrapper = new IntWrapper(1.0f);
			//bag.Add(intWrapper);
			//PrintBag(bag);
			//bag.Add(new IntWrapper(-1.0f));
			//PrintBag(bag);
			//bag.Add(new IntWrapper(2.0f));
			//bag.Add(new IntWrapper(242.0f));
			//bag.Add(new IntWrapper(242.5f));
			//intWrapper.v = 999f;
			//bag.Add(new IntWrapper(-1000f));
			//PrintBag(bag);
			//bag.
			//return;

			//Parser parser = new Parser();

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

			//Solvable
			//input.Add(new List<int>(new int[] { 1, 2, 3 }));
			//input.Add(new List<int>(new int[] { 8, 0, 4 }));
			//input.Add(new List<int>(new int[] { 7, 6, 5 }));

			// Solvable BUG not solvable?
			//input.Add(new List<int>(new int[] { 1, 2, 3, 4 }));
			//input.Add(new List<int>(new int[] { 12, 13, 5, 6 }));
			//input.Add(new List<int>(new int[] { 11, 15, 14, 0 }));
			//input.Add(new List<int>(new int[] { 10, 9, 8, 7 }));

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
