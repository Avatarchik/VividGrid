using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LevelTestingProgressionManager : MonoBehaviour {

	// ************ Singleton Logic ***************
	public static LevelTestingProgressionManager Instance;
	void Awake() {
		if (Instance) {
			DestroyImmediate(gameObject);
		} else {
			DontDestroyOnLoad(gameObject);
			Instance = this;
			Initialize();
		}
	}
	// ********************************************

	private Stack<int> playOrderGUIDs = new Stack<int>();
	private ProgressionManager _progManager;

	public int CurrentID {
		get {
			return _progManager.selectedLevel;
		}
	}

	// Use this for initialization
	void Initialize () {

		_progManager = GameObject.Find("Progression Manager").GetComponent<ProgressionManager>();

		// get all the GUIDs
		var allLevelGUIDs = new List<int>();
		var info = new DirectoryInfo(Application.dataPath + "/Resources/Levels/Testing/");
		var files = info.GetFiles("*.txt");
		foreach (FileInfo file in files) {
			var fullName = file.Name;
			var noFileExtension = fullName.Substring(0, fullName.Length - 4);
			var noPrefix = noFileExtension.Substring(2);
			var intOnly = int.Parse(noPrefix);
			allLevelGUIDs.Add(intOnly);
		}

		// create a random play order
		while ( allLevelGUIDs.Count > 0 ) {
			var rand = Random.Range(0, allLevelGUIDs.Count);
			var num = allLevelGUIDs[rand];
			allLevelGUIDs.RemoveAt(rand);
			playOrderGUIDs.Push(num);
		}

	}



	public void NextLevel () {
		if (playOrderGUIDs.Count > 0) {
			_progManager.selectedLevel = playOrderGUIDs.Pop();
			Application.LoadLevel("LevelTestingPuzzleScene");
		} else {
			Debug.Log("No more levels!");
		}
	}
}
