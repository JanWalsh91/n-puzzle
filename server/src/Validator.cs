using System;
using System.Collections.Generic;

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
					if (input[i][y] == 0) {
						onEvenRow = (sideLength - i) % 2 == 0;
					}
				}
			}

			// check if all numbers are in order
			List<int> copy = new List<int>(listOfNumbers);
			copy.Sort();

			for (int i = 1; i < copy.Count; i++) {
				if (copy[i] - 1 != copy[i - 1]) {
					throw new ValidatorException("Input numbers non-contiguous");
				}
			}

			if (copy[0] != 0) {
				throw new ValidatorException("Missing empty cell");
			}
		}

		private void ValidateInversions(Board.SolutionType solutionType) {

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
			return num;
		}
	}
}


public class ValidatorException : Exception {
	public ValidatorException() { }
	public ValidatorException(string message) : base(message) { }
	public ValidatorException(string message, Exception inner) : base(message, inner) { }
}
