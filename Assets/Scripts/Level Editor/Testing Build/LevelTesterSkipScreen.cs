using UnityEngine;
using System.Collections;

public class LevelTesterSkipScreen : MonoBehaviour {

	private LevelTestingProgressionManager _ltProgMan;

	void Start () {
	
		_ltProgMan = GameObject.Find("LTProgressionManager").GetComponent<LevelTestingProgressionManager>();
	}
	
	public void AlreadyPlayed () {

		_ltProgMan.NextLevel();
	}

	public void TooHard () {

		var playerName = WWW.EscapeURL(PlayerPrefs.GetString("kPlayerName", ""));
		var levelID = _ltProgMan.CurrentID;

		Application.OpenURL("http://docs.google.com/forms/d/1OsTaJ18SEa1dVCbi5H4iTS3oLZBsz_ZDJnmsWM7FajM/viewform?entry.1853222905=" + levelID.ToString() + "&entry.428203114=Yes&entry.36245525=10&entry.623193524=" + playerName); 
		
		_ltProgMan.NextLevel();
	}

}
