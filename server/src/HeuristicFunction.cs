using System;
namespace server.src {
	public class HeuristicFunction {

		public enum Types { MANHATTAN, OKLOP, EUCLIDIAN, UNIFORMCOST };

		public delegate float heuristicDelegate(Board board);
		public heuristicDelegate heuristic;

		heuristicDelegate[] heuristics = new heuristicDelegate[4];

		Board solution;

		public HeuristicFunction(ref Board solution) {
			this.solution = solution;
			this.heuristics[0] = this.Manhattan;
			this.heuristics[1] = this.Oklop;
			this.heuristics[2] = this.Euclidian;
			this.heuristics[3] = this.UniformCost;
			this.heuristic = heuristics[1];
		}

		public void SetHeuristic(Types type) {
			this.heuristic = heuristics[(int)type];
		}

		private float Manhattan (Board board) {
			float cost = 0.0f;

			for (int i = 0; i < board.GetSize() * board.GetSize(); i++) {
				int i2 = this.solution.GetIndexOf(board.GetList()[i]);
				int hDist = Math.Abs(i2 % board.GetSize() - i % board.GetSize());
				int vDist = Math.Abs(i2 / board.GetSize() - i / board.GetSize());
				cost += hDist + vDist;
			}

			return cost;
		}

		private float Euclidian(Board board) {
			float cost = 0.0f;

			for (int i = 0; i < board.GetSize() * board.GetSize(); i++) {
				int i2 = this.solution.GetIndexOf(board.GetList()[i]);
				double hDist = Math.Pow((i2 % board.GetSize() - i % board.GetSize()), 2.0);
				double vDist = Math.Pow((i2 / board.GetSize() - i / board.GetSize()), 2.0);
				cost += (float)hDist + (float)vDist;
			}

			return cost;
		}

		private float Oklop(Board board) {
			float cost = 0.0f;
			for (int i = 0; i < board.GetSize() * board.GetSize(); i++) {
				int i2 = this.solution.GetIndexOf(board.GetList()[i]);
				int hDist = Math.Abs(i2 % board.GetSize() - i % board.GetSize());
				int vDist = Math.Abs(i2 / board.GetSize() - i / board.GetSize());
				int big = Math.Max(hDist, vDist);
				int sml = Math.Min(hDist, vDist);

				cost += (5 * big - 4) - (big - sml) * 2;
			}

			return cost;
		}

		private float UniformCost(Board board) {
			return 0.0f;
		}
	}
}
