using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Notice, that this class proposes moves always for black color only
public class CompPlayer : MonoBehaviour {

	public GameController gameController;
	List<Vector2> validMoves = new List<Vector2>();


	public Vector2? ProposeMove (GameObject[,] gameBoard, CompStrategy compStrategy)
	{
		Debug.Assert (validMoves.Count == 0, "ERROR::ProposeMove: validMoves List not empty in beginning");

		Vector2? proposedMove = new Vector2 ();
		switch (compStrategy)
		{
		case CompStrategy.RANDOM:
			proposedMove = RandomMode (gameBoard);
			break;
		default:
			Debug.Assert (false, "ERROR::IsValidMove: Unexpected condition reached");
			break;
		}

		validMoves.Clear ();
		return proposedMove;
	}


	// returns place for random suitable move
	public Vector2? RandomMode (GameObject[,] gameboard)
	{
		// TODO: Remove arraylength hardcodings
		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				if (gameController.IsValidMove (x, y, ChipColor.BLACK, false))
					validMoves.Add (new Vector2 (x, y));
			}
		}
		Debug.Log ("validMoves count: " + validMoves.Count);

		Vector2? returnVector;
		if (validMoves.Count == 0)
			returnVector = null;
		else {
			//TODO: currently returning always first option - enable random again when bugs are fixed
			int randomMove = Random.Range (0, validMoves.Count);
			returnVector = validMoves [randomMove];
		}
		return returnVector;
	}
}
