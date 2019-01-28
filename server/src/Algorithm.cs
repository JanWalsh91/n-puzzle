using System;
using System.Collections.Generic;
using System.Threading;

namespace server.src {
	abstract public class Algorithm {

		protected Board input;
		protected Board solution;
		protected int timeComplexity = 1;
		protected int sizeComplexity = 0;
		protected bool greedySearch = false;
		protected HeuristicFunction heuristicFunction;

		public abstract List<Node> Resolve(CancellationToken ct);

		public void SetHeuristicFunction(HeuristicFunction.Types type) {
			this.heuristicFunction.SetHeuristic(type);
		}

		public void SetGreedySearch(bool x) {
			Console.WriteLine("Set GreedySearch to " + x);
			this.greedySearch = x;
		}

		public int GetTimeComplexity() {
			return this.timeComplexity;
		}
		public int GetSizeComplexity() {
			return this.sizeComplexity;
		}
	}
}
