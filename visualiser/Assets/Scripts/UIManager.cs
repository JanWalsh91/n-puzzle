using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Runtime.InteropServices;
using Crosstales.FB;

public class UIManager : MonoBehaviour {

	public Slider speed;
	public Dropdown algorithmType;
	public Dropdown heuristicFunction;
	public Dropdown solutionType;
	public Dropdown sizeDropdown;
	public Toggle greedySearch;

	public InputField timeout;
	public InputField hostInputField;
	public InputField portInputField;
	public RectTransform errorPanel;
	public Text errorMessage;
	public GameManager gameManager;

	private Client client;
	private Animator animator;
	private bool rebuild = true;
	private Text greedySearchText;

	//[DllImport("user32.dll")]
	//private static extern void OpenFileDialog();

	void Start() {
		gameManager = FindObjectOfType<GameManager>();
		client = FindObjectOfType<Client>();
		greedySearchText = greedySearch.GetComponentInChildren<Text>();

		List<string> options = new List<string>();
		for (int i = 3; i < 7; i++) {
			options.Add(i.ToString() + " x " + i.ToString());
		}
		sizeDropdown.AddOptions(options);

		timeout.text = gameManager.timeOut.ToString();
		hostInputField.text = client.hostName;
		portInputField.text = client.portNum.ToString();

		timeout.onValueChanged.AddListener(delegate { OnTimeOutChange(); });
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

	public void OnAlgorithmTypeChange() {
		gameManager.algorithmType = algorithmType.value;
		greedySearch.interactable = algorithmType.value == 0;
		greedySearchText.color = new Color(greedySearchText.color.r, greedySearchText.color.g, greedySearchText.color.b, algorithmType.value == 0 ? 1f : 0.2f);
	}

	public void OnHeuristicFunctionChange() {
		gameManager.heuristicFunction = heuristicFunction.value;
	}

	public void OnSolutionTypeChange() {
		gameManager.solutionType = solutionType.value;
	}

	public void OnGreedySearchChange() {
		gameManager.isGreedy = System.Convert.ToInt32(greedySearch.isOn);
	}

	public void DisplayError(string message) {
		errorMessage.text = "Error: " + message;
		animator.SetTrigger("Display");
	}

	public void OnSpeedChange() {
		gameManager.boardManager.movingSpeed = 0.1f / speed.value;
	}

	public void OnTimeOutChange() {
		gameManager.timeOut = System.Convert.ToInt32(timeout.text);
	}

	public void OpenFile() {

		//string fileName = EditorUtility.OpenFilePanel("Open n-puzzle file", ".", "np");

		//System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

		//ofd.InitialDirectory = ".";
		//ofd.Filter = "NPuzzple files (*.np)|*.np";
		//ofd.FilterIndex = 2;
		//ofd.RestoreDirectory = true;

		//string fileName = null;

		//if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
		//	fileName = ofd.FileName;
		//}

		string ext = "np";
		string fileName = FileBrowser.OpenSingleFile("Open NPuzzle File", "", ext);

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
