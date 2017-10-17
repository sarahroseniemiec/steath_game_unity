 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
	public GameObject gameWinUI;
	public GameObject gameLoseUI;
	bool gameIsOver;

	// Use this for initialization
	void Start () {
		Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
		FindObjectOfType<Player>().OnGameWin += ShowGameWinUI;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameIsOver) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				SceneManager.LoadScene (0);
				gameIsOver = false;
			}
		}
		
	}

	void ShowGameWinUI() {
		OnGameOver (gameWinUI);
	}

	void ShowGameLoseUI() {
		OnGameOver (gameLoseUI);
	}

	void OnGameOver(GameObject gameOverUI) {
		gameOverUI.SetActive (true);
		gameIsOver = true;
		Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
		FindObjectOfType<Player>().OnGameWin -= ShowGameWinUI;
		
	}

}
