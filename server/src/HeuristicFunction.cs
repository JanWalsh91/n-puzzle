using System;
namespace server.src {
	public class HeuristicFunction {

		public enum Types { MANHATTAN, A, B };

		public delegate float heuristicDelegate(Board board);
		public heuristicDelegate heuristic;

		heuristicDelegate[] heuristics = new heuristicDelegate[3];

		Board solution;

		public HeuristicFunction(ref Board solution) {
			this.solution = solution;
			this.heuristics[0] = this.Manhattan;
			this.heuristics[1] = this.A;
			this.heuristics[2] = this.B;
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

		private float A(Board board) {
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

		private float B(Board board) {
			return 0.0f;
		}
	}
}
