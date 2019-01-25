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
				
				Validator validator = new Validator(input);
				try {
					validator.Validate((Board.SolutionType)parameters[0]);
				} catch (ValidatorException ve) {
					Console.WriteLine(ve.Message);
					bf.Serialize(ns, new List<string> { "Error", ve.Message });
					continue;
				} catch (Exception e) {
					//TODO: handle, something very bad happened in that case
					Console.WriteLine(e.Message);
				}
				AStar aStar = null;

				Board b1 = new Board(input);
				Board b2 = solutionTypeList[parameters[0]](input.Count);
				
				try {
					aStar = new AStar(ref b1, ref b2);
				} catch (OutOfMemoryException oome) {
					Console.WriteLine(":( " + oome.Message);
				}

				// TODO: Verify that
				aStar.SetHeuristicFunction((HeuristicFunction.Types)parameters[1]);

				List<Node> solution = aStar.Resolve();
				
				// Send response. Probably need try catch
				if (solution != null) {
					aStar.PrintSolution(solution);
					bf.Serialize(ns, aStar.GetStringSolution(solution));
					//aStar.PrintStringSolution(solution);
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