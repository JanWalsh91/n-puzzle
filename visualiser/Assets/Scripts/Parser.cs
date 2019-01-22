using System;
using System.Collections.Generic;
using System.Linq;

public class Parser {

	public Parser() {}

	public List<List<int>> SolveFromFile(string fileName) {
		string[] lines = System.IO.File.ReadAllLines(fileName);
		List<List<int>> input = null;

		int N = -1;
		int currentLine = 0;

		for (int i = 0; i < lines.Length; i++) {
			//Console.WriteLine(lines[i]);

			lines[i] = lines[i].Trim();
			//Console.WriteLine("-" + lines[i] + "-");

			lines[i] = lines[i].Split('#')[0];
			if (lines[i].Length != 0) {

				if (N == -1) {
					// First line that corresponds to the puzzle dimension

					bool success = Int32.TryParse(lines[i], out N);
					//Console.WriteLine("N is now: " + N);
					if (!success) {
						throw new ParserException("Wrong format at " + fileName + ":" + (i + 1));
					}
					input = new List<List<int>>(N);
				} else {
					// Regular line
					input.Add(new List<int>(N));
					List<string> values = lines[i].Split(null).ToList();

					values.ForEach(s => s.Trim());
					values = values.Where(s => s.Length != 0).ToList();

					if (values.Count != N) {
						throw new ParserException("Wrong format (missing / too many values) at " + fileName + ":" + (i + 1));
					}
					for (int y = 0; y < N; y++) {
						int tmp;
						bool success = Int32.TryParse(values[y], out tmp);
						if (!success) {
							throw new ParserException("Wrong format (should be integers) at " + fileName + ":" + (i + 1));
						}
						//Console.WriteLine("i: " + currentLine + ", y: " + y);
						input[currentLine].Add(tmp);
					}
					currentLine++;
				}

			}
		}
		if (input.Count != N) {
			throw new ParserException("Wrong format (missing / too many values)" + " in " + fileName);
		}
		return input;
	}
}

public class ParserException : Exception {
	public ParserException() { }
	public ParserException(string message) : base(message) { }
	public ParserException(string message, Exception inner) : base(message, inner) { }
}
