using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class PuzzleGridEditor : MonoBehaviour {

	public GameObject editorTurret_prefab;

	public int uniqueLevelID;
	public int minimumNumberOfMoves;

	private EditorTurret activeTurret;
	private SpawnTurretButton activeButton;

	public SpawnTurretButton[] allSpawners;

	private string dataPath;
	private string fileName;

	void Start () {
		var possibleLevel = GameObject.Find("Progression Manager").GetComponent<ProgressionManager>().selectedLevel;
		if (possibleLevel != -1) {
			uniqueLevelID = possibleLevel;
		}

		// try loading the level ID
		fileName = "l_" + uniqueLevelID + ".txt";
		dataPath = Application.dataPath + "/Resources/Levels/Testing/" + fileName;
		Load();
	}

	void Update () {

		// check if mouse was clicked
		if (Input.GetMouseButtonDown(0)) {

			// cast ray
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
													Vector2.zero);
			if (hit.collider != null) {
				if (hit.collider.CompareTag("Button")) {
					var b = hit.collider.gameObject.GetComponent<SpawnTurretButton>();
					SelectTurret(b);
					return;
				}
			}
			DeselectTurret();
		}

		if (activeTurret != null) {

			// set type 		// 12345 sets the type
			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				activeTurret.SetType(0);
			} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
				activeTurret.SetType(1);
			} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
				activeTurret.SetType(2);
			} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
				activeTurret.SetType(3);
			} else if (Input.GetKeyDown(KeyCode.Alpha5)) {
				activeTurret.SetType(4);
			} else if (Input.GetKeyDown(KeyCode.Alpha6)) {
				activeTurret.SetType(5);
			}

			// set rotation		// qwerty sets the rotation
			if (Input.GetKeyDown(KeyCode.Q)) {
				activeTurret.SetRotation(0);
			} else if (Input.GetKeyDown(KeyCode.W)) {
				activeTurret.SetRotation(1);
			} else if (Input.GetKeyDown(KeyCode.E)) {
				activeTurret.SetRotation(2);
			} else if (Input.GetKeyDown(KeyCode.R)) {
				activeTurret.SetRotation(3);
			}

			// set type		// asdfg sets number of heads
			if (Input.GetKeyDown(KeyCode.A)) {
				activeTurret.SetLayout(0);
			} else if (Input.GetKeyDown(KeyCode.S)) {
				activeTurret.SetLayout(1);
			} else if (Input.GetKeyDown(KeyCode.D)) {
				activeTurret.SetLayout(2);
			} else if (Input.GetKeyDown(KeyCode.F)) {
				activeTurret.SetLayout(3);
			} else if (Input.GetKeyDown(KeyCode.G)) {
				activeTurret.SetLayout(4);
			}

			if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) {
				DeleteTurret(activeButton);
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Save();
			GameObject.Find("Progression Manager").GetComponent<ProgressionManager>().selectedLevel = uniqueLevelID;
			GoToInGame();
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			Save();
		}
	}

	private void DeselectTurret () {
		if (activeButton != null) {
			activeButton.SetVisuallyInactive();
		}
		activeButton = null;
		activeTurret = null;
	}

	private void DeleteTurret ( SpawnTurretButton button ) {
		Destroy(button.turret.gameObject);
		button.turret = null;

		if (activeButton == button) {
			activeTurret = null;
			activeButton.SetVisuallyInactive();
			activeButton = null;
		}
	}

	private void SelectTurret ( SpawnTurretButton button ) {

		// change new button
		if ( activeButton == null ) {
			activeButton = button;
			activeButton.SetVisuallyActive();
		} else if ( activeButton != button ) {
			activeButton.SetVisuallyInactive();
			activeButton = button;
			activeButton.SetVisuallyActive();
		} else {
			return;
		}

		// if button doesnt have a turret, make one
		if ( activeButton.turret == null ) {

			var turret = CreateTurret(activeButton);
			activeTurret = activeButton.turret;

		} else {

			activeTurret = activeButton.turret;
		}
	}

	private EditorTurret CreateTurret ( SpawnTurretButton button ) {

		var turret = (GameObject)Instantiate(editorTurret_prefab);

		// set position
		var p = button.transform.position;
		turret.transform.position = new Vector3(p.x, p.y, 0);

		var editorTurret = turret.GetComponent<EditorTurret>();
		editorTurret.Startup();
		button.turret = editorTurret;
		return editorTurret;
	}

	private void GoToInGame () {
		Application.LoadLevel("LevelTestingPuzzleScene");
	}

	private void LoadTurret ( string command ) {

		// Debug.Log("Load turret");

		char[] delimiters = {','};
		string[] commands = command.Split(delimiters);

		int col = int.Parse(commands[0]);
		int row = int.Parse(commands[1]);
		var type = (Turret.Type) Enum.Parse(typeof(Turret.Type), commands[2]);
		var rotation = (Turret.Rotation) Enum.Parse(typeof(Turret.Rotation), commands[3]);
		var layout = (Turret.Layout) Enum.Parse(typeof(Turret.Layout), commands[4]);
		var direction = (Turret.Direction) Enum.Parse(typeof(Turret.Direction), commands[5]);

		foreach (SpawnTurretButton b in allSpawners) {

			if (b.col == col && b.row == row) {
				var turret = CreateTurret(b);

				if (rotation == Turret.Rotation.CW_90) {
					turret.SetType(0);
				} else if (rotation == Turret.Rotation.CCW_90) {
					turret.SetType(1);
				} else if (rotation == Turret.Rotation.CW_180) {
					turret.SetType(2);
				} else if (type == Turret.Type.Spawner) {
					turret.SetType(3);
				} else if (type == Turret.Type.Receiver) {
					turret.SetType(4);
				} else if (type == Turret.Type.Redirect && rotation == Turret.Rotation.Static) {
					turret.SetType(5);
				}

				switch (direction)
				{
					case Turret.Direction.Up:
						turret.SetRotation(0);
						break;
					case Turret.Direction.Right:
						turret.SetRotation(1);
						break;
					case Turret.Direction.Down:
						turret.SetRotation(2);
						break;
					case Turret.Direction.Left:
						turret.SetRotation(3);
						break;
				}

				switch (layout)
				{
					case Turret.Layout.Single:
						turret.SetLayout(0);
						break;
					case Turret.Layout.Double_90:
						turret.SetLayout(1);
						break;
					case Turret.Layout.Double_180:
						turret.SetLayout(2);
						break;
					case Turret.Layout.Triple:
						turret.SetLayout(3);
						break;
					case Turret.Layout.Receiver:
						turret.SetLayout(4);
						break;
				}
				break;
			}
		}
	}

	public void Save () {
		string levelData = "";

		List<string> turrets = new List<string>();

		foreach (SpawnTurretButton b in allSpawners) {
			if (b.turret != null) {
				turrets.Add(b.GetTurretString());
			}
		}

		// add the number of moves to the file
		turrets.Add(minimumNumberOfMoves.ToString());

		if (turrets.Count > 0) {
			levelData = string.Join(Environment.NewLine, turrets.ToArray());
			ConvertStringToTextAsset(levelData);
		} else {
			Debug.Log("No turrets to save");
		}
	}

	public void Load () {
		if ( File.Exists(dataPath) ) {

			// get ready to load file
			var turretCommands = new List<string>();

			// load level asset and convert to bytestream
			var levelInfo = Resources.Load<TextAsset>("Levels/Testing/l_" + uniqueLevelID);
			var byteStream = new MemoryStream(levelInfo.bytes);

			// load file into strings
			var reader = new StreamReader(byteStream);
	        try {
	            do {
	                turretCommands.Add(reader.ReadLine());
	            } while(reader.Peek() != -1);
	        } catch {
	            Debug.Log("Level File is empty");
	        } finally {
	            reader.Close();
	        }

	        // load the turret for the commands
	        for ( int i = 0; i < turretCommands.Count - 1; i++ ) {
	        	var s = turretCommands[i];
	        	LoadTurret(s);
	        }
	        
	        minimumNumberOfMoves = int.Parse(turretCommands[turretCommands.Count - 1]);
		}
	}

	TextAsset ConvertStringToTextAsset(string text) {
         // string temporaryTextFileName = "l_" + uniqueLevelID;
         File.WriteAllText(dataPath, text);
         AssetDatabase.SaveAssets();
         AssetDatabase.Refresh();
         TextAsset textAsset = Resources.Load(fileName) as TextAsset;
         return textAsset;
    }

}
