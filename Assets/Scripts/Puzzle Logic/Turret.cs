using UnityEngine;
using System.Collections.Generic;

public class TurretPort {

	public enum Type {

		Empty = 0,
		Input,
		Emitter,
	}

	public TurretPort		_visuallyConnectedPort;
	public TurretPort		_connectedPort;
	public Turret 			_turret;
	public Turret.Direction _position;
	public Type 			_type;
	public bool				_active;

	public TurretPort ( Turret turret ) {

		_turret = turret;
		_active = false;
	}

	public void Reset () {

		_active = false;
		_visuallyConnectedPort = null;
		_connectedPort = null;
	}

	/*
		WillRotate ()
		Breaks all forward and backward connections before a rotation happens. 
	*/
	public void WillRotate () {

		if ( _type == Type.Emitter ) {
			// if ( _connectedPort != null ) {
			// 	_connectedPort.DisconnectInput();
			// }
			RetractBeam();
		} else if ( _type == Type.Input ) {
			if ( _connectedPort != null ) {
				_connectedPort.RetractBeam();
			}
			// DisconnectInput();
		}
	}

	/*
		TryToConnectInput ()
		Takes a sending port as an argument, and if it is an input node, saves
		the sender (as a emitter) and tells the owner turret that the port
		information should be updated (usually to power it on).
	*/
	public bool TryToConnectInput ( TurretPort sender ) {

		if ( _type == Type.Input ) {
			_connectedPort = sender;
			_active = true;
			_turret.PortsChanged();

			Debug.Log( _connectedPort._turret.gameObject.name + _connectedPort._position + _connectedPort._type + " connected to " +  _turret.name + _position + _type );

			return true;
		}
		return false;
	}

	/*
		DisconnectInput ()
		This function simply powers down an input port, and then tells the
		turret to update its port information.
	*/
	public void DisconnectInput () {

		if ( _type == Type.Input ) {

			if ( _connectedPort != null ) {
				Debug.Log( _connectedPort._turret.gameObject.name + _connectedPort._position + _connectedPort._type + " disconnected from " +  _turret.name + _position + _type );
			}

			_connectedPort = null;
			_visuallyConnectedPort = null;
			_active = false;
			_turret.PortsChanged();
		}
	}

	public bool InputIsPowered () {

		if ( _type == Type.Input ) {
			return _active;
		}
		return false;
	}

	/*
		UpdatePules ()
		Passes the pulse signal through the web of connected ports until
		it hits a dead end. If it finds a dead end, it tries to cast a
		beam.
	*/
	public void UpdatePulse () {

		if ( _type == Type.Emitter ) {
			if ( _connectedPort != null ) {
				_connectedPort._turret.SendUpdatePulse();
			} else {
				CastBeam();
			}
		}
	}

	/*
		CastBeam ()
		This function determines the direction where a beam should
		be cast, and then checks to see if a turret (with a valid port)
		exists in that direction. If so, it connects to that input,
		and that port handles the consequences on its own.
	*/
	public void CastBeam () {

		if ( _type == Type.Emitter ) {

			_active = true;

			// find my world direction
			var worldDirection = _turret.GetWorldDirection( _position, _turret._nextDirection );

			// find potential turret
			Debug.Log("Looked for turret to the " + worldDirection + " of " + _turret.name);
			var turret = _turret.NextTurret( worldDirection );

			if (turret != null) {

				// figure out what port you'd be connecting to
				var newPortWorldDir = _turret.FlipDirection( worldDirection );
				var newPort = turret.GetPortAtWorldDirection( newPortWorldDir );
				if ( newPort.TryToConnectInput(this) ) {
					_connectedPort = newPort;
				} else {
					Debug.Log( _turret.name + " hit a turret, but no input was available!" );
				}
				_visuallyConnectedPort = newPort;
			} else {
				Debug.Log( _turret.name + " fired into nothing!" );
			}

			_turret.PortsChanged();
		}
	}

	/*
		RetractBeam ()
		This function disconnects itself from a connected port,
		if one exists. The disconnected port will handle the
		consequences on its own.
	*/
	public void RetractBeam () {

		if ( _type == Type.Emitter ) {

			// retract beam
			_active = false;

			// if connected to a port, disconnect
			if ( _connectedPort != null ) {
				_connectedPort.DisconnectInput();
				_connectedPort = null;
				_visuallyConnectedPort = null;
			}

			_turret.PortsChanged();
		}
	}
}

[RequireComponent(typeof(VisualTurret))]
public class Turret : MonoBehaviour {

	public enum Type {
		Redirect = 0,
		Spawner,
		Receiver
	}

	public enum Rotation {
		CCW_90 = 0,
		CW_90,
		CCW_180,
		CW_180,
		Static
	}

	public enum Layout {
		Single = 0,
		Double_90,
		Double_180,
		Triple,
		Receiver
	}

	public enum Direction {
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3
	}

	public enum Status {
		PoweredOff = 0,
		PoweredOn
	}

	private VisualTurret _visual;
	private PuzzleGrid _grid;

	// turret description
	public int _id;
	public int _row;
	public int _col;
	public Type 	_type;
	public Rotation _rotation;
	public Layout 	_layout;
	public TurretPort[] _ports;

	// turret status
	private Direction _initialDirection;
	public Direction  _direction;
	public Direction  _nextDirection;
	public Status 	  _status;

	public void Initialize ( int id, int col, int row, PuzzleGrid grid, Type type, Rotation rotation, Layout layout, Direction initialDirection ) {
		
		// save components
		_visual = GetComponent<VisualTurret>();
		_grid = grid;

		// initialze all fields
		_id = id;
		_col = col;
		_row = row;
		_type = type;
		_rotation = rotation;
		_layout = layout;
		_initialDirection = initialDirection;
		_direction = initialDirection;
		_nextDirection = initialDirection;
		_status = Status.PoweredOff;

		// set up ports
		initializePorts(_type, _layout);

		// finally, connect the visuals
		_visual.Initialize(_type, _rotation, _layout, _direction);
	}

	public void Reset () {
		_nextDirection = _direction = _initialDirection;
		PowerOff();
		_visual.Reset(_initialDirection);
	}

	public void Rotate () {
		if (_rotation != Rotation.Static) {
			_visual.InitiateRotationAction(_direction, _rotation);
		} else {
			_grid.ResolveRotation( _id );
		}
	}

	public void UndoRotate () {
		if (_rotation != Rotation.Static) {
			_visual.InitiateRotationAction(_direction, ReverseRotation(_rotation));
		} else {
			_grid.ResolveRotation( _id );
		}
	}

	public void WillRotate ( Direction nextDirection ) {

		_nextDirection = nextDirection;
		foreach ( TurretPort p in _ports ) {
			p.WillRotate();
		}
	}

	public void DidRotate () {
		
		_direction = _nextDirection;
		_grid.ResolveRotation( _id );
	}

	public void PortsChanged () {

		checkIfStillPowered();
	}

	public TurretPort GetPortAtWorldDirection ( Direction worldDirection ) {
		int local = (int)GetLocalDirection( worldDirection, _nextDirection );
		return _ports[local];
	}

	public void SendUpdatePulse () {
		foreach ( TurretPort p in _ports ) {
			p.UpdatePulse();
			_visual.SendBeams();
		}
	}

	public void PowerOn () {

		if ( _status != Status.PoweredOn ) {
			_status = Status.PoweredOn;
			foreach ( TurretPort p in _ports ) {
				p.CastBeam();
			}
			if (_type != Type.Receiver) {
				_visual.SendBeams();
			}
		}
	}

	public void PowerOff () {

		_visual.RetractBeams();

		if ( _status != Status.PoweredOff ) {
			_status = Status.PoweredOff;
			foreach ( TurretPort p in _ports ) {
				p.RetractBeam();
			}
		}
	}

	public Direction GetLocalDirection ( Direction worldPosition, Direction localOrientation ) {

		int localOr = (int)localOrientation;
		int worldPos = (int)worldPosition;
		int localDir = worldPos - localOr;
		if (localDir > 3) { localDir -= 4; }
		if (localDir < 0) { localDir += 4; }
		return (Turret.Direction)localDir;
	}

	public Direction GetWorldDirection ( Direction localPosition, Direction worldOrientation ) {

		int localPos = (int)localPosition;
		int worldOr = (int)worldOrientation;
		int worldDir = localPos + worldOr;
		if (worldDir > 3) { worldDir -= 4; }
		if (worldDir < 0) { worldDir += 4; }
		return (Turret.Direction)worldDir;
	}

	public Direction FlipDirection ( Direction inputDirection ) {

		int inputDir = (int)inputDirection;
		int flippedDir = inputDir + 2;
		if (flippedDir > 3) { flippedDir -= 4; }
		return (Turret.Direction)flippedDir;
	}

	public Rotation ReverseRotation ( Rotation rotation ) {
		switch (rotation)
		{
			case Rotation.CW_90:
				return Rotation.CCW_90;
			case Rotation.CCW_90:
				return Rotation.CW_90;
			case Rotation.CW_180:
				return Rotation.CCW_180;
			case Rotation.CCW_180:
				return Rotation.CW_180;
		}
		return Rotation.Static;
	}

	public Turret NextTurret ( Direction direction ) {

		return _grid.NextTurret( _id, _col, _row, direction );
	}

	public Transform DestinationForBeam ( Direction localDirection ) {
		var p = getPort(localDirection);
		var connectedPort = p._visuallyConnectedPort;
		if (connectedPort != null) {
			var connectedPortTransform = connectedPort._turret._visual.GetPortTransformAtPosition(connectedPort._position);
			return connectedPortTransform;
		} else {
			Debug.Log("Visually Connected Port is Null");
		}
		return null;
	}

	// private functions
	private void initializePorts ( Type type, Layout layout ) {

		_ports = new TurretPort[4];

		switch (type)
		{
			// case Turret.Type.Spawner:
			// 	createPort( TurretPort.Type.Emitter, Direction.Up 	 );
			// 	createPort( TurretPort.Type.Empty, 	 Direction.Right );
			// 	createPort( TurretPort.Type.Empty, 	 Direction.Down  );
			// 	createPort( TurretPort.Type.Empty, 	 Direction.Left  );
			// 	break;
				
			case Turret.Type.Receiver:
				createPort( TurretPort.Type.Input, Direction.Up    );
				createPort( TurretPort.Type.Input, Direction.Right );
				createPort( TurretPort.Type.Input, Direction.Down  );
				createPort( TurretPort.Type.Input, Direction.Left  );
				break;

			case Turret.Type.Spawner:
			case Turret.Type.Redirect:
				switch (layout)
				{
				case Turret.Layout.Single:
					createPort( TurretPort.Type.Emitter, Direction.Up    );
					createPort( TurretPort.Type.Input, 	 Direction.Right );
					createPort( TurretPort.Type.Input, 	 Direction.Down  );
					createPort( TurretPort.Type.Input, 	 Direction.Left ) ;
					break;

				case Turret.Layout.Double_90:
					createPort( TurretPort.Type.Emitter, Direction.Up    );
					createPort( TurretPort.Type.Emitter, Direction.Right );
					createPort( TurretPort.Type.Input, 	 Direction.Down  );
					createPort( TurretPort.Type.Input, 	 Direction.Left  );
					break;
					
				case Turret.Layout.Double_180:
					createPort( TurretPort.Type.Emitter, Direction.Up    );
					createPort( TurretPort.Type.Input, 	 Direction.Right );
					createPort( TurretPort.Type.Emitter, Direction.Down  );
					createPort( TurretPort.Type.Input, 	 Direction.Left  );
					break;
					
				case Turret.Layout.Triple:
					createPort( TurretPort.Type.Emitter, Direction.Up    );
					createPort( TurretPort.Type.Emitter, Direction.Right );
					createPort( TurretPort.Type.Emitter, Direction.Down  );
					createPort( TurretPort.Type.Input, 	 Direction.Left  );
					break;
				}
				break;
		}
	}

	private void resetPorts () {

	}

	private void createPort ( TurretPort.Type type, Direction direction ) {

		var port = new TurretPort ( this );

		port._type = type;
		port._position = direction;

		_ports[(int)direction] = port;
	}

	private TurretPort getPort ( Direction position ) {
		foreach ( TurretPort p in _ports ) {
			if ( p._position == position ) {
				return p;
			}
		}
		return null;
	}

	private void checkIfStillPowered () {

		if ( _type == Type.Spawner ) {
			return;
		}

		foreach ( TurretPort p in _ports ) {
			if ( p.InputIsPowered() ) {
				PowerOn();
				return;
			}
		}
		PowerOff();
	}
}
