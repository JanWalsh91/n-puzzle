using System;
using System.Collections.Generic;
using System.Linq;

namespace server.src {
	public class Board {
		List<int> list = new List<int>();
		int size;								// N

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

		public Board(Board board) {
			this.list = new List<int>(board.list);
			this.size = board.size;
		}

		public Board(List<int> input) {
			this.list = new List<int>(input);
			this.size = (int)Math.Sqrt(input.Count);
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

		public override string ToString() {
			return String.Join(";", list.ToArray());
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
				list[y][x] = i == N * N ? 0 : i;

				if (x + deltaX >= N || y + deltaY >= N || x + deltaX < 0 || y + deltaY < 0) {
					UpdateDirection(ref deltaX, ref deltaY);
				} else if (i < N * N && list[y + deltaY][x + deltaX] != 0) {
					UpdateDirection(ref deltaX, ref deltaY);
				}

				x += deltaX;
				y += deltaY;
			}

			Console.WriteLine("Solution: ");
			Board board = new Board(list);
			board.PrintBoard();
			return board;

		}

		public static void UpdateDirection(ref int deltaX, ref int deltaY) {
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
			//Console.WriteLine("UpdateDirection, X: " + deltaX + " Y: " + deltaY);
		}

		static void UpdateCoords(ref int x, ref int y, int size) {
			x++;
			if (x >= size) {
				x = 0;
			}
		}

		public void PrintBoard() {
			Console.WriteLine("PrintBoard: ");
			for (int i = 0; i < this.size * this.size; i++) {
				Console.Write(this.list[i]);
				Console.Write('\t');
				if ((i + 1) % this.size == 0) {
					Console.Write('\n');
				}
			}
		}

		public List<string> GetNeighborHashes() {
			int index = this.GetIndexOfEmpty();
			int x = index % this.size;
			int y = index / this.size;
			List<string> hashes = new List<string>();


			if (x != 0) {
				List<int> list2 = new List<int>(this.list);
				list2[index] = list2[index - 1];
				list2[index - 1] = 0;
				hashes.Add(String.Join(";", list2.ToArray()));
			}
			if (x != this.size - 1) {
				List<int> list2 = new List<int>(this.list);
				list2[index] = list2[index + 1];
				list2[index + 1] = 0;
				hashes.Add(String.Join(";", list2.ToArray()));
			}
			if (y != 0) {
				List<int> list2 = new List<int>(this.list);
				list2[index] = list2[index - this.size];
				list2[index - this.size] = 0;
				hashes.Add(String.Join(";", list2.ToArray()));
			}
			if (y != this.size - 1) {
				List<int> list2 = new List<int>(this.list);
				list2[index] = list2[index + this.size];
				list2[index + this.size] = 0;
				hashes.Add(String.Join(";", list2.ToArray()));
			}
			return hashes;
		}

		int GetIndexOfEmpty() {
			return list.IndexOf(0);
		}
	}
}
