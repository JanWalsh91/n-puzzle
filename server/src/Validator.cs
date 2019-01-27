using System;
using System.Collections.Generic;

// https://www.geeksforgeeks.org/check-instance-15-puzzle-solvable/

namespace server.src {
	public class Validator {

		List<List<int>> input;
		List<List<int>> solution;
		int size;									// N * N - 1
		int sideLength;								// N
		List<int> listOfNumbers = new List<int>();
		bool onEvenRow = false;

		public Validator(List<List<int>> input, List<List<int>> solution) {
			this.input = input;
			this.solution = solution;
		}

		public void Validate(Board.SolutionType solutionType) {
			this.size = input.Count * input.Count - 1;
			ValidateShape();
			ValidateNumbers();
			ValidateInversions(solutionType);
		}

		private void ValidateShape() {
			this.sideLength = input.Count;
			foreach (var i in input) {
				if (i.Count != this.sideLength) {
					throw new ValidatorException("Invalid input shape");
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
					throw new ValidatorException("Input numbers non-contiguous");
				}
			}

			if (copy[0] != 0) {
				throw new ValidatorException("Missing empty cell");
			}
		}

		private void ValidateInversions(Board.SolutionType solutionType) {

			//int numberOfInversions = solutionType == Board.SolutionType.Snail ? CountNumberOfSnailInversions(input) : CountNumberOfRegularInversions(input);
			//int numberOfInversionsSolution = solutionType == Board.SolutionType.Snail ? CountNumberOfSnailInversions(solution) : CountNumberOfRegularInversions(solution);

			int numberOfInversions = CountNumberOfRegularInversions(input);
			int numberOfInversionsSolution = CountNumberOfRegularInversions(solution);

			int start0Index = -1;
			int goal0Index = -1;

			for (int i = 0; i < input.Count; i++) {
				start0Index = input[i].FindIndex(c => c == 0);
				if (start0Index > -1) {
					start0Index = i * input.Count + start0Index;
					break;
				}
			}
			for (int i = 0; i < solution.Count; i++) {
				goal0Index = solution[i].FindIndex(c => c == 0);
				if (goal0Index > -1) {
					goal0Index = i * solution.Count + goal0Index;
					break;
				}
			}
			if (input.Count % 2 == 0) { // In this case, the row of the '0' tile matters
				numberOfInversions += start0Index / input.Count;
				numberOfInversionsSolution += goal0Index / solution.Count;
			}


			if (numberOfInversions % 2 != numberOfInversionsSolution % 2) {
				throw new ValidatorException("Unsolvable puzzle");
			}

			//if (this.sideLength % 2 == 1) {
			//	if (numberOfInversions % 2 == 1) {
			//		throw new ValidatorException("Unsolvable puzzle (1)");
			//	}
			//} else {
			//	//If N is even, puzzle instance is solvable if
			//	//the blank is on an even row counting from the bottom(second-last, fourth - last, etc.) and number of inversions is odd.
			//	if (this.onEvenRow && numberOfInversions % 2 == 0) {
			//		Console.WriteLine("Inersions: " + numberOfInversions + " onEvenRow: " + onEvenRow);
			//		throw new ValidatorException("Unsolvable puzzle (2)");
			//	} else if (!this.onEvenRow && numberOfInversions % 2 == 1) {
			//		//the blank is on an odd row counting from the bottom(last, third-last, fifth - last, etc.) and number of inversions is even.
			//		if (numberOfInversions % 2 != 0) {
			//			throw new ValidatorException("Unsolvable puzzle (3)");
			//		}
			//	}
			//}
		}

		private int CountNumberOfRegularInversions(List<List<int>> toCheck) {
			int num = 0;

			List<int> inputList = new List<int>();
			foreach (var item in toCheck) {
				inputList.AddRange(item);
			}

			for (int i = 0; i < size; i++) {
				for (int j = i + 1; j < size + 1; j++) {
					if (inputList[j] != 0 && inputList[i] != 0 && inputList[i] > inputList[j]) {
						num++;
					}
				}
			}
			Console.WriteLine("Number of inversions: " + num);
			return num;
		}

		//private int CountNumberOfSnailInversions(List<List<int>> toCheck) {
		//	int num = 0;

		//	int deltaX = 1;
		//	int deltaY = 0;
		//	int x = 0;
		//	int y = 0;

		//	int stepsTaken = 1;
		//	int stepsToTake = sideLength;
		//	bool repeat = false;

		//	List<int> unrolledSnail = new List<int>();

		//	while (stepsToTake > 0) {
		//		unrolledSnail.Add(toCheck[y][x]);
		//		x += deltaX;
		//		y += deltaY;
		//		stepsTaken++;

		//		if (stepsTaken == stepsToTake) {
		//			Board.UpdateDirection(ref deltaX, ref deltaY);
		//			if (!repeat) {
		//				stepsToTake--;
		//				repeat = true;
		//			} else {
		//				repeat = false;
		//			}
		//			stepsTaken = 0;
		//		}
		//	}
		//	unrolledSnail.Add(toCheck[y][x]);

		//	Console.WriteLine("=== Unrolled snail === ");
		//	for (int z = 0; z < unrolledSnail.Count; z++) {
		//		Console.WriteLine(unrolledSnail[z]);
		//	}

		//	for (int i = 0; i < sideLength * sideLength - 1; i++) {
		//		for (int j = i + 1; j < sideLength * sideLength; j++) {
		//			// count pairs(i, j) such that i appears 
		//			// before j, but i > j. 
		//			if (unrolledSnail[j] != 0 && unrolledSnail[i] != 0 && unrolledSnail[i] > unrolledSnail[j]) {
		//				//Console.WriteLine("Inversion: " + unrolledSnail[i] + " > " + unrolledSnail[j]);
		//				num++;
		//			}
		//		}
		//	}

		//	Console.WriteLine("Number of inversions: " + num);

		//	return num;
		//}


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


public class ValidatorException : Exception {
	public ValidatorException() { }
	public ValidatorException(string message) : base(message) { }
	public ValidatorException(string message, Exception inner) : base(message, inner) { }
}
