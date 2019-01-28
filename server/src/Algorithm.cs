using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
namespace server.src {
	abstract public class Algorithm {

		protected Board input;
		protected Board solution;
		protected bool greedySearch = false;
		protected int timeComplexity = 1;
		protected int sizeComplexity = 0;
		protected Stopwatch watch = null;
		protected HeuristicFunction heuristicFunction;

		public abstract List<Node> Resolve(CancellationToken ct);

		public void SetHeuristicFunction(HeuristicFunction.Types type) {
			this.heuristicFunction.SetHeuristic(type);
		}

		public void SetGreedySearch(bool x) {
			Console.WriteLine("Set GreedySearch to " + x);
			this.greedySearch = x;
		}

		protected void StartTimer() {
			this.watch = Stopwatch.StartNew();
		}

		protected void StopTimer() {
			this.watch.Stop();
		}

		public int GetTimeComplexity() {
			return this.timeComplexity;
		}
		public int GetSizeComplexity() {
			return this.sizeComplexity;
		}
		public int GetElaspedMS() {
			return (int)this.watch.ElapsedMilliseconds;
		}

	}
}
