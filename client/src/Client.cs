using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public class TcpTimeClient {
	private const int portNum = 2000;
	private const string hostName = "localhost";

	public static int Main(String[] args) {


		List<List<int>> input = new List<List<int>>();

		// Solvable
		//input.Add(new List<int>(new int[] { 2, 4, 1 }));
		//input.Add(new List<int>(new int[] { 6, 5, 3 }));
		//input.Add(new List<int>(new int[] { 8, 7, 0 }));
	

		try {
			TcpClient client = new TcpClient(hostName, portNum);

			NetworkStream ns = client.GetStream();



			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(ns, input);

			List<string> solution;
			solution = (List<string>)bf.Deserialize(ns);

			foreach (var item in solution) {
				Console.WriteLine(item);
			}

			//byte[] bytes = new byte[1024];
			//int bytesRead = ns.Read(bytes, 0, bytes.Length);
			//Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));

			client.Close();

		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}

		return 0;
	}
}