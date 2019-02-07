using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Client: MonoBehaviour {

	public int portNum = 2000;
	public string hostName = "localhost";

	public BoardManager boardManager;
	public GameManager gameManager;

	public string errorMessage;

	void Start() {
		errorMessage = null;
	}

	public List<string> CallServer(List<List<int>> input) {
		try {
			TcpClient client = new TcpClient(hostName, portNum);
			NetworkStream ns = client.GetStream();
			BinaryFormatter bf = new BinaryFormatter();

			input.Add(new List<int> { gameManager.algorithmType, gameManager.solutionType, gameManager.heuristicFunction, gameManager.isGreedy, gameManager.timeOut });

			try {
				bf.Serialize(ns, input);
				input.RemoveAt(input.Count - 1);
			} catch (Exception e) {
				Debug.Log(e.Message);
			}

			List<string> solution;
			solution = (List<string>)bf.Deserialize(ns);
			if (solution != null && solution.Count > 0 && solution[0].Equals("Error")) {
				errorMessage = solution[1];
			}
			client.Close();
			return solution;
		} catch (IOException) {
			errorMessage = "Time out";
		} catch (Exception e) {
			errorMessage = e.Message;
		}
		return null;
	}
}
