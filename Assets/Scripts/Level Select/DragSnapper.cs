using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
/// <summary>
/// Snap a scroll rect to its children items. All self contained.
/// Note: Only supports 1 direction
/// </summary>

// The direction we are snapping in
public enum SnapDirection {
	Horizontal,
	Vertical
}

public class DragSnapper : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public LevelSelectManager _levelSelectManager;

	public ScrollRect scrollRect; // the scroll rect to scroll
	public SnapDirection direction; // the direction we are scrolling
	public int itemCount; // how many items we have in our scroll rect

	public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // a curve for transitioning in order to give it a little bit of extra polish
	public float speed; // the speed in which we snap ( normalized position per second? )

	private float dragSpeed;
	private int lastConfirmedTarget = 0;

	protected override void Reset() {

		base.Reset();

		if (scrollRect == null) // if we are resetting or attaching our script, try and find a scroll rect for convenience 
			scrollRect = GetComponent<ScrollRect>();
	}

	public void SetZoneFocus ( int zoneNumber ) {
		scrollRect.verticalNormalizedPosition = (1f / (float)(itemCount - 1)) * zoneNumber;
	}

	public void OnBeginDrag(PointerEventData eventData) {

		StopCoroutine(SnapRect()); // if we are snapping, stop for the next input
	}

	public void OnDrag(PointerEventData eventData) {

		float deltaMovement = ( direction == SnapDirection.Horizontal ) ? eventData.delta.x : eventData.delta.y;
		dragSpeed = deltaMovement / Time.deltaTime;
		// Debug.Log("Scroll Delta: " + dragSpeed);
	}

	public void OnEndDrag(PointerEventData eventData) {

		StartCoroutine(SnapRect()); // simply start our coroutine ( better than using update )
	}

	private IEnumerator SnapRect() {

		if (scrollRect == null)
			throw new System.Exception("Scroll Rect can not be null");
		if (itemCount == 0)
			throw new System.Exception("Item count can not be zero");

		// find our start position
		float startNormal = direction == SnapDirection.Horizontal ?
					scrollRect.horizontalNormalizedPosition :
					scrollRect.verticalNormalizedPosition;

		// percentage each item takes
		float spacing = 1f / (float)(itemCount - 1);
		float speedModifier = Mathf.Clamp(-dragSpeed/2000f, -spacing/2f, spacing/2f);

		// determine target zones
		int nextTarget = Mathf.RoundToInt( (startNormal + speedModifier) / spacing); // this finds us the closest target based on our starting point
		nextTarget = Mathf.Clamp(nextTarget, 0, itemCount-1);
		Debug.Log("Next Target: " + nextTarget);

		// tell the level select about it
		if (_levelSelectManager.MoveToZone(nextTarget)) {
			lastConfirmedTarget = nextTarget;
		} else {
			nextTarget = lastConfirmedTarget;
		}
		
		// this finds the normalized value of our target
		float endNormal = spacing * nextTarget;
		float duration = Mathf.Abs((endNormal - startNormal) / speed);

		float timer = 0f; // timer value of course
		while (timer < 1f) { // loop until we are done
			
			timer = Mathf.Min(1f, timer + Time.deltaTime / duration); // calculate our timer based on our speed
			// Debug.Log("Start Normal: " + startNormal);
			// Debug.Log("Spacing: " + spacing);
			// Debug.Log("Target: " + target);
			// Debug.Log("EndNormal: " + endNormal);
			// Debug.Log("Duration: " + duration);
			// Debug.Log("Timer: " + timer);
			float value = Mathf.Lerp(startNormal, endNormal, curve.Evaluate(timer)); // our value based on our animation curve, cause linear is lame

			if (direction == SnapDirection.Horizontal) // depending on direction we set our horizontal or vertical position
				scrollRect.horizontalNormalizedPosition = value;
			else
				scrollRect.verticalNormalizedPosition = value;

			yield return new WaitForEndOfFrame(); // wait until next frame
		}
	}
 }
