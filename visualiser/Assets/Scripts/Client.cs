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
				input.RemoveAt(input.Count - 1);
			} catch (Exception e) {
				Debug.Log(e.Message);
			}
			List<string> solution;
			solution = (List<string>)bf.Deserialize(ns);
			if (solution[0].Equals("Error")) {
				Debug.Log(solution[1]);
				// TODO: Maybe raise an exception... Thread stuff?
			}

			client.Close();
			return solution;
		} catch (Exception e) {
			// TODO: Exception?
			Debug.Log("Connection error: " + e.Message);
		}
		return null;
	}
}
