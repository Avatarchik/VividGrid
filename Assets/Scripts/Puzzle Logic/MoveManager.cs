using UnityEngine;
using System.Collections.Generic;

public class Move {

	private GridLine.Type lineType;
	private int gridLineIndex;

	public GridLine.Type Type {
		get {
			return lineType;
		}
	}

	public int ID {
		get {
			return gridLineIndex;
		}
	}

	public Move ( GridLine.Type type, int id ) {
		lineType = type;
		gridLineIndex = id;
	}
}

[RequireComponent(typeof(PuzzleGrid))]
public class MoveManager : MonoBehaviour {

	private PuzzleGrid grid;
	private Stack<Move> prevMoves;
	private Stack<Move> nextMoves;

	public int NumberOfMoves {
		get {
			return prevMoves.Count;
		}
	}

	void Start () {
		grid = GetComponent<PuzzleGrid>();
		prevMoves = new Stack<Move>();
		nextMoves = new Stack<Move>();
	}

	public void RegisterMove ( GridLine.Type type, int id ) {
		var newMove = new Move (type, id);
		prevMoves.Push(newMove);
		nextMoves.Clear();
	}

	public void UndoLastMove () {
		if (prevMoves.Count > 0) {
			var lastMove = prevMoves.Pop();
			nextMoves.Push(lastMove);
			switch (lastMove.Type)
			{
				case GridLine.Type.Column:
					grid.UndoColumn(lastMove.ID);
					break;
				case GridLine.Type.Row:
					grid.UndoRow(lastMove.ID);
					break;
			}
		}
	}

	public void RedoNextMove () {
		if (nextMoves.Count > 0) {
			var nextMove = nextMoves.Pop();
			prevMoves.Push(nextMove);
			switch (nextMove.Type)
			{
				case GridLine.Type.Column:
					grid.RedoColumn(nextMove.ID);
					break;
				case GridLine.Type.Row:
					grid.RedoRow(nextMove.ID);
					break;
			}
		}
	}

	public void Reset () {
		prevMoves.Clear();
		nextMoves.Clear();
	}
}
