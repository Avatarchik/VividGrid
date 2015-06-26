using UnityEngine;
using System.Collections;

public class LevelSelectLevelPack : MonoBehaviour {

	public string levelPackName;
	public LevelSelectZone[] zones;

	public void LayoutZones ( ProgressionManager p ) {
		foreach (LevelSelectZone zoneLayout in zones) {
			var zoneData = p.GetLevelPack(levelPackName).GetZone(zoneLayout.zoneName);
			zoneLayout.LayoutIcons(zoneData);
		}
	}
}
