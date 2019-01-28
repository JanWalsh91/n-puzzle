using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;
using System.Threading.Tasks;
using System.Threading;

namespace server.src {
	public class AStar : Algorithm {

		static int pouet = 0;

		Dictionary<string, Node> nodes = new Dictionary<string, Node>();
		Dictionary<string, bool> inOpenSet = new Dictionary<string, bool>();
		Dictionary<string, bool> inClosedSet = new Dictionary<string, bool>();

		Hashtable closedSet;
		OrderedSet<Node> openSet;

		internal class Comparer : Comparer<Node> {
			override public int Compare(Node x, Node y) {
				if (x.hash.Equals(y.hash)) {
					return 0;
				}
				return ((x.f - y.f > 0) ? 1 : -1);
			}
		}

		public AStar(ref Board input, ref Board solution) {
			this.input = input;
			this.solution = solution;
			heuristicFunction = new HeuristicFunction(ref solution);
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

		public override List<Node> Resolve(CancellationToken ct) {

			closedSet = new Hashtable();

			openSet = new OrderedSet<Node>();

			Node n = new Node(ref input);
			nodes.Add(n.hash, n);
			inOpenSet.Add(n.hash, true);
			openSet.Add(n);

			UpdateTimeComplexity();

			n.g = 0;
			n.f = heuristicFunction.heuristic(n.state);

			bool evalNeighbor = false;
			Node current = null;
			float maxF = float.MinValue;
			Node bestNeighbor = null;

			while (openSet.Count > 0) {
				if (ct.IsCancellationRequested) {
					ct.ThrowIfCancellationRequested();
				}

				if (!evalNeighbor) {
					current = openSet.RemoveFirst();
					UpdateTimeComplexity();
				} else {
					evalNeighbor = false;
				}
				inOpenSet[current.hash] = false;

				if (current.state.Equals(this.solution)) {
					return ReconstructPath(current);
				}

				inClosedSet[current.hash] = true;
				closedSet[current.hash] = current;

				UpdateSizeComplexity();

				if (this.greedySearch) {
					maxF = float.MinValue;
					bestNeighbor = null;
				}

				List<Node> neighbors = GetNeighbors(ref current);
				for (int i = 0; i < neighbors.Count; i++) {

					bool val;
					if (inClosedSet.TryGetValue(neighbors[i].hash, out val) && val) {
						continue;
					}

					float tentativeGScore = current.g + 1;

					if (inOpenSet.TryGetValue(neighbors[i].hash, out val) && val) {
						if (tentativeGScore >= neighbors[i].g) {
							continue;
						}
					}

					neighbors[i].cameFrom = current;
					neighbors[i].g = tentativeGScore;
					neighbors[i].f = neighbors[i].g + heuristicFunction.heuristic(neighbors[i].state);

					if (inOpenSet.TryGetValue(neighbors[i].hash, out val) || !val) {
						inOpenSet[neighbors[i].hash] = true;
						openSet.Add(neighbors[i]);
						UpdateTimeComplexity();
					}

					if (this.greedySearch && neighbors[i].f > maxF) {
						maxF = neighbors[i].f;
						bestNeighbor = neighbors[i];
					}

				}

				if (this.greedySearch && bestNeighbor != null) {
					evalNeighbor = true;
					current = bestNeighbor;
				}

				AStar.pouet++;

				if (AStar.pouet % 500 == 0) {
					Console.WriteLine("pouet: " + AStar.pouet + ". openSet.Count: " + this.openSet.Count + ". closedSet.Count: " + this.closedSet.Count + ". f: " + current.f);
					current.state.PrintBoard();
					Console.WriteLine("==========");
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

		private void UpdateSizeComplexity() {
			int t = openSet.Count + closedSet.Count;
			if (t > sizeComplexity) {
				sizeComplexity = t;
			}
		}

		private void UpdateTimeComplexity() {
			timeComplexity++;
		}
	}
}

