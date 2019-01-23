using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour {

	public Dropdown sizeDropdown;
	public InputField hostInputField;
	public InputField portInputField;

	public GameManager gameManager;

	private Client client;


	void Start() {
		gameManager = GameObject.FindObjectOfType<GameManager>();

		client = GameObject.FindObjectOfType<Client>();

		List<string> options = new List<string>();
		for (int i = 3; i < 8; i++) {
			options.Add(i.ToString() + " x " + i.ToString());
		}
		sizeDropdown.AddOptions(options);

		hostInputField.text = client.hostName;
		portInputField.text = client.portNum.ToString();

		hostInputField.onValueChanged.AddListener(delegate { OnHostChange(); });
		portInputField.onValueChanged.AddListener(delegate { OnPortChange(); });
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
		Debug.Log(sizeDropdown.value);

		gameManager.boardManager.N = sizeDropdown.value + 3;
		gameManager.boardManager.BuildBoard(null);
	}

	public void OnHostChange() {
		client.hostName = hostInputField.text;
	}

	public void OnPortChange() {
		client.portNum = int.Parse(portInputField.text);
	}
}
