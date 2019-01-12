﻿using System;
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
				list[y][x] = i;
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
			Console.WriteLine("UpdateDirection, X: " + deltaX + " Y: " + deltaY);
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
				if (this.list[i] == this.size * this.size) {
					Console.Write('@');
				} else {
					Console.Write(this.list[i]);
				}
				Console.Write('\t');
				if ((i + 1) % this.size == 0) {
					Console.Write('\n');
				}
			}
		}

		public static List<Board> GetNeighbors(Board current) {
			int index = current.GetIndexOfEmpty();
			int x = index % current.size;
			int y = index / current.size;
			List<Board> boards = new List<Board>();


			if (x != 0) {
				List<int> list = new List<int>(current.list);
				list[index] = list[index - 1];
				list[index - 1] = current.size * current.size;
				boards.Add(new Board(list));
			}
			if (x != current.size - 1) {
				List<int> list = new List<int>(current.list);
				list[index] = list[index + 1];
				list[index + 1] = current.size * current.size;
				boards.Add(new Board(list));
			}
			if (y != 0) {
				List<int> list = new List<int>(current.list);
				list[index] = list[index - current.size];
				list[index - current.size] = current.size * current.size;
				boards.Add(new Board(list));
			}
			if (y != current.size - 1) {
				List<int> list = new List<int>(current.list);
				list[index] = list[index + current.size];
				list[index + current.size] = current.size * current.size;
				boards.Add(new Board(list));
			}
			return boards;
		}

		int GetIndexOfEmpty() {
			return list.IndexOf(this.size * this.size);
		}
	}
}
