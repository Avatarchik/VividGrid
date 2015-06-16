using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class GridLine {

	public enum Type {
		Column = 0,
		Row
	}

	// private int _index;
	// private Type _type;

	private readonly List<int> _turretIDs = new List<int>();

	public void SetUpLine(int i, Type type) {
		// _index = i;
		// _type = type;
	}

	public void AddNewTurret (int turretID) {
		_turretIDs.Add(turretID);
	}

	public int[] GetTurretIDs () {
		return _turretIDs.ToArray();
	}
}

[RequireComponent(typeof(VisualGrid))]
public class PuzzleGrid : MonoBehaviour {

	public int _numColumns = 7;
	public int _numRows = 13;

	public int _levelID;

	private bool _isRotating;

	private GridLine[] columns;
	private GridLine[] rows;
	private Turret[] turrets;

	private int[] spawnerTurretIDs;
	private int[] receiverTurretIDs;
	private List<int> resolvingTurretIDs;

	private VisualGrid _visual;


	// Use this for initialization
	public void Start () {

		// initialize grid lines
		columns = new GridLine[_numColumns];
		rows = new GridLine[_numRows];

		for (int i = 1; i < _numColumns+1; i++) {
			var col = new GridLine();
			col.SetUpLine(i, GridLine.Type.Column);
			columns[i-1] = col;
		}

		for (int i = 1; i < _numRows+1; i++) {
			var row = new GridLine();
			row.SetUpLine(i, GridLine.Type.Row);
			rows[i-1] = row;
		}

		// initialize properties
		resolvingTurretIDs = new List<int>();

		// set up the visual grid
		_visual = GetComponent<VisualGrid>();
		_visual.SetUpGrid();
		_visual.SetUpButtons();

		LoadLayout();
	}

	public void ActivateColumn ( int i ) {
		Debug.Log("Column Rotated at Index " + i);
		ActivateGridLine(GridLine.Type.Column, i);
	}

	public void ActivateRow ( int i ) {
		Debug.Log("Row Rotated at Index " + i);
		ActivateGridLine(GridLine.Type.Row, i);
	}

	public void ActivateGridLine ( GridLine.Type type, int index ) {

		// get turret IDs
		var turretIDs = new int[0];
		switch (type)
		{
			case GridLine.Type.Column:
				turretIDs = columns[index-1].GetTurretIDs();
				break;

			case GridLine.Type.Row:
				turretIDs = rows[index-1].GetTurretIDs();
				break;
		}

		// tell turrets to rotate
		foreach (int id in turretIDs) {
			resolvingTurretIDs.Add(id);
			turrets[id].Rotate();
		}

		_isRotating = true;
		_visual.DisableRotation();
	}

	public void ResolveRotation ( int id ) {

		// remove 
		if ( !resolvingTurretIDs.Remove( id ) ) {
			Debug.Log("Tried to resolve rotation that doesn't exist!");
		}

		// if all rotations are resolved update
		if ( resolvingTurretIDs.Count == 0 ) {

			Debug.Log("Resolving rotation...");

			// resolve new beam path
			SendUpdatePulse();

			// reenable rotations
			_isRotating = false;
			_visual.EnableRotation();
		}
	}

	public void LoadLayout () {

		// get ready to load file
		var turretCommands = new List<string>();
		var filepath = Application.dataPath + "/Data/Levels/level" + _levelID + ".vgl";

		// load file into strings
		var reader = new  StreamReader(filepath);
        try {
            do {
                turretCommands.Add(reader.ReadLine());
            } while(reader.Peek() != -1);
        } catch {
            Debug.Log("Level File is empty");
        } finally {
            reader.Close();
        }

        // 
        int numTurrets = turretCommands.Count;
        turrets = new Turret[numTurrets];

        // prime visuals
        _visual.PrimeTurretArray(numTurrets);

        // extract turret info and create them
        int turretID = 0;
        char[] delimiters = {','};
        var spawners  = new List<int>();
        var receviers = new List<int>();

        foreach (string command in turretCommands) {

        	string[] commands = command.Split(delimiters);

        	int col = int.Parse(commands[0]);
        	int row = int.Parse(commands[1]);
        	var type = (Turret.Type) Enum.Parse(typeof(Turret.Type), commands[2]);
        	var rotation = (Turret.Rotation) Enum.Parse(typeof(Turret.Rotation), commands[3]);
        	var layout = (Turret.Layout) Enum.Parse(typeof(Turret.Layout), commands[4]);
        	var direction = (Turret.Direction) Enum.Parse(typeof(Turret.Direction), commands[5]);

        	if ( type == Turret.Type.Spawner ) {
        		spawners.Add(turretID);
        	} else if ( type == Turret.Type.Receiver ) {
        		receviers.Add(turretID);
        	}

        	createTurret(turretID, col, row, type, rotation, layout, direction);
        	turretID++;
        }

        // save spawner turrets
        spawnerTurretIDs = new int[spawners.Count];
        spawnerTurretIDs = spawners.ToArray();

        receiverTurretIDs = new int[receviers.Count];
        receiverTurretIDs = receviers.ToArray();

		layoutTurrets();
	}

	public void InitializeBeam () {

		foreach (int id in spawnerTurretIDs) {
			turrets[id].PowerOn();
		}
	}

	public void SendUpdatePulse () {

		// make each spawner pulse
		foreach (int id in spawnerTurretIDs) {
			turrets[id].SendUpdatePulse();
		}

		// check if all receivers are on
		foreach (int id in receiverTurretIDs) { 
			if ( turrets[id]._status == Turret.Status.PoweredOff ) {
				return;
			}
		}

		levelComplete ();
	}

	public Turret NextTurret ( int id, int col, int row, Turret.Direction direction ) {

		switch (direction)
		{
		case Turret.Direction.Up:
			for ( int i = row-1; i >= 1; i-- ) {
				var nextID = getTurretID(col, i);
				if ( nextID.HasValue ) {
					return turrets[nextID.Value];
				}
			}
			break;

		case Turret.Direction.Down:
			for ( int i = row+1; i <= _numRows; i++ ) {
				var nextID = getTurretID(col, i);
				if ( nextID.HasValue ) {
					return turrets[nextID.Value];
				}
			}
			break;

		case Turret.Direction.Left:
			for ( int i = col-1; i >= 1; i-- ) {
				var nextID = getTurretID(i, row);
				if ( nextID.HasValue ) {
					return turrets[nextID.Value];
				}
			}
			break;

		case Turret.Direction.Right:
			for ( int i = col+1; i <= _numColumns; i++ ) {
				var nextID = getTurretID(i, row);
				if ( nextID.HasValue ) {
					return turrets[nextID.Value];
				}
			}
			break;
		}
		return null;
	}

	public void ResetGrid () {

		Debug.Log("Resetting Grid");
		foreach (Turret turret in turrets) {
			resolvingTurretIDs.Add(turret._id);
			turret.Reset();
		}
	}

	// private functions
	private void levelComplete () {

		Debug.Log("Level Complete");
	}

	private void layoutTurrets () {
		foreach (Turret t in turrets) {
			var r = t._row;
			var c = t._col;
			var v = t.GetComponent<VisualTurret>();
			var pos = _visual.GetPositionFor(c, r);
			v.SetPosition(pos);
		}

		InitializeBeam();
	}

	private void createTurret ( int turretID, int col, int row, Turret.Type type, Turret.Rotation rotation, Turret.Layout layout, Turret.Direction initialDirection ) {
		
		// create a turret, initilize it, and save it
		var turretObject = _visual.CreateTurret(turretID);

		var turret = turretObject.GetComponent<Turret>();
		turret.Initialize(turretID, col, row, this, type, rotation, layout, initialDirection);
		turrets[turretID] = turret;
		
		// let the rows/cols know about it
		columns[col-1].AddNewTurret(turretID);
		rows[row-1].AddNewTurret(turretID);
	}

	private int? getTurretID ( int col, int row ) {
		var column = columns[col-1];
		var ids = column.GetTurretIDs();
		foreach (int id in ids) {
			var turret = turrets[id];
			if (turret._row == row) {
				return id;
			}
		}
		return null;
	}
}
