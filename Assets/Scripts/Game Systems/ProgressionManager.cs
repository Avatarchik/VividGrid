using UnityEngine;

using System.Xml.Serialization;
using System.IO;

public class Level {
	public int LevelID;
	public int NextLevelID;
	public bool IsUnlocked;
	public bool IsComplete;
	public bool IsAced;
	public int RequiredMoves;
	public int BestMoves;

	public void DebugOutput () {
		Debug.Log("Level ID: " + LevelID);
		if (IsComplete) {
			Debug.Log("Is Complete");
		} else {
			Debug.Log("Is Not Complete");
		}

		if (IsAced) {
			Debug.Log("Is Aced");
		} else {
			Debug.Log("Is Not Aced");
		}

		if (IsUnlocked) {
			Debug.Log("Is Unlocked");
		} else {
			Debug.Log("Is Locked");
		}
	}
}

public class Zone {

	[XmlAttribute]
	public string Name;

	[XmlArray("Levels"), XmlArrayItem("Level")]
	public Level[] levels;

	// zone stats
	public int TotalLevels;
	public int CompletedLevels;
	public float CompletionPercentage;

	public void DebugOutput () {

		Debug.Log("Zone Stats");
		Debug.Log("Total Levels: " + TotalLevels);
		Debug.Log("Completed Levels: " + CompletedLevels);
		Debug.Log("Completion %: " + CompletionPercentage);

		// zoneStats.DebugOutput();
		foreach (Level l in levels) {
			l.DebugOutput();
		}
	}

	public Level GetLevel ( int levelID ) {
		foreach (Level l in levels) {
			if (l.LevelID == levelID) {
				return l;
			}
		}
		return null;
	}

	public void UpdateStats () {

		int totalLevels = levels.Length;
		int completedLevels = 0;
		int acedLevels = 0;
		foreach (Level l in levels) {
			completedLevels = (l.IsComplete) ? completedLevels + 1 : completedLevels;
			acedLevels = (l.IsAced) ? acedLevels + 1 : acedLevels;
		}
		float completionPercentage = (((float)completedLevels/(float)totalLevels) * 0.5f) +
									 (((float)acedLevels/(float)totalLevels) * 0.5f);

		// update stats
		TotalLevels = totalLevels;
		CompletedLevels = completedLevels;
		CompletionPercentage = completionPercentage;
	}
}

[XmlRoot("ZoneCollection")]
public class ZoneContainer {

	[XmlArray("Zones"), XmlArrayItem("Zone")]
	public Zone[] zones;

	public void DebugOutput () {
		foreach (Zone z in zones) {
			z.DebugOutput();
		}
	}

	public Zone GetZone ( string name ) {
		foreach ( Zone z in zones ) {
			if ( z.Name == name ) {
				return z;
			}
		}
		return null;
	}

	public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(ZoneContainer));
        using(var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }
   
    public static ZoneContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(ZoneContainer));
        using(var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as ZoneContainer;
        }
    }

    public static ZoneContainer LoadFromText(string text) 
 	{
 		var serializer = new XmlSerializer(typeof(ZoneContainer));
 		return serializer.Deserialize(new StringReader(text)) as ZoneContainer;
 	}
}

public class ProgressionManager : MonoBehaviour {

	public TextAsset _originalGameData;

	private string levelPackName = "DefaultLevelPack";

	private ZoneContainer zones;
	private string nameOfCurrentZone = "Test Zone";

	private bool shouldBeSavedFlag;
	private bool statsShouldBeUpdatedFlag;

	// Use this for initialization
	void Start () {

		loadProgress();
	}

	public void LevelCompleted ( int levelID, int numberOfMoves ) {

		Debug.Log("Level " + levelID + " Completed");
		var level = getLevel(levelID);
		if (level != null) {
			setComplete(level);
			registerMoves(level, numberOfMoves);
		}
		commitChanges();
	}

	public void LevelAced ( int levelID, int numberOfMoves ) {

		Debug.Log("Level " + levelID + " Aced");
		var level = getLevel(levelID);
		if (level != null) {
			setAced(level);
			registerMoves(level, numberOfMoves);
		}
		commitChanges();
	}

	public void ResetAllProgress () {

		zones = ZoneContainer.LoadFromText(_originalGameData.text);
		saveProgress();
	}

	private void setComplete ( Level level ) {

		if ( !level.IsComplete ) {
			level.IsComplete = true;
			unlockNextLevel( level );
			setSaveFlag();
			setStatsShouldBeUpdatedFlag();
		}
	}

	private void setAced ( Level level ) {

		setComplete(level);
		if ( !level.IsAced ) {
			level.IsAced = true;
			setSaveFlag();
			setStatsShouldBeUpdatedFlag();
		}
	}

	private void registerMoves ( Level level, int numberOfMoves ) {

		if ( numberOfMoves < level.BestMoves || level.BestMoves == -1 ) {
			level.BestMoves = numberOfMoves;
			setSaveFlag();
		}
	}

	private void setStatsShouldBeUpdatedFlag() {

		statsShouldBeUpdatedFlag = true;
	}

	private void setSaveFlag () {

		shouldBeSavedFlag = true;
	}

	private void commitChanges () {

		if (statsShouldBeUpdatedFlag) {
			zones.GetZone(nameOfCurrentZone).UpdateStats();
		}
		if (shouldBeSavedFlag) {
			saveProgress();
		}
		statsShouldBeUpdatedFlag = false;
		shouldBeSavedFlag = false;
	}

	private void unlockNextLevel ( Level thisLevel ) {

		var nextID = thisLevel.NextLevelID;
		if ( nextID != -1 ) {
			var nextLevel = getLevel(nextID);
			if ( !nextLevel.IsUnlocked ) {
				nextLevel.IsUnlocked = true;
				setSaveFlag();
			}
		}
	}

	private Level getLevel ( int levelID ) {

		var zone = zones.GetZone(nameOfCurrentZone);
		if (zone != null) {
			var level = zone.GetLevel(levelID);
			if (level != null) {
				return level;
			}
			Debug.Log("Tried to access invalid levelID");
		} else {
			Debug.Log("Tried to access invalid zoneName");
		}
		return null;
	}

	private void loadProgress () {

		var pPath = Path.Combine(Application.persistentDataPath, levelPackName + ".xml");
		zones = (File.Exists(pPath)) ? ZoneContainer.Load(pPath) : ZoneContainer.LoadFromText(_originalGameData.text);

		zones.DebugOutput();
	}

	private void saveProgress () {

		zones.Save(Path.Combine(Application.persistentDataPath, levelPackName + ".xml"));
	}
}
