using UnityEngine;

public class Singletons : MonoBehaviour {

	// ************ Singleton Logic ***************
	public static Singletons Instance;
	void Awake() {
		if (Instance) {
			DestroyImmediate(gameObject);
		} else {
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
	}
	// ********************************************
	
}
