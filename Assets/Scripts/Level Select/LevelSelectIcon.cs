using UnityEngine;
using System.Collections;

public class LevelSelectIcon : MonoBehaviour {

	private LevelSelectZone _zone;

	[SerializeField] private Turret.Direction direction;

	// object links
	public RectTransform rotations;
	public GameObject lockedCenter;
	public GameObject unlockedCenter;
	public GameObject incompleteRing;
	public GameObject completedRing;
	public GameObject acedRing;

	public Transform beam;
	public float beamLength;

	public int levelID_ref;
	public LevelSelectZone Zone { get { return _zone; } }

	void Start() {
		_zone = transform.parent.GetComponent<LevelSelectZone>();
		setRotation();
	}

	public void ButtonPressed () {
		Zone.LevelWasSelected(levelID_ref);
		// Debug.Log("Level Selected at " + Zone.zoneName + " " + levelID_ref);
	}

	public void SetState ( bool unlocked, bool complete, bool aced ) {
		lockedCenter.SetActive(!unlocked);
		unlockedCenter.SetActive(unlocked);
		if (unlocked) {
			incompleteRing.SetActive(!complete);
			completedRing.SetActive(complete);
			acedRing.SetActive(aced);

			if (complete) {
				beam.localScale = new Vector3(1.0f, beamLength);
			}
		}
	}

	private void setRotation () {
		float angle = 0;
		switch (direction)
		{
			case Turret.Direction.Up:
				break;
			case Turret.Direction.Right:
				angle = -90.0f;
				break;
			case Turret.Direction.Down:
				angle = 180.0f;
				break;
			case Turret.Direction.Left:
				angle = 90.0f;
				break;

		}
		rotations.eulerAngles = new Vector3(0,0,angle);
	}
}
