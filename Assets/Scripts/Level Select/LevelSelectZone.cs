using UnityEngine;
using System.Collections;

public class LevelSelectZone : MonoBehaviour {

	public string zoneName;
	public LevelSelectManager levelSelectManager;

	public void LayoutIcons ( Zone z ) {
		foreach (LevelSelectIcon i in GetComponentsInChildren<LevelSelectIcon>()) {
			var level = z.GetLevel(i.levelID_ref);
			i.SetState(level.IsUnlocked, level.IsComplete, level.IsAced);
		}
	}

	public void LevelWasSelected ( int levelID ) {
		levelSelectManager.LevelSelected(levelID);
	}
}
