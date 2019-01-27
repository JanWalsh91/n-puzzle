using System;
using System.Collections.Generic;

namespace server.src {
	public class IDA {

		Board input;
		Board solution;

		HeuristicFunction heuristicFunction;

		Stack<Node> path;

		public IDA(ref Board input, ref Board solution) {
			this.input = input;
			this.solution = solution;
			heuristicFunction = new HeuristicFunction(ref solution);
		}

		public void SetHeuristicFunction(HeuristicFunction.Types type) {
			heuristicFunction.SetHeuristic(type);
		}

		public List<Node> Resolve() {
			Console.WriteLine("IDA Resolve");
			Node root = new Node(ref this.input);
			root.g = 0;
			root.f = heuristicFunction.heuristic(root.state);

			float bound = heuristicFunction.heuristic(root.state);
			this.path = new Stack<Node>(new Node[] { root });

			while(true) {
				float t = this.Search(0, bound);
				if (Math.Abs(t) < 0.001f) {
					Console.WriteLine("FOUND RES");
					List<Node> finalPath = new List<Node>(this.path);
					finalPath.Reverse();
					return finalPath; // FOUND
				} else if (float.IsPositiveInfinity(t)) {
					Console.WriteLine("NOT FOUND RES");
					return new List<Node>(); // NOT FOUND
				}
				bound = t;
			}
		}

		float Search(float g, float bound) {
			Console.WriteLine("IDA Search.");
			Console.WriteLine("\tBound: " + bound);
			Node node = path.Peek();
			Console.WriteLine("\tf: " + node.f);
			float f = node.f;

			if (f > bound) {
				Console.WriteLine("Return f: " + f);
				return f;
			}
			if (this.solution.Equals(node.state)) {
				Console.WriteLine("FOUND end state");
				return 0; // FOUND
			}
			float min = float.PositiveInfinity;
			List<Node> neighbors = GetNeighbors(ref node);

			Console.WriteLine("Neighbor Loop: " + neighbors.Count + " neighbors");
			for (int i = 0; i < neighbors.Count; i++) {
				if (!path.Contains(neighbors[i])) {
					Console.WriteLine("\tNeihgbor: " + i);
					neighbors[i].g = node.g + 1;
					neighbors[i].f = neighbors[i].g + heuristicFunction.heuristic(neighbors[i].state);
					path.Push(neighbors[i]);
					float t = this.Search(0, bound);
					if (Math.Abs(t) < 0.001f) {
						Console.WriteLine("\tFOUND");
						return 0; // FOUND
					}
					if (t < min) {
						Console.WriteLine("\tUpdate min to t: " + t);
						min = t;
					}
					path.Pop();
				}
			}
			return min;
		}

		public List<Node> GetNeighbors(ref Node current) {
			Console.WriteLine("GetNeighbors");
			List<string> hashes = current.GetNeighborHashes();
			//Console.WriteLine("GET NEIGHBORS. found " + hashes.Count);

			List<Node> neighbors = new List<Node>();
			List<Node> tmpPath = new List<Node>(path);
			foreach (var hash in hashes) {
				//Console.WriteLine("hash: " + hash);
				if (tmpPath.Exists(x => x.hash == hash)) {
					//Console.WriteLine("Contains " + hash);
					neighbors.Add(tmpPath.Find(x => x.hash == hash));
				} else {
					//Console.WriteLine("NOT Contains " + hash);
					Node node = new Node(hash);
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
	}
}
