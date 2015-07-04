using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpawnTurretButton : MonoBehaviour {

	public int row;
	public int col;

	private SpriteRenderer spriteRenderer;

	public EditorTurret turret;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		SetVisuallyInactive();
	}

	public void SetVisuallyActive() {
		spriteRenderer.color = new Color(1, 1, 1, 1);
		Debug.Log("test");
	}

	public void SetVisuallyInactive() {
		spriteRenderer.color = new Color(1, 1, 1, 0.25f);
	}

	public string GetTurretString () {
		string turretInfo = "";

		turretInfo += col + ",";
		turretInfo += row + ",";
		turretInfo += turret.GetTypeString() + ",";
		turretInfo += turret.GetRotationString() + ",";
		turretInfo += turret.GetLayoutString() + ",";
		turretInfo += turret.GetDirectionString();

		return turretInfo;
	}

}
