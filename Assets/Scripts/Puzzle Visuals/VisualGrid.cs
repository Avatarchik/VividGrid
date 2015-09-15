using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PuzzleGrid))]
public class VisualGrid : MonoBehaviour {

	[Header("Prefabs")]
	[SerializeField] private Object buttonColumn_prefab;
	[SerializeField] private Object buttonRow_prefab;
	[SerializeField] private Object baseTurret_prefab;
	[SerializeField] private Object gridLine_prefab;

	[Header("Grid Sizing")]
	[SerializeField] private float _gridSpacing;
	[SerializeField] private Vector3 _gridOffset;
	[SerializeField] private Vector2 _buttonOffset;

	// internal components and children
	[SerializeField] private Canvas _buttonCanvas;
	private PuzzleGrid _puzzleGrid;
	private GameObject _gridContainer;
	private GameObject _turretContainer;

	private GameObject[] colButtons;
	private GameObject[] rowButtons;
	private GameObject[] columns;
	private GameObject[] rows;
	private GameObject[] turrets;


	// initialization methods
	public void SetUpGrid () {

		_puzzleGrid = GetComponent<PuzzleGrid>();

		// set up container
		_gridContainer = new GameObject();
		_gridContainer.transform.position = Vector3.zero;
		_gridContainer.name = "Grid";

		int nCol = _puzzleGrid._numColumns;
		int nRow = _puzzleGrid._numRows;
		columns = new GameObject[nCol];
		rows    = new GameObject[nRow];

		// create columns
		var columnContainer = new GameObject();
		columnContainer.name = "Columns";
		columnContainer.transform.parent = _gridContainer.transform;

		for ( int i = 0; i < nCol; i++ ) {
			var col = (GameObject)Instantiate(gridLine_prefab);
			col.name = "Column " + i;
			col.transform.parent = columnContainer.transform;
			columns[i] = col;
		}

		// create rows
		var rowContainer = new GameObject();
		rowContainer.name = "Rows";
		rowContainer.transform.parent = _gridContainer.transform;

		for ( int i = 0; i < nRow; i++ ) {
			var row = (GameObject)Instantiate(gridLine_prefab);
			row.name = "Row " + i;
			row.transform.parent = rowContainer.transform;
			row.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
			rows[i] = row;
		}

		UpdateGridLayout();
	}
	public void SetUpButtons () {

		// _buttonCanvas.transform.SetParent(_gridContainer.transform);
		_buttonCanvas.transform.localPosition = Vector3.zero;

		// set up column buttons
		int numCol = columns.Length;
		colButtons = new GameObject[numCol];
		var colContainer = new GameObject();
		colContainer.name = "Column Buttons";
		colContainer.transform.parent = _buttonCanvas.transform;
		colContainer.transform.localScale = Vector3.one;

		float canvasWidth = _buttonCanvas.GetComponent<RectTransform>().rect.width;
		for ( int i = 0; i < numCol; i++ ) {
			var button = (GameObject)Instantiate(buttonColumn_prefab);
			button.name = "Button " + i;
			button.transform.SetParent(colContainer.transform);
			button.transform.localScale = Vector3.one;
			button.transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.forward);

			// set position
			float xPosVP = Camera.main.WorldToViewportPoint(columns[i].transform.position).x;
			float xPosCanvas = (xPosVP * canvasWidth) - (canvasWidth/2.0f);
			button.transform.localPosition = new Vector3( xPosCanvas, 0 );

			// set button action
			int columnIndex = i + 1;
			button.GetComponent<Button>().interactable = false;
			button.GetComponent<Button>().onClick.AddListener(() => {
					activateColumn(columnIndex);
				});

			colButtons[i] = button;
		}

		// set up row buttons
		int numRow = rows.Length;
		rowButtons = new GameObject[numRow];
		var rowContainer = new GameObject();
		rowContainer.name = "Row Buttons";
		rowContainer.transform.parent = _buttonCanvas.transform;
		rowContainer.transform.localScale = Vector3.one;

		float canvasHeight = _buttonCanvas.GetComponent<RectTransform>().rect.height;
		for ( int i = 0; i < numRow; i++ ) {
			var button = (GameObject)Instantiate(buttonRow_prefab);
			button.name = "Button " + i;
			button.transform.SetParent(rowContainer.transform);
			button.transform.localScale = Vector3.one;

			// set position
			float yPosVP = Camera.main.WorldToViewportPoint(rows[i].transform.position).y;
			float yPosCanvas = (yPosVP * canvasHeight) - (canvasHeight/2.0f);
			button.transform.localPosition = new Vector3( 0, yPosCanvas );

			// set button action
			int rowIndex = i + 1;
			button.GetComponent<Button>().interactable = false;
			button.GetComponent<Button>().onClick.AddListener(() => {
					activateRow(rowIndex);
				});

			rowButtons[i] = button;
		}

		// prepare the grid for button offset
		_gridContainer.transform.position = _gridOffset;

		// move the whole row of COLUMN buttons
		Vector3 bottomRowPositionWithWorldOffset = (rows[numRow-1].transform.position + new Vector3( 0, -_buttonOffset.y ) );
		Vector3 bottomRowVP = Camera.main.WorldToViewportPoint(bottomRowPositionWithWorldOffset);
		float bottomRowXPosCanvas = (bottomRowVP.x * canvasWidth) - (canvasWidth/2.0f);
		float bottomRowYPosCanvas = (bottomRowVP.y * canvasHeight) - (canvasHeight/2.0f);
		colContainer.transform.localPosition = new Vector3( bottomRowXPosCanvas, bottomRowYPosCanvas );

		// move the whole column of ROW buttons
		Vector3 leftColumnPositionWithWorldOffset = (columns[0].transform.position + new Vector3( -_buttonOffset.x, 0 ) );
		Vector3 leftColVP = Camera.main.WorldToViewportPoint(leftColumnPositionWithWorldOffset);
		float leftColXPosCanvas = (leftColVP.x * canvasWidth) - (canvasWidth/2.0f);
		float leftColYPosCanvas = (leftColVP.y * canvasHeight) - (canvasHeight/2.0f);
		rowContainer.transform.localPosition = new Vector3( leftColXPosCanvas, leftColYPosCanvas );
	}

	// layout information
	public void UpdateGridLayout () {

		int numCol = columns.Length;
		int numRow = rows.Length;

		float width  = (numCol - 1) * _gridSpacing;
		float height = (numRow - 1) * _gridSpacing;

		float colStart = -(width/2.0f);
		float rowStart = (height/2.0f);

		float colStep = _gridSpacing;
		float rowStep = -1.0f * _gridSpacing;

		for ( int i = 0; i < numCol; i++ ) {
			var columnToPlace = columns[i];
			columnToPlace.transform.position = new Vector3( colStart + (i * colStep), 0 );
		}

		for ( int i = 0; i < numRow; i++ ) {
			var rowToPlace = rows[i];
			rowToPlace.transform.position = new Vector3( 0, rowStart + (i * rowStep) );
		}
	}
	public void SetActiveButtons ( int[] activeColumnButtonIDs, int[] activeRowButtonIDs ) {

		float inactiveOffset = 0.33f;

		foreach (int i in activeColumnButtonIDs) {
			var c = colButtons[i-1].GetComponent<Button>();
			c.interactable = true;
			c.transform.Translate(new Vector3(inactiveOffset,0));
		}

		foreach (int i in activeRowButtonIDs) {
			var r = rowButtons[i-1].GetComponent<Button>();
			r.interactable = true;
			r.transform.Translate(new Vector3(inactiveOffset,0));
		}

		foreach (GameObject o in colButtons) {
			o.transform.Translate(new Vector3(-inactiveOffset,0));
		}

		foreach (GameObject o in rowButtons) {
			o.transform.Translate(new Vector3(-inactiveOffset,0));
		}
	}
	public void DisableRotation () {

		// TODO: Implement Rotation Disabled Functionality
		// this happens while rotation is in progress
	}
	public void EnableRotation () {
		
		// TODO: Implement Rotation Enabled Functionality
	}


	// turret management
	public void PrimeTurretArray ( int numTurrets ) {

		turrets = new GameObject[numTurrets];

		// create turret container
		_turretContainer = new GameObject();
		_turretContainer.name = "Turrets";
		_turretContainer.transform.parent = _gridContainer.transform;
	}
	public GameObject CreateTurret ( int id ) {

		var turret = (GameObject)Instantiate(baseTurret_prefab);
		turret.name = "Turret " + id;
 		turret.transform.parent = _turretContainer.transform;
		turrets[id] = turret;
		return turret;
	}

	// helpers
	public Vector2 GetPositionFor( int col, int row ) {
		float x = columns[col - 1].transform.position.x;
		float y = rows[row - 1].transform.position.y;
		return new Vector2( x, y );
	}

	// private functions
	private void activateColumn ( int columnIndex ) {

		_puzzleGrid.ActivateColumn(columnIndex);
	}
	private void activateRow ( int rowIndex ) {

		_puzzleGrid.ActivateRow(rowIndex);
	}
}
