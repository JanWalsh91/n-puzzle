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

		public static int Main(String[] args) {
			List<SolutionType> solutionTypeList = new List<SolutionType>{
				Board.GetSnailSolution,
				Board.GetRegularSolution
			};

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

				// parameters[0]: Solution Type
				// parameters[1]: Heuristic Function
				List<int> parameters = input[input.Count - 1];
				input.RemoveAt(input.Count - 1);

				foreach (var item in input) {
					Console.WriteLine(String.Join(" - ", item));
				}

				Board bSol = solutionTypeList[parameters[0]](input.Count);

				Console.WriteLine("Solution Board:" );
				bSol.PrintBoard();

				Console.WriteLine("Original Board: ");
				foreach (var item in input) {
					Console.WriteLine(String.Join(" - ", item));
				}

				Console.WriteLine("Solution Type: " + parameters[0]);

				Validator validator = new Validator(input, bSol.Get2DList());
				try {
					validator.Validate((Board.SolutionType)parameters[0]);
				} catch (ValidatorException ve) {
					Console.WriteLine(ve.Message);
					bf.Serialize(ns, new List<string> { "Error", ve.Message });
					continue;
				} catch (Exception e) {
					//TODO: handle, something very bad happened in that case
					Console.WriteLine(e.Message);
					continue;
				}

				AStar aStar = null;
				//IDA ida = null;

				Board b1 = new Board(input);

				try {
					aStar = new AStar(ref b1, ref bSol);
					//ida = new IDA(ref b1, ref bSol);
				} catch (OutOfMemoryException oome) {
					Console.WriteLine(":( " + oome.Message);
				}

				// TODO: Verify that
				aStar.SetHeuristicFunction((HeuristicFunction.Types)parameters[1]);
				//ida.SetHeuristicFunction((HeuristicFunction.Types)parameters[1]);


				List<Node> solution = null;


				CancellationTokenSource tokenSource = new CancellationTokenSource();
				CancellationToken token = tokenSource.Token;

				var task = Task.Run(() => aStar.Resolve(token), token);

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

				// Send response. Probably need try catch
				if (solution != null) {
					//aStar.PrintSolution(solution);
					//ida.PrintSolution(solution);
					bf.Serialize(ns, aStar.GetStringSolution(solution));

					//bf.Serialize(ns, ida.GetStringSolution(solution));
					//aStar.PrintSolution(solution);
				}
				
				ns.Close();
				client.Close();
			}
			
			listener.Stop();
			
			return 0;
		}
	}
}