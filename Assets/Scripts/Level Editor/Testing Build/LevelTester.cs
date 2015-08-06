using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelTester : MonoBehaviour {

	LevelTestingProgressionManager _ltProgMan;

	public GameObject levelCompleteScreen;
	public Text description;
	public Text targetMoves;
	public Text yourMoves;



	private void Start() {

		_ltProgMan = GameObject.Find("LTProgressionManager").GetComponent<LevelTestingProgressionManager>();
	}

	private void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel("Level Editor");
		}
	}



	public void SubmitResults ( int rating ) {

		var playerName = WWW.EscapeURL(PlayerPrefs.GetString("kPlayerName", ""));
		var levelID = _ltProgMan.CurrentID;
		var difficulty = rating;

		Debug.Log(playerName + " " + levelID + " " + " " + difficulty);

		Application.OpenURL("http://docs.google.com/forms/d/1OsTaJ18SEa1dVCbi5H4iTS3oLZBsz_ZDJnmsWM7FajM/viewform?entry.1853222905=" + levelID.ToString() + "&entry.36245525=" + difficulty.ToString() + "&entry.623193524=" + playerName); // &entry.428203114=Yes
		Debug.Log("fuck me");
	}

	public void NextLevel () {

		_ltProgMan.NextLevel();
	}

	public void LevelComplete (int target, int actual) {

		description.text = "ID: " + _ltProgMan.CurrentID;
		targetMoves.text = "Target Moves: " + target;
		yourMoves.text = "Your Moves: " + actual;
		levelCompleteScreen.SetActive(true);
	}

	public void RetryLevel () {

		levelCompleteScreen.SetActive(false);
		GameObject.Find("Grid Manager").GetComponent<PuzzleGrid>().ResetGrid();
	}

	public void SkipLevel() {

		Application.LoadLevel("LevelTestingSkip");
	}
}
