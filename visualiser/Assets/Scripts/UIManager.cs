using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

	void OnEnable() {
		GameManager.OnLoadFile += UpdateValue;
	}

	void OnDisable() {
		GameManager.OnLoadFile -= UpdateValue;
	}

	public void UpdateValue(int N) {
		sizeDropdown.value = N - 3;
	}

	public void OnSizeChange() {
		gameManager.boardManager.N = sizeDropdown.value + 3;
		gameManager.boardManager.BuildBoard(null);
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
}
