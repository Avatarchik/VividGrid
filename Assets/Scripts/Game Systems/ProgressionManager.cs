using UnityEngine;
using System.Collections.Generic;
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
public class LevelPack {

	public string DefaultZoneName;

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
        var serializer = new XmlSerializer(typeof(LevelPack));
        using(var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }
   
    public static LevelPack Load(string path)
    {
        var serializer = new XmlSerializer(typeof(LevelPack));
        using(var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as LevelPack;
        }
    }

    public static LevelPack LoadFromText(string text) 
 	{
 		var serializer = new XmlSerializer(typeof(LevelPack));
 		return serializer.Deserialize(new StringReader(text)) as LevelPack;
 	}
}

public class ProgressionManager : MonoBehaviour {

	// ************ Singleton Logic ***************
	public static ProgressionManager Instance;
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

	[SerializeField] private string[] allLevelPackNames;

	private string currentLevelPackName = "";
	private const string saveDataSuffix = "_LevelPackZoneSaveData.xml";
	private const string loadDataSuffix = "_LevelPackZoneLoadData";

	private Dictionary<string,LevelPack> levelPackData = new Dictionary<string, LevelPack>();
	private string nameOfCurrentZone = "";

	private bool shouldBeSavedFlag;
	private bool statsShouldBeUpdatedFlag;

	public int selectedLevel = -1;

	public string CurrentLevelPackName {
		get {
			return currentLevelPackName;
		}
	}
	public LevelPack CurrentLevelPack {
		get {
			return getCurrentPack();
		}
	}
	public Zone CurrentZone {
		get {
			return getCurrentPack().GetZone(nameOfCurrentZone);
		}
	}
	public Level CurrentLevel {
		get {
			return getLevel(selectedLevel);
		}
	}


			// test garbage
			public void LevelAcedTest() {

				LevelAced(1, 2);
			}
			public void LevelCompletedTest() {

				LevelCompleted(1, 3);
			}
			public static void GoToMusic () {

				Application.LoadLevel(0);
			}


	// Use this for initialization
	public void Initialize () {

		// set initial level pack
		SetCurrentLevelPack(allLevelPackNames[0]);

		loadProgress();
	}

	
	public void SetCurrentLevelPack ( string levelPackName ) {

		currentLevelPackName = levelPackName;
	}
	public void SetCurrentZone ( string zoneName ) {

		nameOfCurrentZone = zoneName;
	}
	public string[] GetZoneNames () {
		// Debug.Log("GetZoneNames() - CurrentLevelPackName: " + CurrentLevelPackName);
		var names = new string[CurrentLevelPack.zones.Length];
		for ( int i = 0; i < CurrentLevelPack.zones.Length; i++ ) {
			names[i] = CurrentLevelPack.zones[i].Name;
		}
		return names;
	}
	public LevelPack GetLevelPack ( string levelPackName ) {

		return levelPackData[levelPackName];
	}


	public void LevelCompleted ( int levelID, int numberOfMoves ) {

		// Debug.Log("Level " + levelID + " Completed");
		var level = getLevel(levelID);
		if (level != null) {
			setComplete(level);
			registerMoves(level, numberOfMoves);
		}
		commitChanges();
	}
	public void LevelAced ( int levelID, int numberOfMoves ) {

		// Debug.Log("Level " + levelID + " Aced");
		var level = getLevel(levelID);
		if (level != null) {
			setAced(level);
			registerMoves(level, numberOfMoves);
		}
		commitChanges();
	}
	public void ResetAllProgress () {

		foreach (string s in allLevelPackNames) {
			ResetProgress(s);
		}
	}
	public void ResetProgress ( string levelPackName ) {

		var originalData = Resources.Load<TextAsset>("Data/" + currentLevelPackName + loadDataSuffix);
		levelPackData[levelPackName] = LevelPack.LoadFromText(originalData.text);
		saveProgress();

		Debug.Log("Reset Progress for " + levelPackName);
	}


	// private functions
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


	private void setStatsShouldBeUpdatedFlag() {

		statsShouldBeUpdatedFlag = true;
	}
	private void setSaveFlag () {

		shouldBeSavedFlag = true;
	}
	private void commitChanges () {

		if (statsShouldBeUpdatedFlag) {
			getCurrentPack().GetZone(nameOfCurrentZone).UpdateStats();
		}
		if (shouldBeSavedFlag) {
			saveProgress();
		}
		statsShouldBeUpdatedFlag = false;
		shouldBeSavedFlag = false;
	}


	private LevelPack getCurrentPack () {

		return levelPackData[currentLevelPackName];
	}
	private Level getLevel ( int levelID ) {

		var zone = getCurrentPack().GetZone(nameOfCurrentZone);
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

		foreach (string s in allLevelPackNames) {
			loadPack(s);
		}

		// select the default zone
		nameOfCurrentZone = CurrentLevelPack.DefaultZoneName;
	}
	private void loadPack ( string packName ) {

		var pPathSave = Path.Combine(Application.persistentDataPath, packName + saveDataSuffix);
		var packData = (File.Exists(pPathSave)) ? LevelPack.Load(pPathSave) :
				LevelPack.LoadFromText(Resources.Load<TextAsset>("Data/" + packName + loadDataSuffix).text);
		levelPackData.Add(packName, packData);

		// packData.DebugOutput();
	}
	private void saveProgress () {

		foreach (string s in allLevelPackNames) {
			levelPackData[s].Save(Path.Combine(Application.persistentDataPath, s + saveDataSuffix));
		}
	}
}
