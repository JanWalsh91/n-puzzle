using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;

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
				//XmlSerializer ser = new XmlSerializer(typeof(Data));

				//Data data = (Data)ser.Deserialize(ns);

				//Console.WriteLine(data.input.Count);

				//byte[] bytes = new byte[1024];
				//List<byte> byteList = new List<byte>();
				//int nbBytes = 0;
				//do {
				//	nbBytes = ns.Read(bytes, 0, bytes.Length);
				//	byteList.AddRange(bytes.Take(nbBytes));
				//} while (ns.DataAvailable);

				//Data pouet = new Data();
				//pouet.input = bf.Deserialize()
				//pouet.heuristicFunction = BitConverter.ToInt32(byteList.ToArray(), byteList.Count - 8);
				//pouet.solutionType = BitConverter.ToInt32(byteList.ToArray(), byteList.Count - 4);


				List<List<int>> input = (List<List<int>>)bf.Deserialize(ns);

				List<int> parameters = input[input.Count - 1];
				input.RemoveAt(input.Count - 1);

				foreach (var item in input) {
					Console.WriteLine(String.Join(" - ", item));
				}
				
				Validator validator = new Validator(input);
				try {
					validator.Validate();
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