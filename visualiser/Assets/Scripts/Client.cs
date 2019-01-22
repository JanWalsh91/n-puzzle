using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Client {
	private const int portNum = 2000;
	private const string hostName = "localhost";

	public void CallServer(List<List<int>> input) {
		
		//Validator validator = new Validator(input);

		//try {
		//	validator.Validate();
		//} catch (Exception e) {
		//	Console.WriteLine(e.Message);
		//	return 1;
		//}


		try {
			TcpClient client = new TcpClient(hostName, portNum);

			NetworkStream ns = client.GetStream();

			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(ns, input);

			List<string> solution;
			solution = (List<string>)bf.Deserialize(ns);

			foreach (var item in solution) {
				Debug.Log(item);
			}

			//byte[] bytes = new byte[1024];
			//int bytesRead = ns.Read(bytes, 0, bytes.Length);
			//Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));

			client.Close();

		} catch (Exception e) {
			//Console.WriteLine(e.ToString());
			Debug.Log("Connection error: " + e.Message);
		}

		//return 0;
	}
}