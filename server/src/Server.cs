using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace server.src {
	public class Server {
		
		private const int port = 2000;
		private delegate Board SolutionType(int N);

		private static List<string> GetStringSolution(List<Node> moves, Algorithm algorithm) {
			List<string> pathToSolution = new List<string>();
			int emptyCellIndex;
			emptyCellIndex = moves[0].state.GetList().IndexOf(0);
			moves.RemoveAt(0);

			foreach (var move in moves) {
				List<int> currentList = move.state.GetList();
				int newEmptyCellIndex = currentList.IndexOf(0);
				if (newEmptyCellIndex / move.state.GetSize() == emptyCellIndex / move.state.GetSize()) {
					pathToSolution.Add(newEmptyCellIndex < emptyCellIndex ? "Right" : "Left");

				} else {
					pathToSolution.Add(newEmptyCellIndex < emptyCellIndex ? "Down" : "Up");
				}
				emptyCellIndex = newEmptyCellIndex;
			}

			// Add Number of Moves
			pathToSolution.Add("Number of Moves: " + pathToSolution.Count.ToString());

			// Add Complexity In Time
			pathToSolution.Add("Complexity In Time: " + algorithm.GetTimeComplexity());

			// Add Complexity In Size
			pathToSolution.Add("Complexity In Size: " + algorithm.GetSizeComplexity());

			// Add Elasped Time
			pathToSolution.Add("Elapsed Time: " + algorithm.GetElaspedMS() + "ms");

			return pathToSolution;
		}

		private static void PrintSolution(List<Node> moves) {
			if (moves == null) {
				Console.WriteLine("No Solution Found");
			} else {
				foreach (var move in moves) {
					move.state.PrintBoard();
				}
			}
		}

		public static int Main(String[] args) {
			List<SolutionType> solutionTypeList = new List<SolutionType>{
				Board.GetSnailSolution,
				Board.GetRegularSolution
			};


			Algorithm algorithm = null;

			bool done = false;

			TcpListener listener = new TcpListener(IPAddress.Loopback, port);
			
			listener.Start();
			
			while (!done) {
				Console.Write("Waiting for connection...");
				TcpClient client = listener.AcceptTcpClient();
				
				Console.WriteLine("Connection accepted.");
				NetworkStream ns = client.GetStream();

				// Read request
				BinaryFormatter bf = new BinaryFormatter();

				List<List<int>> input = (List<List<int>>)bf.Deserialize(ns);

				// parameters[0]: Algo Type (0, 1)
				// parameters[1]: Solution Type (0, 1)
				// parameters[2]: Heuristic Function (0, 1, 2, 3) MANHATTAN, OKLOP, EUCLIDIAN, UNIFORMCOST
				// parameters[3]: Greedy Search (0, 1) (0: no, 1: yes, only for A*)
				List<int> parameters = input[input.Count - 1];
				input.RemoveAt(input.Count - 1);

				foreach (var item in parameters) {
					Console.WriteLine(item);
				}

				foreach (var item in input) {
					Console.WriteLine(String.Join(" - ", item));
				}

				Board bSol = solutionTypeList[parameters[1]](input.Count);

				Console.WriteLine("Solution Board:" );
				bSol.PrintBoard();

				Console.WriteLine("Original Board: ");
				foreach (var item in input) {
					Console.WriteLine(String.Join(" - ", item));
				}

				Console.WriteLine("Solution Type: " + parameters[1]);


				Validator validator = new Validator(input, bSol.Get2DList());
				try {
					validator.Validate((Board.SolutionType)parameters[1]);
				} catch (ValidatorException ve) {
					Console.WriteLine(ve.Message);
					bf.Serialize(ns, new List<string> { "Error", ve.Message });
					continue;
				} catch (Exception e) {
					//TODO: handle, something very bad happened in that case
					Console.WriteLine(e.Message);
					continue;
				}

				Board b1 = new Board(input);

				try {
					if (parameters[0] == 0) {
						algorithm = new AStar(ref b1, ref bSol);
					} else if (parameters[0] == 1) {
						algorithm = new IDA(ref b1, ref bSol);
					}
				} catch (OutOfMemoryException oome) {
					Console.WriteLine(":( " + oome.Message);
				}

				algorithm.SetHeuristicFunction((HeuristicFunction.Types)parameters[2]);
				algorithm.SetGreedySearch(parameters[3] == 1);

				List<Node> solution = null;

				CancellationTokenSource tokenSource = new CancellationTokenSource();
				CancellationToken token = tokenSource.Token;

				var task = Task.Run(() => algorithm.Resolve(token), token);

				tokenSource.CancelAfter(TimeSpan.FromSeconds(30));
				try {
					task.Wait();
					solution = task.Result;
				} catch (AggregateException e) {
					foreach (var v in e.InnerExceptions) {
						Console.WriteLine(e.Message + " " + v.Message);
					}
				} finally {
					tokenSource.Dispose();
				}

				if (solution != null) {
					PrintSolution(solution);
					Console.WriteLine("Elapsed Time: " + algorithm.GetElaspedMS() + "ms");
					bf.Serialize(ns, GetStringSolution(solution, algorithm));
				}
				
				ns.Close();
				client.Close();
			}
			
			listener.Stop();
			
			return 0;
		}
	}
}