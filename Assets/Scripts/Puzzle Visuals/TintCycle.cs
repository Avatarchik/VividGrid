using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TintCycle : MonoBehaviour {

	public float transitionTime = 1;
	public float timeOffset = 0;
	
	public List<MyCollection> colorLayouts;
	
	private int currentIndex = 0;
	// private SpriteRenderer objectRenderer;
	
	[System.Serializable]
	public class MyCollection
	{
		public int red;
		public int green;
	    public int blue;
	}

	void Awake(){
		// objectRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	void Start () {
		cycleColor();
	}
	
	void cycleColor(){
		currentIndex = currentIndex+1;
		if (currentIndex >= colorLayouts.Count){
			currentIndex = 0;
		}
		float currentR = colorLayouts[currentIndex].red/255.0f;
		float currentG = colorLayouts[currentIndex].green/255.0f;
		float currentB = colorLayouts[currentIndex].blue/255.0f;
		startTween(currentR,currentG,currentB);
	}
	void startTween(float currentR,float currentG,float currentB){
		Color newColor = new Color(currentR,currentG,currentB);
		LeanTween.color(gameObject,newColor, transitionTime - timeOffset).setOnComplete(
			()=>{
				timeOffset = 0;
				cycleColor();
			}
		);
	}
}
