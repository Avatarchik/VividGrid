using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	private ScrollRect scrollRect;

	void Start () {
		scrollRect = GetComponent<ScrollRect>();
	}

	public void OnBeginDrag (PointerEventData data)  {
		// Debug.Log("They started dragging at " + data.position);
		// Debug.Log("They started dragging at " + scrollRect.verticalNormalizedPosition);
	}

	public void OnDrag (PointerEventData data)  {
		// Debug.Log("They are dragging at " + data.position);
		// Debug.Log("They are dragging at " + scrollRect.verticalNormalizedPosition);
	}

	public void OnEndDrag (PointerEventData data)  {
		// Debug.Log("They stopped dragging at " + data.position);
		// Debug.Log("They stopped dragging at " + scrollRect.verticalNormalizedPosition);
	}
}
