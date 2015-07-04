using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EditorTurret : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	public Sprite single;
	public Sprite double90;
	public Sprite double180;
	public Sprite triple;
	public Sprite quad;

	private int _type = -1;
	private int _rotation = -1;
	private int _layout = -1;

	public void Startup() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		SetType(0);
		SetRotation(0);
		SetLayout(0);
	}

	public void SetType ( int type ) {
		if (type == 4) {
			_type = -1;
			SetLayout(4);
			SetRotation(0);
		} else if ( _type == 4 ) {
			_type = -1;
			SetLayout(0);
		}

		_type = type;
		switch (_type)
		{
			case 0:
				spriteRenderer.color = Color.red;
				break;
			case 1:
				spriteRenderer.color = Color.green;
				break;
			case 2:
				spriteRenderer.color = Color.blue;
				break;
			case 3:
				spriteRenderer.color = Color.white;
				break;
			case 4:
				spriteRenderer.color = Color.black;
				SetLayout(4);
				break;
			case 5:
				spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
				break;
		}
	}

	public void SetRotation ( int rot ) {
		if (_type == 4) return;

		_rotation = rot;
		switch (_rotation)
		{
			case 0:
				transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
				break;
			case 1:
				transform.rotation = Quaternion.AngleAxis(-90.0f, Vector3.forward);
				break;
			case 2:
				transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.forward);
				break;
			case 3:
				transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.forward);
				break;

		}
	}

	public void SetLayout ( int lay ) {
		if (_type == 4) return;

		_layout = lay;
		switch (_layout)
		{
			case 0:
				spriteRenderer.sprite = single;
				break;
			case 1:
				spriteRenderer.sprite = double90;
				break;
			case 2:
				spriteRenderer.sprite = double180;
				break;
			case 3:
				spriteRenderer.sprite = triple;
				break;
			case 4:
				if (_type != -1) {return;}
				spriteRenderer.sprite = quad;
				break;
		}
	}

	public string GetTypeString () {
		switch (_type)
		{
			case 0:
			case 1:
			case 2:
			case 5:
				return "Redirect";
				break;
			case 3:
				return "Spawner";
				break;
			case 4:
				return "Receiver";
				break;
		}
		return "Error";
	}

	public string GetRotationString () {
		switch (_type)
		{
			case 0:
				return "CW_90";
				break;
			case 1:
				return "CCW_90";
				break;
			case 2:
				return "CW_180";
				break;
			case 3:
			case 4:
			case 5:
				return "Static";
				break;
		}
		return "Error";
	}

	public string GetDirectionString () {
		switch (_rotation)
		{
			case 0:
				return "Up";
				break;
			case 1:
				return "Right";
				break;
			case 2:
				return "Down";
				break;
			case 3:
				return "Left";
				break;
		}
		return "Error";
	}

	public string GetLayoutString() {
		switch (_layout)
		{
			case 0:
				return "Single";
				break;
			case 1:
				return "Double_90";
				break;
			case 2:
				return "Double_180";
				break;
			case 3:
				return "Triple";
				break;
			case 4:
				return "Receiver";
				break;
		}
		return "Error";
	}

}