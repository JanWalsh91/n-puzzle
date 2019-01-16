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
		Dictionary<string, bool> inOpenSet = new Dictionary<string, bool>();
		Dictionary<string, bool> inClosedSet = new Dictionary<string, bool>();

		Hashtable closedSet;
		OrderedSet<Node> openSet;

		internal class Comparer : Comparer<Node> {
			override public int Compare(Node x, Node y) {
				//return (x.f - y.f > 0 ? 1 : -1);
				if (x.f > 5000 || y.f > 5000) {
					Console.WriteLine("!!!x: " + x.f + " y: " + y.f);

				}
				if (x.hash.Equals(y.hash)) {
					Console.WriteLine("Equals hash");
					return 0;
				}
				return ((x.f - y.f > 0) ? 1 : -1);
			}
		}

		//internal class EqualityComparer : IEqualityComparer<Node> {
		//	public bool Equals(Node x, Node y) {
		//		return x.hash.Equals(y.hash);
		//	}

		//	public int GetHashCode(Node obj) {
		//		return obj.GetHashCode();
		//	}
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

			//openSet = new OrderedSet<Node>(new Comparer());
			openSet = new OrderedSet<Node>();

			//EqualityComparer equalityComparer = new EqualityComparer();

			Node n = new Node(ref input);
			nodes.Add(n.hash, n);
			inOpenSet.Add(n.hash, true);
			openSet.Add(n);

			n.g = 0;
			n.f = heuristicFunction.heuristic(n.state);

			bool evalNeighbor = false;
			Node current = null;

			while (openSet.Count > 0) {
				//Console.WriteLine("WHILE START");
				if (!evalNeighbor) {
					current = openSet.RemoveFirst();
					//current.isInOpenSet = false;
				} else {
					evalNeighbor = false;
				}
				inOpenSet[current.hash] = false;

				//Console.WriteLine("set current to : " + current.hash);

				if (current.state.Equals(this.solution)) {
					//Console.WriteLine("END");
					return ReconstructPath(current);
				}

				//Console.WriteLine("ADDED TO CLOSED SET " + current.hash);
				inClosedSet[current.hash] = true;
				closedSet[current.hash] = current;
				
				//closedSet[current.hash] = current;
				//Console.WriteLine("CURRENT " + current.hash);

				List<Node> neighbors = GetNeighbors(ref current);
				for (int i = 0; i < neighbors.Count; i++) {
					//Console.WriteLine("FOR START");

					// If neighbor in closedSet
					bool val;
					if (inClosedSet.TryGetValue(neighbors[i].hash, out val) && val) {
						//Console.WriteLine("SKIP " + neighbors[i].hash);
						continue;
					}
					//Console.WriteLine("CONTINUE " + neighbors[i].hash);

					float tentativeGScore = current.g + 1;

					if (inOpenSet.TryGetValue(neighbors[i].hash, out val) && val) {
						if (tentativeGScore >= neighbors[i].g) {
							// DOES NOT PASS HERE
							//Console.WriteLine("continue");
							continue;
						}
					}

					neighbors[i].cameFrom = current;
					neighbors[i].g = tentativeGScore;
					neighbors[i].f = neighbors[i].g + heuristicFunction.heuristic(neighbors[i].state);

					if (inOpenSet.TryGetValue(neighbors[i].hash, out val) || !val) {
						inOpenSet[neighbors[i].hash] = true;
						//Console.WriteLine("ADDED TO OPENSET: " + neighbors[i].hash);
						openSet.Add(neighbors[i]);
					} else {
						//Console.WriteLine("READDED TO OPENSET: " + neighbors[i].hash);
						////if (neighbors[i].hash.Equals("3;2;7;4;11;8;12;13;1;9;14;5;15;0;6;10")) {
						////	int I = openSet.FindAll(a => a.hash.Equals(neighbors[i].hash)).ToList().Count;
						////	Console.WriteLine("I: " + I);
						////}
						//while (openSet.Contains(neighbors[i])) {
							
						//	openSet.Remove(neighbors[i]);
						//}
						////openSet.
						////if (neighbors[i].hash.Equals("3;2;7;4;11;8;12;13;1;9;14;5;15;0;6;10")) {
						////	int I = openSet.FindAll(a => a.hash.Equals(neighbors[i].hash)).ToList().Count;
						////	Console.WriteLine("I: " + I);
						////}
						////openSet[neighbors[i].hash]
						//openSet.Add(neighbors[i]);
						////if (neighbors[i].hash.Equals("3;2;7;4;11;8;12;13;1;9;14;5;15;0;6;10")) {
						////	int I = openSet.FindAll(a => a.hash.Equals(neighbors[i].hash)).ToList().Count;
						////	Console.WriteLine("I: " + I);
						////}
					}

				}

				//if (neighbors.Count > 0) {
				//	var bestNeighborList = neighbors.Where(nn => Math.Abs(nn.f - neighbors.Min(o => o.f)) < 0.0001).ToList();
				//	if (bestNeighborList.Count > 0) {
				//		var bestNeighbor = bestNeighborList[0];
				//		if (bestNeighbor.f < current.f) {
				//			//Console.WriteLine("Going to Neighbor Express town ");
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
					//	Console.WriteLine(item.f);
					//	//Console.WriteLine(item.ToString());
					//}

					//openSet = openSet.Reverse();

					//if (AStar.pouet > 50000) {
					//	while (openSet.Count > 0) {
					//		Node tmp = openSet.RemoveFirst();
					//		//Console.WriteLine(tmp.GetHashCode());
					//		Console.WriteLine(tmp.f);
					//		//TypedReference tr = __makeref(tmp);
					//		//Console.WriteLine(tr.);
					//	}

					//	return null;
					//}

				}
			}
			return null;
		}

		public List<Node> GetNeighbors(ref Node current) {
			List<string> hashes = current.GetNeighborHashes();
			//Console.WriteLine("GET NEIGHBORS. found " + hashes.Count);

			List<Node> neighbors = new List<Node>();
			foreach (var hash in hashes) {
				//Console.WriteLine("hash: " + hash);
				if (nodes.ContainsKey(hash)) {
					//Console.WriteLine("Contains " + hash);
					neighbors.Add(nodes[hash]);	
				} else {
					//Console.WriteLine("NOT Contains " + hash);
					Node node = new Node(hash);
					nodes.Add(node.hash, node);
					neighbors.Add(node);
				}
			}

			//Console.WriteLine("Current: ");
			//current.state.PrintBoard();
			//foreach (var node in neighbors) {
			//	//Console.WriteLine("Neighbor: ");
			//	node.state.PrintBoard();
			//	Console.WriteLine(node.hash);
			//}
			//Console.WriteLine("END GET NEIGHBORS. return " + neighbors.Count);

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

		public List<string> GetStringSolution(List<Node> moves) {
			List<string> pathToSolution = new List<string>();
			int emptyCellIndex;
			emptyCellIndex = moves[0].state.GetList().IndexOf(0);
			moves.RemoveAt(0);

			foreach (var move in moves) {
				List<int> currentList = move.state.GetList();
				int newEmptyCellIndex = currentList.IndexOf(0);
				if (newEmptyCellIndex / move.state.GetSize() == emptyCellIndex / move.state.GetSize()) {
					pathToSolution.Add(newEmptyCellIndex < emptyCellIndex ? "Right" : "Left");

				} else {
					pathToSolution.Add(newEmptyCellIndex < emptyCellIndex ? "Down" : "Up");
				}
				emptyCellIndex = newEmptyCellIndex;
			}
			return pathToSolution;
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
		//public static void Main(string[] argv) {


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

		//List<List<int>> input = new List<List<int>>();

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
		//input.Add(new List<int>(new int[] { 3, 7, 4, 13 }));
		//input.Add(new List<int>(new int[] { 0, 8, 2, 12 }));
		//input.Add(new List<int>(new int[] { 11, 1, 9, 5 }));
		//input.Add(new List<int>(new int[] { 15, 6, 14, 10 }));

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

		// SooOO00lvable
		//input.Add(new List<int>(new int[] { 11, 22, 1, 5, 14 }));
		//input.Add(new List<int>(new int[] { 23, 4, 9, 17, 24  }));
		//input.Add(new List<int>(new int[] { 0, 21, 16, 7, 15 }));
		//input.Add(new List<int>(new int[] { 18, 2, 19, 3, 12 }));
		//input.Add(new List<int>(new int[] { 8, 20, 13, 6, 10 }));

		// Solvable
		//input.Add(new List<int>(new int[] { 16, 19, 3, 17, 31, 22 }));
		//input.Add(new List<int>(new int[] { 20, 1, 18, 26, 15, 11 }));
		//input.Add(new List<int>(new int[] { 4, 8, 28, 6, 34, 12 }));
		//input.Add(new List<int>(new int[] { 0, 32, 13, 29, 21, 30 }));
		//input.Add(new List<int>(new int[] { 27, 9, 7, 25, 10, 35 }));
		//input.Add(new List<int>(new int[] { 2, 33, 14, 5, 24, 23 }));

		//Board b2 = Board.GetSnailSolution(input.Count);
		//Board b1 = new Board(input);


		//Console.WriteLine("B1:");
		//b1.PrintBoard();
		//Console.WriteLine("B2:");
		//b2.PrintBoard();
		//List<Board> boards = Board.GetNeighbors(b1);
		//foreach (var board in boards) {
		//board.PrintBoard();
		//}

		//Validator validator = new Validator(input);
		//try {
		//validator.Validate();
		//} catch (Exception e) {
		//	Console.WriteLine(e.Message);
		//	return;
		//}
		//return;

		//AStar aStar = null;
		//try {
		//	aStar = new AStar(ref b1, ref b2);
		//} catch (OutOfMemoryException oome) {
		//	Console.WriteLine(":( " + oome.Message);
		//	return;
		//}

		//List<Node> solution = aStar.Resolve();
		//if (solution != null) {
		//	aStar.PrintSolution(solution);
		//}


		//Console.WriteLine("OpenSet.Count: " + aStar.openSet.Count);
		//Console.WriteLine("ClosedSet.Count: " + aStar.closedSet.Count);
		//if (solution != null) {
		//	Console.WriteLine("Nunber of Moves to solution: " + solution.Count);
		//}
	//}

	}
}

