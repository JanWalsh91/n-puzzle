using System;
using System.Collections.Generic;
using System.Threading;

namespace server.src {
	public class IDA : Algorithm{

		Stack<Node> path;

		public IDA(ref Board input, ref Board solution) {
			this.input = input;
			this.solution = solution;
			heuristicFunction = new HeuristicFunction(ref solution);
		}

		private int currentSize = 1;

		public override List<Node> Resolve(CancellationToken ct) {
			this.StartTimer();

			Node root = new Node(ref this.input);
			root.g = 0;
			root.f = heuristicFunction.heuristic(root.state);

			float bound = heuristicFunction.heuristic(root.state);
			this.path = new Stack<Node>(new Node[] { root });

			while(true) {
				float t = this.Search(0, bound, ct);
				if (Math.Abs(t) < 0.001f) {
					List<Node> finalPath = new List<Node>(this.path);
					finalPath.Reverse();
					this.StopTimer();
					return finalPath; // FOUND
				} else if (float.IsPositiveInfinity(t)) {
					this.StopTimer();
					return new List<Node>(); // NOT FOUND
				}
				bound = t;
			}
		}

		private float Search(float g, float bound, CancellationToken ct) {

			if (ct.IsCancellationRequested) {
				ct.ThrowIfCancellationRequested();
			}

			Node node = path.Peek();
			float f = node.f;

			if (f > bound) {
				return f;
			}
			if (this.solution.Equals(node.state)) {
				return 0; // FOUND
			}
			float min = float.PositiveInfinity;
			List<Node> neighbors = GetNeighbors(ref node);

			currentSize += neighbors.Count;

			for (int i = 0; i < neighbors.Count; i++) {
				if (!path.Contains(neighbors[i])) {
					neighbors[i].g = node.g + 1;
					neighbors[i].f = neighbors[i].g + heuristicFunction.heuristic(neighbors[i].state);
					path.Push(neighbors[i]);
					UpdateTimeComplexity();
					float t = this.Search(0, bound, ct);
					if (Math.Abs(t) < 0.001f) {
						return 0; // FOUND
					}
					if (t < min) {
						min = t;
					}
					path.Pop();
				}
			}
			UpdateSizeComplexity();
			currentSize -= neighbors.Count;
			return min;
		}

		private List<Node> GetNeighbors(ref Node current) {
			List<string> hashes = current.GetNeighborHashes();
			List<Node> neighbors = new List<Node>();
			List<Node> tmpPath = new List<Node>(path);

			foreach (var hash in hashes) {
				if (tmpPath.Exists(x => x.hash == hash)) {
					neighbors.Add(tmpPath.Find(x => x.hash == hash));
				} else {
					Node node = new Node(hash);
					neighbors.Add(node);
				}
			}
			return neighbors;
		}

		private void UpdateSizeComplexity() {
			if (currentSize > sizeComplexity) {
				sizeComplexity = currentSize;
			}
		}

		private void UpdateTimeComplexity() {
			timeComplexity++;
		}
	}
}
