using System;
using System.Collections.Generic;

namespace server.src {
	public class HeuristicFunction {

		public enum Types { MANHATTAN, EUCLIDIAN, OKCLOP, GWYNEFIS, UNIFORMCOST };

		public delegate float heuristicDelegate(Board board);
		public heuristicDelegate heuristic;

		heuristicDelegate[] heuristics = new heuristicDelegate[5];

		Board solution;

		public HeuristicFunction(ref Board solution) {
			this.solution = solution;
			this.heuristics[0] = this.Manhattan;
			this.heuristics[1] = this.Euclidian;
			this.heuristics[2] = this.Oklop;
			this.heuristics[3] = this.Gwynefis;
			this.heuristics[4] = this.UniformCost;
			this.heuristic = heuristics[1];
		}

		public void SetHeuristic(Types type) {
			this.heuristic = heuristics[(int)type];
		}

		private float Manhattan (Board board) {
			float cost = 0.0f;
			List<int> list = board.GetList();

			for (int i = 0; i < board.GetSize() * board.GetSize(); i++) {
				int i2 = this.solution.GetIndexOf(list[i]);
				int hDist = Math.Abs(i2 % board.GetSize() - i % board.GetSize());
				int vDist = Math.Abs(i2 / board.GetSize() - i / board.GetSize());
				cost += hDist + vDist;
			}

			return cost;
		}

		private float Euclidian(Board board) {
			float cost = 0.0f;
			List<int> list = board.GetList();

			for (int i = 0; i < board.GetSize() * board.GetSize(); i++) {
				int i2 = this.solution.GetIndexOf(list[i]);
				double hDist = Math.Pow((i2 % board.GetSize() - i % board.GetSize()), 2.0);
				double vDist = Math.Pow((i2 / board.GetSize() - i / board.GetSize()), 2.0);
				cost += (float)hDist + (float)vDist;
			}

			return cost;
		}

		private float Oklop(Board board) {
			float cost = 0.0f;
			List<int> list = board.GetList();

			for (int i = 0; i < board.GetSize() * board.GetSize(); i++) {
				int i2 = this.solution.GetIndexOf(list[i]);
				int hDist = Math.Abs(i2 % board.GetSize() - i % board.GetSize());
				int vDist = Math.Abs(i2 / board.GetSize() - i / board.GetSize());
				int big = Math.Max(hDist, vDist);
				int sml = Math.Min(hDist, vDist);
				int hMvt = 5 * big - (big > 1 ? 4 : 0);
				int dMvt = (big - sml) * 6 - ((big - sml) > 1 ? 2 : 0);
				cost += hMvt + dMvt;
			}

			return cost;
		}

		private float Gwynefis(Board board) {
			float cost = 0.0f;
			List<int> list = board.GetList();

			for (int i = 0; i < board.GetSize() * board.GetSize(); i++) {
				int i2 = this.solution.GetIndexOf(list[i]);
				int hDist = Math.Abs(i2 % board.GetSize() - i % board.GetSize());
				int vDist = Math.Abs(i2 / board.GetSize() - i / board.GetSize());
				int big = Math.Max(hDist, vDist);
				int sml = Math.Min(hDist, vDist);
				int hMvt = 5 * big - (big > 1 ? 4 : 0);
				int dMvt = (big - sml) * 6 - ((big - sml) > 1 ? 2 : 0);
				int add = hMvt + dMvt;
				cost += add + (add == 0 ? ReduceCostForCorrectPiece() : 0);
			}

			return cost + ReduceCostPerCompleteLine(board);
		}

		private float ReduceCostForCorrectPiece() {
			return -1000;
		}

		private float ReduceCostPerCompleteLine(Board board) {
			float reducedCost = 0;
			List<int> bList = board.GetList();
			List<int> sList = solution.GetList();
			int size = board.GetSize();
			bool ok = true;

			for (int i = 0; i < size; i++) {

				ok = true;
				for (int y = 0; y < size; y++) {
					if (bList[i * size + y] != sList[i * size + y]) {
						ok = false;
						break;
					}
				}
				if (ok) { reducedCost -= size * 100; }
			}
			return reducedCost;
		}

		private float UniformCost(Board board) {
			return 0.0f;
		}
	}
}
