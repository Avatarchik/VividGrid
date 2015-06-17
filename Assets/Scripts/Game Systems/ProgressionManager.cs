using UnityEngine;

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;


public class Level {
	public int LevelID;
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
	}


}

public class Zone {

	[XmlAttribute("name")]
	public string Name;

	[XmlArray("Levels"), XmlArrayItem("Level")]
	public Level[] levels;// = new List<Level>();

	public void DebugOutput () {
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

	// public void LoadLevel ( int levelID, bool isUnlocked, bool isComplete, bool isAced, int requiredMoves, int bestMoves ) {
	// 	var level = new Level();

	// 	level.LevelID = levelID;
	// 	level.IsUnlocked = isUnlocked;
	// 	level.IsComplete = isComplete;
	// 	level.IsAced = isAced;
	// 	level.RequiredMoves = requiredMoves;
	// 	level.BestMoves = bestMoves;

	// 	levels.Add(level);
	// }
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

	private ZoneContainer zones;
	private string nameOfCurrentZone = "Test Zone";

	// Use this for initialization
	void Start () {
		Debug.Log("Is Even Here");
		loadProgress();
	}

	private void loadProgress () {

		// check to see if file has been created
		if ( File.Exists(Path.Combine(Application.persistentDataPath, "Zones.xml")) ) {
			zones = ZoneContainer.Load(Path.Combine(Application.persistentDataPath, "Zones.xml"));
		} else { // if not, load it from the resources
			zones = ZoneContainer.LoadFromText(_originalGameData.text);
			// zones = ZoneContainer.Load(Path.Combine(Application.dataPath, "Data/Zones.xml"));
		}

		zones.DebugOutput();
	}

	public void ResetAllProgress () {

		zones = ZoneContainer.Load(Path.Combine(Application.dataPath, "Data/Zones.xml"));
		saveProgress();
	}

	private void saveProgress () {
		zones.Save(Path.Combine(Application.persistentDataPath, "Zones.xml"));
	}

	public void LevelCompleted ( int levelID ) {

		Debug.Log("Level " + levelID + " Completed");
		var zone = zones.GetZone(nameOfCurrentZone);
		if (zone != null) {
			var level = zone.GetLevel(levelID);
			if (level != null) {
				level.IsComplete = true;
				saveProgress();
			}
		}
	}

	public void LevelAced ( int levelID ) {

	}
}
