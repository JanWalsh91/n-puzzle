using System;
using System.Collections.Generic;
using System.Linq;

namespace server.src {
	public class Board {
		List<int> list = new List<int>();
		int size;

		public Board(List<List<int>> input) {
			// make list
			for (int i = input.Count - 1; i >= 0; i--) {
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

			var a = list.Except(item.list).ToList();
			var b = item.list.Except(list).ToList();
			return a.Count == 0 && b.Count == 0;
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
	}
}
