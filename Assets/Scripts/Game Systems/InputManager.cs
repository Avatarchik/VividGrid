using UnityEngine;

public class InputManager : MonoBehaviour {

	private Vector2? touchPos;
	private bool touchUp;
	private bool touchDown;

	[SerializeField] private bool debug;

	void Start () {
		
	}
	
	void Update () {

		touchDown = false;
		touchUp = false;

		// if any touches are recognized
		if (Input.touchCount > 0) {
			if (!touchPos.HasValue) {
				if (debug) {
					Debug.Log("touch detected at (" + Input.GetTouch (0).position.x + "," + Input.GetTouch (0).position.y + ")");
				}
				
				touchDown = true;
			}

			// update touch position
			touchPos = Input.GetTouch (0).position;

			// get deltas to figure out gestures

		} else {

			if (touchPos.HasValue) {
				if (debug) {
					Debug.Log("touch up at (" + touchPos.Value.x + "," + touchPos.Value.y + ")");
				}
				touchUp = true;
			}

			touchPos = null;

		}

	}

	public bool TouchUp() {
		return touchUp;
	}

	public bool TouchDown() {
		return touchDown;
	}

	public Vector2? GetCurrentTouchScreenPos() {
		return touchPos;
	}

	public Vector2? GetCurrentTouchWorldPos() {
		if (touchPos.HasValue) {
			return Camera.main.ScreenToWorldPoint ( touchPos.Value );
		}
		return null;
	}

	public bool TouchIsInCircleWithRadius(Vector2 position, float radius, bool world = true) {

		Vector2 touchPosition;

		if (world) {
			Vector2? temp = GetCurrentTouchWorldPos();
			if (temp.HasValue) {
				touchPosition = temp.Value;
			} else {
				// Debug.Log ("couldn't get touch in circle because no touch");
				return false;
			}
		} else {
			Vector2? temp = GetCurrentTouchScreenPos();
			if (temp.HasValue) {
				touchPosition = temp.Value;
			} else {
				// Debug.Log ("couldn't get touch in circle because no touch");
				return false;
			}
		}

		float distance = Vector2.Distance(touchPosition, position);
		// Debug.Log (distance);
		if (distance < radius) {
			return true;
		}
		return false;

	}

	public bool TouchIsInCircleWithRadiusAndOffset(Vector2 position, Vector2 offset, float radius, bool world = true) {
		return TouchIsInCircleWithRadius((position - offset), radius, world);
	}

}