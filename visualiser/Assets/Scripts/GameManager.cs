﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Linq;

public class GameManager : MonoBehaviour {

	public BoardManager boardManager;
	public Client client;
	public Text nbStep;
	public UIManager uiManager;

	private Parser parser;
	private List<string> solution;

	private LinkedList<BoardManager.MoveDirection> solutionNextMoves;
	private LinkedList<BoardManager.MoveDirection> solutionPrevMoves;

	private Dictionary<string, BoardManager.MoveDirection> stringToMoveDirection;
	private Dictionary<BoardManager.MoveDirection, BoardManager.MoveDirection> oppositeMoveDirection;

	private bool needToUpdateNbStep = false;

	public delegate void OnLoadFileAction(int N);
	public static event OnLoadFileAction OnLoadFile; 

	void Start() {
		parser = new Parser();
		solution = null;

		solutionNextMoves = new LinkedList<BoardManager.MoveDirection>();
		solutionPrevMoves = new LinkedList<BoardManager.MoveDirection>();

		stringToMoveDirection = new Dictionary<string, BoardManager.MoveDirection>();
		stringToMoveDirection.Add("Right", BoardManager.MoveDirection.Right);
		stringToMoveDirection.Add("Left", BoardManager.MoveDirection.Left);
		stringToMoveDirection.Add("Up", BoardManager.MoveDirection.Up);
		stringToMoveDirection.Add("Down", BoardManager.MoveDirection.Down);

		oppositeMoveDirection = new Dictionary<BoardManager.MoveDirection, BoardManager.MoveDirection>();
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Right, BoardManager.MoveDirection.Left);
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Left, BoardManager.MoveDirection.Right);
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Up, BoardManager.MoveDirection.Down);
		oppositeMoveDirection.Add(BoardManager.MoveDirection.Down, BoardManager.MoveDirection.Up);
	}

	void Update() {
		if (needToUpdateNbStep) {
			needToUpdateNbStep = false;
			nbStep.text = solutionNextMoves.Count.ToString();
		}

		if (client.errorMessage != null) {
			uiManager.DisplayError(client.errorMessage);
			client.errorMessage = null;
		}

		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			boardManager.AddMovements(BoardManager.MoveDirection.Right);
			solutionNextMoves.AddLast(BoardManager.MoveDirection.Left);
			needToUpdateNbStep = true;
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			boardManager.AddMovements(BoardManager.MoveDirection.Left);
			solutionNextMoves.AddLast(BoardManager.MoveDirection.Right);
			needToUpdateNbStep = true;
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			boardManager.AddMovements(BoardManager.MoveDirection.Up);
			solutionNextMoves.AddLast(BoardManager.MoveDirection.Down);
			needToUpdateNbStep = true;
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			boardManager.AddMovements(BoardManager.MoveDirection.Down);
			solutionNextMoves.AddLast(BoardManager.MoveDirection.Up);
			needToUpdateNbStep = true;
		}
	}
	
	public void NextStep() {
		if (solutionNextMoves.Count > 0) {
			BoardManager.MoveDirection nextDirection = solutionNextMoves.Last.Value;
			solutionNextMoves.RemoveLast();
			boardManager.AddMovements(nextDirection);
			solutionPrevMoves.AddLast(oppositeMoveDirection[nextDirection]);
			nbStep.text = solutionNextMoves.Count.ToString();
		}
	}

	public void PrevStep() {
		if (solutionPrevMoves.Count > 0) {
			BoardManager.MoveDirection nextDirection = solutionPrevMoves.Last.Value;
			solutionPrevMoves.RemoveLast();
			boardManager.AddMovements(nextDirection);
			solutionNextMoves.AddLast(oppositeMoveDirection[nextDirection]);
			nbStep.text = solutionNextMoves.Count.ToString();
		}
	}

	public void Play() {
		while (solutionNextMoves.Count > 0) {
			NextStep();
		}
		// Create a event in the board manager to update the step to solution counter
	}

	private void Solve(List<List<int>> input) {

		Thread serverCommunicationThread = new Thread(new ThreadStart(() => {
			solution = client.CallServer(input);

			if (solution == null) {
				return;
			}

			for (int i = 0; i < solution.Count; i++) {
				solutionNextMoves.AddFirst(stringToMoveDirection[solution[i]]);
			}
			needToUpdateNbStep = true;
		}));
		serverCommunicationThread.IsBackground = true;
		serverCommunicationThread.Start();
	}

	public void SolveBoard() {
		Solve(boardManager.values);
	}

	public void OpenFile() {
		string fileName = EditorUtility.OpenFilePanel("Open n-puzzle file", ".", null);
		List<List<int>> input = parser.SolveFromFile(fileName);
		// TODO: handle unsupported size
		boardManager.N = input.Count;
		boardManager.BuildBoard(input);
		OnLoadFile(input.Count);
	}
}
