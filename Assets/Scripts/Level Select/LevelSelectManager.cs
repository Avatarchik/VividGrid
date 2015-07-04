using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectManager : MonoBehaviour {

	public ProgressionManager _progMan;
	public MusicManager _musMan;
	public DragSnapper _dragManager;

	[SerializeField] private LevelSelectLevelPack[] packs;

	private int currentZoneIndex;
	private string[] levelPackZoneNames;

	void Start () {
		
		_progMan = GameObject.Find("Progression Manager").GetComponent<ProgressionManager>();
		// _musMan = GameObject.Find("Music Manager").GetComponent<MusicManager>();

		LayoutLevelPacks();
		// MoveToLevelPack( "Default" );
	}

	public void LayoutLevelPacks () {

		// set up the layouts
		foreach (LevelSelectLevelPack l in packs) {
			l.LayoutZones(_progMan);
		}

		// get info from prog manager
		levelPackZoneNames = _progMan.GetZoneNames();
		currentZoneIndex = getIndexForZoneName(_progMan.CurrentZone.Name);

		// set up drag manager
		_dragManager.itemCount = levelPackZoneNames.Length + 1;
		_dragManager.SetZoneFocus(currentZoneIndex + 1);
	}

	public void MoveToLevelPack ( string levelName ) {

		currentZoneIndex = 0;
		_progMan.SetCurrentLevelPack( levelName );
		levelPackZoneNames = _progMan.GetZoneNames();

		// set the drag manager to new length
		_dragManager.itemCount = levelPackZoneNames.Length + 1;
	}

	public bool MoveToZone ( int index ) {

		if (index >= 1 && index <= levelPackZoneNames.Length) {
			currentZoneIndex = index - 1;
			_progMan.SetCurrentZone(levelPackZoneNames[currentZoneIndex]);
			movedToNewZone();
			return true;
		} else if ( index == 0 ) {
			Debug.Log("Moved to logo screen.");
			return true;
		} else {
			Debug.Log("Tried to move to invalid zone.");
			return false;
		}
	}

	public void LevelSelected ( int levelID ) {
		_progMan.selectedLevel = levelID;
		moveToLevel();
	}

	public void LevelUnselected ( int levelID ) {

	}

	private void movedToNewZone() {
		Debug.Log("Moved to new zone named " + _progMan.CurrentZone.Name);
	}

	private void moveToLevel () {
		Application.LoadLevel("PuzzleScene");
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
