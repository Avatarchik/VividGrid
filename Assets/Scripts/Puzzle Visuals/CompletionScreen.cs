using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CompletionScreen : MonoBehaviour {

	public PuzzleGrid grid;
	public Text movesCounter;

	private CanvasGroup canvasGroup;

	void Start () {
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Present () {
		StartCoroutine(fadeIn());
		movesCounter.text = "in " + grid.Moves.NumberOfMoves + " moves";
	}

	public void Dismiss () {
		StartCoroutine(fadeOut());
	}

	public void Retry () {
		grid.ResetGrid();
		Dismiss();
	}

	public void ReturnMainMenu () {
		Application.LoadLevel(0);
	}
	
	IEnumerator fadeOut () {
		while (canvasGroup.alpha > 0.0f) {
			canvasGroup.alpha -= Time.deltaTime;
			yield return null;
		}
		canvasGroup.alpha = 0.0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	IEnumerator fadeIn () {
		while (canvasGroup.alpha < 1.0f) {
			canvasGroup.alpha += Time.deltaTime;
			yield return null;
		}
		canvasGroup.alpha = 1.0f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}
}
