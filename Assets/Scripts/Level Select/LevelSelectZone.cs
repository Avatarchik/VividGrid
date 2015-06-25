using UnityEngine;
using System.Collections;

public class LevelSelectZone : MonoBehaviour {

	public string zoneName;
	public LevelSelectManager levelSelectManager;

	public void LevelWasSelected ( int levelID ) {
		levelSelectManager.LevelSelected(levelID);
	}
}
