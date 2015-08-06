using UnityEngine;
using UnityEngine.UI;

public class LevelTestingDataScreen : MonoBehaviour {

	public Text placeholder;
	public Text editField;
	public Text submitButtonText;

	private LevelTestingProgressionManager _ltProgMan;

	private const string NAME_KEY = "kPlayerName";
	private string _name;

	// Use this for initialization
	void Start () {

		_ltProgMan = GameObject.Find("LTProgressionManager").GetComponent<LevelTestingProgressionManager>();

		// try to get an existing name
		_name = PlayerPrefs.GetString(NAME_KEY, "");
		if (_name != "") {
			nameExists();
		} else {
			nameDoesNotExist();
		}
	}

	void nameExists () {

		editField.text = _name;
		placeholder.text = _name;
		submitButtonText.text = "Update";
	}

	void nameDoesNotExist () {

		placeholder.text = "Enter name...";
	}

	public void Finished () {

		_name = editField.text;
	}

	public void Submit () {

		PlayerPrefs.SetString(NAME_KEY, _name);
		_ltProgMan.NextLevel();
	}

	public void Anonymous () {

		_name = "";
		placeholder.text = "";
		editField.text = "";

		Submit();
	}
}
