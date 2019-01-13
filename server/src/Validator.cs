using System;
using System.Collections.Generic;

// https://www.geeksforgeeks.org/check-instance-15-puzzle-solvable/

namespace server.src {
	public class Validator {

		List<List<int>> input;
		int size;									// N * N - 1
		int sideLength;								// N
		List<int> listOfNumbers = new List<int>();
		bool onEvenRow = false;

		public Validator(List<List<int>> input) {
			this.input = input;
		}

		public void Validate() {
			this.size = input.Count * input.Count - 1;
			ValidateShape();
			ValidateNumbers();
			ValidateInversions();
			Console.WriteLine("Done here");
		}

		private void ValidateShape() {
			this.sideLength = input.Count;
			foreach (var i in input) {
				if (i.Count != this.sideLength) {
					throw new Exception("V: Invalid input shape");
				}
			}
		}

		private void ValidateNumbers() {
			int biggestNum = 0;
			listOfNumbers = new List<int>();

			for (int i = input.Count - 1; i >= 0; i--) {
				for (int y = 0; y < input[i].Count; y++) {
					listOfNumbers.Add(input[i][y]);
					if (biggestNum < input[i][y]) {
						biggestNum = input[i][y];
					}
					//Console.WriteLine("i; " + i);
					if (input[i][y] == 0) {
						onEvenRow = (sideLength - i) % 2 == 0;
						//Console.WriteLine("Row from bottom: " + (sideLength - i) + " onEvenRow: " + onEvenRow);
					}
				}
			}

			// check if all numbers are in order
			List<int> copy = new List<int>(listOfNumbers);
			copy.Sort();

			for (int i = 1; i < copy.Count; i++) {
				if (copy[i] - 1 != copy[i - 1]) {
					//Console.WriteLine("Pas content parce que : " + (copy[i] - 1) + " vs " + copy[i]]);
					throw new Exception("V: Input numbers non-contiguous");
				}
			}

			if (copy[0] != 0) {
				throw new Exception("V: Missing empty square");
			}
		}

		private void ValidateInversions() {
			int numberOfInversions = CountNumberOfInversions();

			//If N is odd, then puzzle instance is solvable if number of inversions is even in the input state.
			if (this.sideLength % 2 == 1) {
				if (numberOfInversions % 2 == 1) {
					throw new Exception("V: Unsolvable (1)");
				}
			} else {
				//If N is even, puzzle instance is solvable if
				//the blank is on an even row counting from the bottom(second-last, fourth - last, etc.) and number of inversions is odd.
				if (this.onEvenRow && numberOfInversions % 2 == 0) {
					throw new Exception("V: Unsolvable (2)");
				} else {
					//the blank is on an odd row counting from the bottom(last, third-last, fifth - last, etc.) and number of inversions is even.
					if (numberOfInversions % 2 != 0) {
						throw new Exception("V: Unsolvable (3)");
					}	
				}
			}

		}

		private int CountNumberOfInversions() {
			int num = 0;

			int deltaX = 1;
			int deltaY = 0;
			int x = 0;
			int y = 0;

			int stepsTaken = 1;
			int stepsToTake = sideLength;
			bool repeat = false;

			List<int> unrolledSnail = new List<int>();

			while (stepsToTake > 0) {
				unrolledSnail.Add(input[y][x]);
				x += deltaX;
				y += deltaY;
				stepsTaken++;

				if (stepsTaken == stepsToTake) {
					Board.UpdateDirection(ref deltaX, ref deltaY);
					if (!repeat) {
						stepsToTake--;
						repeat = true;
					} else {
						repeat = false;
					}
					stepsTaken = 0;
				}
			}
			unrolledSnail.Add(input[y][x]);

			//for (int z = 0; z < unrolledSnail.Count; z++) {
			//	Console.WriteLine(unrolledSnail[z]);
			//}

			for (int i = 0; i < sideLength * sideLength - 1; i++) {
				for (int j = i + 1; j < sideLength * sideLength; j++) {
					// count pairs(i, j) such that i appears 
					// before j, but i > j. 
					if (unrolledSnail[j] != 0 && unrolledSnail[i] != 0 && unrolledSnail[i] > unrolledSnail[j]) {
						num++;
					}
				}
			}

			Console.WriteLine("Number of inversions: " + num);

			return num;
		}

		//public static void Main () {

		//	try {
		//		List<List<int>> input = new List<List<int>>();
				
		//		input.Add(new List<int>(new int[] { 1, 2, 3 }));
		//		input.Add(new List<int>(new int[] { 4, 5, 6 }));
		//		input.Add(new List<int>(new int[] { 7, 8, 9 }));
				
		//		Validator v = new Validator(input);
				
		//		v.Validate();
				
		//	} catch (Exception e) {
		//		Console.WriteLine("LA LIGNE DE THEO");
		//		Console.WriteLine(e);
		//	}

		//}
	}
}


