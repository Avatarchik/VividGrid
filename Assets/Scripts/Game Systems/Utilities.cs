using UnityEngine;

public class Utilities : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		var mm = GameObject.Find("Music Manager");
		if (mm != null) {
			mm.transform.parent = transform;
		}
	}
}
