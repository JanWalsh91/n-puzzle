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
					throw new Exception("Exception 3");
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
					if (input[i][y] == size + 1) {
						onEvenRow = (i + 1) % 2 == 0;
					}
				}
			} 

			// check if all numbers are in order
			List<int> copy = new List<int>(listOfNumbers);
			copy.Sort();

			for (int i = 1; i < copy.Count; i++) {
				if (copy[i] - 1 != copy[i - 1]) {
					//Console.WriteLine("Pas content parce que : " + (copy[i] - 1) + " vs " + copy[i]]);
					throw new Exception("Exceptionn 1");
				}
			}

			if (copy[copy.Count - 1] != size + 1) {
				throw new Exception("Exceptionn 2");
			}
		}

		private void ValidateInversions() {
			int numberOfInversions = CountNumberOfInversions();

			//If N is odd, then puzzle instance is solvable if number of inversions is even in the input state.
			if (this.sideLength % 2 == 1) {
				if (numberOfInversions % 2 == 1) {
					throw new Exception("Exception 4");
				}
			} else {
				//If N is even, puzzle instance is solvable if
				//the blank is on an even row counting from the bottom(second-last, fourth - last, etc.) and number of inversions is odd.
				if (this.onEvenRow && numberOfInversions % 2 == 0) {
					throw new Exception("Exception 5");
				} else {
					//the blank is on an odd row counting from the bottom(last, third-last, fifth - last, etc.) and number of inversions is even.
					if (numberOfInversions % 2 != 0) {
						throw new Exception("Exception 6");
					}	
				}
			}

			//For all other cases, the puzzle instance is not solvable.
		}

		private int CountNumberOfInversions() {
			int num = 0;

			int deltaX = 1;
			int deltaY = 0;
			int x = 0;
			int y = 0;

			Console.WriteLine(sideLength);

			int stepsTaken = 1;
			int stepsToTake = sideLength;
			bool repeat = false;

			while (stepsToTake > 0) {
				Console.WriteLine("x: " + x + " Y:" + y);
				int prev = input[y][x];

				x += deltaX;
				y += deltaY;
				stepsTaken++;

				int current = input[y][x];
				if (prev > current) {
					num++;
				}
				Console.WriteLine("Stepstaken: " + stepsTaken);
				Console.WriteLine("Stepstotkae: " + stepsToTake);
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
			Console.WriteLine("Inversions: " + num);
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


