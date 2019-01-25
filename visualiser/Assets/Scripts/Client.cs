using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Client: MonoBehaviour {

	public int portNum = 2000;
	public string hostName = "localhost";
	public BoardManager boardManager;

	public List<string> CallServer(List<List<int>> input) {
		
		try {
			TcpClient client = new TcpClient(hostName, portNum);

			NetworkStream ns = client.GetStream();

			BinaryFormatter bf = new BinaryFormatter();

			input.Add(new List<int> { 0, 0 });

			try {
				bf.Serialize(ns, input);
			} catch (Exception e) {
				Debug.Log(e.Message);
			}
			List<string> solution;
			solution = (List<string>)bf.Deserialize(ns);

			//byte[] bytes = new byte[1024];
			//int bytesRead = ns.Read(bytes, 0, bytes.Length);
			//Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));


			client.Close();
			return solution;
		} catch (Exception e) {
			//Console.WriteLine(e.ToString());
			Debug.Log("Connection error: " + e.Message);
		}

		return null;
	}
}
