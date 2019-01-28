using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

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
				// parameters[2]: Heuristic Function (0, 1, 2) (2 is Uniform Search)
				// parameters[3]: Greedy Search (0, 1) (0: no, 1: yes, only for A*)
				List<int> parameters = input[input.Count - 1];
				input.RemoveAt(input.Count - 1);

				foreach (var item in input) {
					Console.WriteLine(String.Join(" - ", item));
				}


				Validator validator = new Validator(input);
				try {
					validator.Validate((Board.SolutionType)parameters[1]);
				} catch (ValidatorException ve) {
					Console.WriteLine(ve.Message);
					bf.Serialize(ns, new List<string> { "Error", ve.Message });
					continue;
				} catch (Exception e) {
					//TODO: handle, something very bad happened in that case
					Console.WriteLine(e.Message);
				}

				Board b1 = new Board(input);
				Board b2 = solutionTypeList[parameters[1]](input.Count);

				try {
					if (parameters[0] == 0) {
						algorithm = new AStar(ref b1, ref b2);
					} else if (parameters[0] == 1) {
						algorithm = new IDA(ref b1, ref b2);
					}
				} catch (OutOfMemoryException oome) {
					Console.WriteLine(":( " + oome.Message);
				}

				// TODO: Verify that
				algorithm.SetHeuristicFunction((HeuristicFunction.Types)parameters[2]);

				List<Node> solution = algorithm.Resolve();

				// Send response. Probably need try catch
				if (solution != null) {
					PrintSolution(solution);
					bf.Serialize(ns, GetStringSolution(solution, algorithm));
				}
				
				ns.Close();
				client.Close();
				
				// Send response (additional data?)
				//byte[] byteTime = Encoding.ASCII.GetBytes(DateTime.Now.ToString());
				//try {
				//	ns.Write(byteTime, 0, byteTime.Length);
				//	ns.Close();
				//	client.Close();
				//} catch (Exception e) {
				//	Console.WriteLine(e.ToString());
				//}
			}
			
			listener.Stop();
			
			return 0;
		}
	}
}