using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelTester : MonoBehaviour {

	LevelTestingProgressionManager _ltProgMan;

	public GameObject levelCompleteScreen;
	public Text description;
	public Text targetMoves;
	public Text yourMoves;

	void Start() {
		_ltProgMan = GameObject.Find("LTProgressionManager").GetComponent<LevelTestingProgressionManager>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel("Level Editor");
		}
	}

	public void NextLevel() {
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
		_ltProgMan.NextLevel();
	}
}
