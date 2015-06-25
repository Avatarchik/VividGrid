using UnityEngine;
using System.Collections;

public class FloatingBehavior : MonoBehaviour {
	
	public float fadeTimeMin = 1;
	public float fadeTimeMax = 1;
	public float minOpacity = 0;
	public float maxOpacity = 50;

	public float moveTimeMin = 1;
	public float moveTimeMax = 1;
	public float maxDistance = 1;
	
	private float originalX;
	private float originalY;

	private SpriteRenderer objectRenderer;
	
	void Awake(){
		objectRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	void Start () {
		//record starting position
		originalX = gameObject.transform.position.x;
		originalY = gameObject.transform.position.y;
		
		//set alpha on start
		Color color = objectRenderer.color;
		color.a = (Random.Range(minOpacity,maxOpacity)/100);
		objectRenderer.color = color;
		
		//start tweening
		startTweenOpacity();
		startTweenX();
		startTweenY();
	
	}

	void startTweenOpacity(){
		float targetTime = Random.Range(fadeTimeMin,fadeTimeMax);
		float targetValue = Random.Range(minOpacity,maxOpacity)/100;
		LeanTween.alpha(gameObject, targetValue, targetTime).setOnComplete(
			()=>{
				startTweenOpacity();
			}
		);
	}
	void startTweenX(){
		float targetValueX = Random.Range(originalX-maxDistance, originalX+maxDistance);
		float targetTime = Random.Range(moveTimeMin,moveTimeMax);
		LeanTween.moveX(gameObject,targetValueX, targetTime).setOnComplete(
			()=>{
				startTweenX();
			}
		);
	}
	void startTweenY(){
		float targetValueY = Random.Range(originalY-maxDistance, originalY+maxDistance);
		float targetTime = Random.Range(moveTimeMin,moveTimeMax);
		LeanTween.moveY(gameObject,targetValueY, targetTime).setOnComplete(
			()=>{
				startTweenY();
			}
		);
	}
}
