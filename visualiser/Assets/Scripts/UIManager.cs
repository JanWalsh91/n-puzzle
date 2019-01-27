using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UIManager : MonoBehaviour {

	public Dropdown sizeDropdown;
	public Dropdown heuristicFunction;
	public Dropdown solutionType;
	public InputField hostInputField;
	public InputField portInputField;
	public RectTransform errorPanel;
	public Text errorMessage;
	public GameManager gameManager;

	private Client client;
	private Animator animator;
	private bool rebuild = true;


	void Start() {
		gameManager = FindObjectOfType<GameManager>();

		client = FindObjectOfType<Client>();

		List<string> options = new List<string>();
		for (int i = 3; i < 8; i++) {
			options.Add(i.ToString() + " x " + i.ToString());
		}
		sizeDropdown.AddOptions(options);

		hostInputField.text = client.hostName;
		portInputField.text = client.portNum.ToString();

		hostInputField.onValueChanged.AddListener(delegate { OnHostChange(); });
		portInputField.onValueChanged.AddListener(delegate { OnPortChange(); });

		animator = errorPanel.GetComponent<Animator>();
	}

	public void UpdateValue(int N) {
		sizeDropdown.value = N - 3;
	}

	public void OnSizeChange() {
		gameManager.boardManager.N = sizeDropdown.value + 3;
		if (rebuild) {
			gameManager.boardManager.BuildReversedBoard(null);
		}
	}

	public void OnHostChange() {
		client.hostName = hostInputField.text;
	}

	public void OnPortChange() {
		client.portNum = int.Parse(portInputField.text);
	}

	public void OnHeuristicFunctionChange() {
		gameManager.heuristicFunction = heuristicFunction.value;
	}

	public void OnSolutionTypeChange() {
		gameManager.solutionType = solutionType.value;
	}

	public void DisplayError(string message) {
		errorMessage.text = "Error: " + message;
		animator.SetTrigger("Display");
	}

	public void OpenFile() {
		string fileName = EditorUtility.OpenFilePanel("Open n-puzzle file", ".", "np");
		if (fileName == null || fileName.Length == 0) {
			return;
		}
		List<List<int>> input = null;
		try {
			input = gameManager.parser.SolveFromFile(fileName);
			//gameManager.boardManager.values = input;
		} catch (ParserException pe) {
			DisplayError(pe.Message);
			return;
		}

		//foreach (var item in input) {
		//	Debug.Log(System.String.Join(" - ", item));
		//}

		if (input.Count > 7) {
			DisplayError("Unsupported size");
			return;
		}
		rebuild = false;
		sizeDropdown.value = input.Count - 3;
		rebuild = true;
		gameManager.boardManager.N = input.Count;
		//Debug.Log("New N: " + gameManager.boardManager.N);
		gameManager.boardManager.BuildReversedBoard(input);
	}
}
