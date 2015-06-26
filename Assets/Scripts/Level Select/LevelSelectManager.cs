using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectManager : MonoBehaviour {

	public ProgressionManager _progMan;
	public MusicManager _musMan;

	[SerializeField] private LevelSelectLevelPack[] packs;

	private int currentZoneIndex;
	private string[] levelPackZoneNames;

	void Start () {
		
		_progMan = GameObject.Find("Progression Manager").GetComponent<ProgressionManager>();
		_musMan = GameObject.Find("Music Manager").GetComponent<MusicManager>();

		LoadLevelPack( "Default" );
	}

	public void LoadLevelPack ( string levelName ) {

		currentZoneIndex = 0;
		_progMan.SetCurrentLevelPack( levelName );
		levelPackZoneNames = _progMan.GetZoneNames();

		foreach (LevelSelectLevelPack l in packs) {
			if (l.levelPackName == levelName) {
				l.LayoutZones(_progMan);
			}
		}
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
			_progMan.SetCurrentZone(levelPackZoneNames[currentZoneIndex]);
		}
	}

	public void LevelSelected ( int levelID ) {
		_progMan.selectedLevel = levelID;
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

	public void DebugResetGame () {
		
	}
}
