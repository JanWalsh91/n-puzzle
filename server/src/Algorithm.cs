using System;
using System.Collections.Generic;

namespace server.src {
	abstract public class Algorithm {

		protected Board input;
		protected Board solution;
		protected int timeComplexity = 1;
		protected int sizeComplexity = 0;
		protected HeuristicFunction heuristicFunction;

		public abstract List<Node> Resolve();
		public void SetHeuristicFunction(HeuristicFunction.Types type) {
			heuristicFunction.SetHeuristic(type);
		}

		public int GetTimeComplexity() {
			return this.timeComplexity;
		}
		public int GetSizeComplexity() {
			return this.sizeComplexity;
		}
	}
}
