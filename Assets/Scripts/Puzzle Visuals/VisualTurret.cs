using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Turret), typeof(SpriteRenderer))]
public class VisualTurret : MonoBehaviour {

	// interal component references
	private SpriteRenderer _sprite;
	private Turret _turret;

	// editor connections
	[SerializeField] private Transform _upAnchor;
	[SerializeField] private Transform _rightAnchor;
	[SerializeField] private Transform _downAnchor;
	[SerializeField] private Transform _leftAnchor;

	// prefabs
	[SerializeField] private Object turretHead_prefab;
	[SerializeField] private Object turretPort_prefab;

	// sprites
	[SerializeField] private Sprite turret_cw_90;
	[SerializeField] private Sprite turret_ccw_90;
	[SerializeField] private Sprite turret_180;
	[SerializeField] private Sprite turret_static;
	[SerializeField] private Sprite turret_spawner;
	[SerializeField] private Sprite turret_receiver;
	[SerializeField] private Sprite turret_port;

	// components
	private GameObject[] heads;

	public void Initialize (Turret.Type type, Turret.Rotation rotation, Turret.Layout layout, Turret.Direction initialDirection) {

		_sprite = GetComponent<SpriteRenderer>();
		_turret = GetComponent<Turret>();

		setLayout(layout);
		setInitialDirection(initialDirection);
		setRotation(rotation);
		setType(type);
	}

	public void SetPosition ( Vector2 position ) {

		transform.position = new Vector3 ( position.x, position.y );
	}

	public void InitiateRotationAction ( Turret.Direction startDirection, Turret.Rotation rotation ) {

		int startDir = (int)startDirection;
		int rotAmt = 0;
		float rotationDeg = 0;

		// get rotation data
		switch ( rotation )
		{
		case Turret.Rotation.CW_90:
			rotationDeg = -90.0f;
			rotAmt = 1;
			break;
			
		case Turret.Rotation.CCW_90:
			rotationDeg = 90.0f;
			rotAmt = -1;
			break;
		
		case Turret.Rotation.CW_180:
			rotationDeg = -180.0f;
			rotAmt = 2;
			break;
		
		case Turret.Rotation.CCW_180:
			rotationDeg = 180.0f;
			rotAmt = 2;
			break;
		}

		// determine end direction
		int endDir = startDir + rotAmt;
		if (endDir > 3) { endDir -= 4; }
		else if (endDir < 0) { endDir += 4; }
		var endDirection = (Turret.Direction)endDir;

		// let the turret know it's logic state
		_turret.WillRotate(endDirection);

		// tween rotation
		// TODO: Make this actually tween
		StartCoroutine(rotationTween(rotationDeg, endDirection));
	}

	public void SendBeams () {
		foreach (GameObject o in heads) {
			var head = o.GetComponent<TurretHead>();
			if ( head != null ) {

				var anchor = head.transform.parent;
				var direction = convertAnchorToDirection(anchor);
				var destinationTransform = _turret.DestinationForBeam(direction);

				var scaleOffset = transform.localScale.x;
				if (destinationTransform != null) {
					Debug.Log(destinationTransform.name);
				}
				

				head.SendBeam(destinationTransform, scaleOffset);
			}
		}
	}

	public void RetractBeams () {
		foreach (GameObject o in heads) {
			var head = o.GetComponent<TurretHead>();
			if ( head != null ) {
				head.RetractBeam();
			}
		}
	}

	public void Reset ( Turret.Direction initialDirection ) {

		float endRotation = 0.0f;

		switch (initialDirection)
		{
		case Turret.Direction.Right:
			endRotation = 0.0f;
			break;
		case Turret.Direction.Up:
			endRotation = 90.0f;
			break;
		case Turret.Direction.Left:
			endRotation = 180.0f;
			break;
		case Turret.Direction.Down:
			endRotation = 270.0f;
			break;
		}

		float difference = endRotation - transform.rotation.eulerAngles.z;

		// let the turret know it's logic state
		_turret.WillRotate(initialDirection);

		// tween rotation
		// TODO: Make this actually tween
		StartCoroutine(rotationTween(difference, initialDirection));
	}

	public Transform GetPortTransformAtPosition ( Turret.Direction position ) {
		switch (position)
		{
			case Turret.Direction.Up:
				return _upAnchor.GetChild(0).GetChild(0);

			case Turret.Direction.Right:
				return _rightAnchor.GetChild(0).GetChild(0);

			case Turret.Direction.Down:
				return _downAnchor.GetChild(0).GetChild(0);

			case Turret.Direction.Left:
				return _leftAnchor.GetChild(0).GetChild(0);
		}
		return null;
	}

	private IEnumerator rotationTween ( float deltaRotationDEG, Turret.Direction endDirection ) {

		yield return new WaitForSeconds(0.1f);

		transform.Rotate(Vector3.forward, deltaRotationDEG);

		// on completion verify
		verifyRotation(endDirection);
	}

	// private functions
	private void verifyRotation ( Turret.Direction endDirection ) {

		switch ( endDirection )
		{
		case Turret.Direction.Up:
			transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
			break;

		case Turret.Direction.Right:
			transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
			break;

		case Turret.Direction.Down:
			transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
			break;

		case Turret.Direction.Left:
			transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
			break;
		}

		_turret.DidRotate();
	}


	// visual set up functions
	private void setInitialDirection ( Turret.Direction direction ) {

		switch (direction)
		{
		case Turret.Direction.Up:
			transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
			break;

		case Turret.Direction.Right:
			transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
			break;

		case Turret.Direction.Down:
			transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
			break;

		case Turret.Direction.Left:
			transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
			break;
		}
	}

	private void setLayout ( Turret.Layout layout ) {

		switch (layout)
		{
		case Turret.Layout.Single:
			heads = new GameObject[4];
			spawnHead(Turret.Direction.Up, 0);
			spawnPort(Turret.Direction.Right, 1);
			spawnPort(Turret.Direction.Down, 2);
			spawnPort(Turret.Direction.Left, 3);
			break;

		case Turret.Layout.Double_90:
			heads = new GameObject[4];
			spawnHead(Turret.Direction.Up, 0);
			spawnHead(Turret.Direction.Right, 1);
			spawnPort(Turret.Direction.Down, 2);
			spawnPort(Turret.Direction.Left, 3);
			break;
			
		case Turret.Layout.Double_180:
			heads = new GameObject[4];
			spawnHead(Turret.Direction.Up, 0);
			spawnPort(Turret.Direction.Right, 1);
			spawnHead(Turret.Direction.Down, 2);
			spawnPort(Turret.Direction.Left, 3);
			break;
			
		case Turret.Layout.Triple:
			heads = new GameObject[4];
			spawnHead(Turret.Direction.Up, 0);
			spawnHead(Turret.Direction.Right, 1);
			spawnHead(Turret.Direction.Down, 2);
			spawnPort(Turret.Direction.Left, 3);
			break;
			
		case Turret.Layout.Receiver:
			heads = new GameObject[4];
			spawnHead(Turret.Direction.Up, 0);
			spawnHead(Turret.Direction.Right, 1);
			spawnHead(Turret.Direction.Down, 2);
			spawnHead(Turret.Direction.Left, 3);
			break;
		}
	}

	private void spawnHead ( Turret.Direction direction, int id ) {

		// create a head object
		var head = (GameObject)Instantiate(turretHead_prefab);
		head.name = "Turret Head";

		// set its anchor
		switch (direction)
		{
		case Turret.Direction.Up:
			head.transform.parent = _upAnchor;
			break;

		case Turret.Direction.Right:
			head.transform.parent = _rightAnchor;
			break;

		case Turret.Direction.Down:
			head.transform.parent = _downAnchor;
			break;

		case Turret.Direction.Left:
			head.transform.parent = _leftAnchor;
			break;
		}

		// set transform
		head.transform.localScale = Vector3.one;
		head.transform.localPosition = Vector3.zero;
		head.transform.localRotation = Quaternion.identity;

		// save reference
		heads[id] = head;
	}

	private void spawnPort ( Turret.Direction direction, int id ) {

		// create a port object
		var port = (GameObject)Instantiate(turretPort_prefab);
		port.name = "Turret Port";

		// set its anchor
		switch (direction)
		{
		case Turret.Direction.Up:
			port.transform.parent = _upAnchor;
			break;

		case Turret.Direction.Right:
			port.transform.parent = _rightAnchor;
			break;

		case Turret.Direction.Down:
			port.transform.parent = _downAnchor;
			break;

		case Turret.Direction.Left:
			port.transform.parent = _leftAnchor;
			break;
		}

		// set transform
		port.transform.localScale = Vector3.one;
		port.transform.localPosition = Vector3.zero;
		port.transform.localRotation = Quaternion.identity;

		// save reference
		heads[id] = port;
	}

	private void setType ( Turret.Type type ) {

		switch (type)
		{
			case Turret.Type.Spawner:
				_sprite.sprite = turret_spawner;
				setHeadColor(Color.white);
				break;
				
			case Turret.Type.Receiver:
				_sprite.sprite = turret_receiver;
				setHeadColor(Color.white);
				break;
		}
	}

	private void setRotation ( Turret.Rotation rotation ) {
		switch (rotation)
		{
			case Turret.Rotation.CW_90:
				_sprite.sprite = turret_cw_90;
				break;

			case Turret.Rotation.CCW_90:
				_sprite.sprite = turret_ccw_90;
				break;
			
			case Turret.Rotation.CW_180:
			case Turret.Rotation.CCW_180:
				_sprite.sprite = turret_180;
				break;
				
			case Turret.Rotation.Static:
				_sprite.sprite = turret_static;
				break;
		}
	}

	private void setHeadColor ( Color color ) {

		foreach (GameObject head in heads) {
			// head.GetComponent<SpriteRenderer>().color = color;
		}
	}

	private Turret.Direction convertAnchorToDirection ( Transform anchor ) {
		if (anchor == _upAnchor) {
			return Turret.Direction.Up;
		} else if (anchor == _rightAnchor) {
			return Turret.Direction.Right;
		} else if (anchor == _downAnchor) {
			return Turret.Direction.Down;
		} else {
			return Turret.Direction.Left;
		}
	}

}
