using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using server.src;
using System.Text;
using System.Xml.Serialization;

public class TcpTimeClient {
	private const int portNum = 2000;
	private const string hostName = "localhost";

	public static int Main(String[] args) {

		List<List<int>> input;

		if (args.Length > 0) {
			try {
				Parser parser = new Parser();
				input = parser.SolveFromFile(args[0]);
			} catch (ParserException pe) {
				Console.WriteLine(pe.Message);
				return 1;
			}
		} else {
			input = new List<List<int>>();
			
			// Solvable
			input.Add(new List<int>(new int[] { 2, 4, 1 }));
			input.Add(new List<int>(new int[] { 6, 5, 3 }));
			input.Add(new List<int>(new int[] { 8, 7, 0 }));

			//input.Add(new List<int>(new int[] { 1, 2, 3 }));
			//input.Add(new List<int>(new int[] { 4, 5, 6 }));
			//input.Add(new List<int>(new int[] { 7, 0, 8 }));
		}

		Validator validator = new Validator(input);

		try {
			validator.Validate();
		} catch (Exception e) {
			Console.WriteLine(e.Message);
			return 1;
		}


		try {
			TcpClient client = new TcpClient(hostName, portNum);

			NetworkStream ns = client.GetStream();

			input.Add(new List<int>{0, 0});

			BinaryFormatter bf = new BinaryFormatter();

			//XmlSerializer ser = new XmlSerializer(typeof(server.src.Server.Data));

			bf.Serialize(ns, input);
			//ser.Serialize(ns, data);

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
			//Console.WriteLine(e.ToString());
			Console.WriteLine("Connection error: " + e.Message);
		}

		return 0;
	}
}