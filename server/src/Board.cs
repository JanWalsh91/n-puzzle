using System;
using System.Collections.Generic;
using System.Linq;

namespace server.src {
	public class Board {
		List<int> list = new List<int>();
		int size;

		public Board(List<List<int>> input) {
			// make list
			for (int i = 0; i < input.Count; i++) {
				for (int y = 0; y < input[i].Count; y++) {
					this.list.Add(input[i][y]);
				}
			}

			// set size
			this.size = input.Count;
		}

		public override bool Equals(object obj) {

			if (obj.GetType() != typeof(Board)) {
				return false;
			}
			var item = obj as Board;

			if (this.size != item.size) {
				return false;
			}

			for (int i = 0; i < list.Count; i++) {
				if (list[i] != item.list[i]) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public int GetIndexOf(int i) {
			return list.IndexOf(i);
		}

		public int GetSize() {
			return this.size;
		}

		public List<int> GetList() {
			return this.list;
		}

		public static Board GetSnailSolution(int N) {
			List<List<int>> list = new List<List<int>>();
			for (int i = 0; i < N; i++) {
				list.Add(Enumerable.Repeat(0, N).ToList());
			}

			int deltaX = 1;
			int deltaY = 0;
			int x = 0;
			int y = 0;

			for (int i = 1; i <= N * N; i++) {
				Console.WriteLine("Placed " + i + " at " + x + " " + y);
				list[y][x] = i;
				if (x + deltaX >= N || y + deltaY >= N || x + deltaX < 0 || y + deltaY < 0) {
					UpdateDirection(ref deltaX, ref deltaY);
				} else if (i < N * N && list[y + deltaY][x + deltaX] != 0) {
					UpdateDirection(ref deltaX, ref deltaY);
				}

				x += deltaX;
				y += deltaY;

			}

			return new Board(list);

		}

		static void UpdateDirection(ref int deltaX, ref int deltaY) {
			Console.WriteLine("UpdateDirection");
			Console.WriteLine("dX: " + deltaX + " dY: " + deltaY);
			if (deltaX == -1) {
				deltaX = 0;
				deltaY = -1;
			} else if (deltaX == 1) {
				deltaX = 0;
				deltaY = 1;
			} else if (deltaY == -1) {
				deltaY = 0;
				deltaX = 1;
			} else if (deltaY == 1) {
				deltaY = 0;
				deltaX = -1;
			}
			Console.WriteLine("dX: " + deltaX + " dY: " + deltaY);
		}

		static void UpdateCoords(ref int x, ref int y, int size) {
			x++;
			if (x >= size) {
				x = 0;
			}
		}

		public void PrintBoard() {
			Console.WriteLine("BOARD");
			for (int i = 0; i < this.size * this.size; i++) {
				Console.Write(this.list[i]);
				Console.Write(' ');
				if ((i + 1) % this.size == 0) {
					Console.Write('\n');
				}
			}
		}
	}
}
