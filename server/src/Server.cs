using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using server.src;

public class TcpTimeServer {

	private const int port = 2000;

	public static int Main(String[] args) {
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

			List<List<int>> input;
			input = (List<List<int>>)bf.Deserialize(ns);

			Validator validator = new Validator(input);
			try {
				validator.Validate();
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
			AStar aStar = null;
			Board b2 = Board.GetSnailSolution(input.Count);
			Board b1 = new Board(input);
			try {
				aStar = new AStar(ref b1, ref b2);
			} catch (OutOfMemoryException oome) {
				Console.WriteLine(":( " + oome.Message);
			}
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