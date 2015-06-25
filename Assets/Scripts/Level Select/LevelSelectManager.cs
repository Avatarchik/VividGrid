using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectManager : MonoBehaviour {

	public ProgressionManager _progressionManager;
	public MusicManager _musicManager;

	private int currentZoneIndex;
	private string[] levelPackZoneNames;

	void Start () {
		
		_progressionManager = GameObject.Find("Progression Manager").GetComponent<ProgressionManager>();
		_musicManager = GameObject.Find("Music Manager").GetComponent<MusicManager>();

		LoadLevelPack( "Default" );
	}

	public void LoadLevelPack ( string levelName ) {

		currentZoneIndex = 0;
		_progressionManager.SetCurrentLevelPack( levelName );
		levelPackZoneNames = _progressionManager.GetZoneNames();
	}

	public bool NextZoneExistsFor ( string zoneName ) {

		int index = getIndexForZoneName(zoneName);
		if (index == -1) {
			Debug.Log("Zone " + zoneName + " does not exist.");
			return false;
		}
		return ( (index + 1) < levelPackZoneNames.Length );
	}

	public void MoveToNextZone () {

		int test = currentZoneIndex + 1;
		if (test >= levelPackZoneNames.Length) {
			// we're at last zone
			// tell the visuals to fuck off
		} else {
			currentZoneIndex = test;
			_progressionManager.SetCurrentZone(levelPackZoneNames[currentZoneIndex]);
		}
	}

	public void LevelSelected ( int levelID ) {
		_progressionManager.selectedLevel = levelID;
		moveToLevel();
	}

	public void LevelUnselected ( int levelID ) {

	}

	private void moveToLevel () {
		Application.LoadLevel(1);
	}

	private int getIndexForZoneName( string zoneName ) {

		for (int i = 0; i < levelPackZoneNames.Length; i++) {
			if (levelPackZoneNames[i] == zoneName) {
				return i;
			}
		}
		return -1;
	}
}
