using UnityEngine;

public class TurretHead : MonoBehaviour {

	[SerializeField] Transform laserBeam;

	public void SendBeam ( Transform destination, float scaleOffset, float duration = 0.0f ) {
		float scale = 25.0f;
		if (destination != null) {
			var xDelta = destination.position.x - laserBeam.position.x;
			var yDelta = destination.position.y - laserBeam.position.y;
			if (xDelta < 0.01f && xDelta > -0.01f) {
				scale = Mathf.Abs(yDelta) / scaleOffset;
			} else if (yDelta < 0.01f && yDelta > -0.01f) {
				scale = Mathf.Abs(xDelta) / scaleOffset;
			} else {
				scale = -2.0f;
				Debug.Log("Beam: " + laserBeam.transform.position.x + " " + laserBeam.transform.position.y);
				Debug.Log("Dest: " + destination.transform.position.x + " " + destination.transform.position.y);
				Debug.Log("Tried to connect to port at a diagonal?");
			}
		}
		laserBeam.transform.localScale = new Vector3(laserBeam.transform.localScale.x, scale);
	}

	public void RetractBeam ( float duration = 0.0f ) {
		laserBeam.transform.localScale = new Vector3(laserBeam.transform.localScale.x, 0.0f);
	}

}
