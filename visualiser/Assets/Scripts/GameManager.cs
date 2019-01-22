using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class GameManager : MonoBehaviour {

	public BoardManager boardManager;


	private Parser parser;
	private Client client;

	void Start() {
		parser = new Parser();
		client = new Client();
	}

	void Update() {
	}

	public void OpenFile() {
		string fileName = EditorUtility.OpenFilePanel("Open n-puzzle file", ".", null);

		List<List<int>> input = parser.SolveFromFile(fileName);
		boardManager.BuildBoard(input);
		client.CallServer(input);
	}
}
