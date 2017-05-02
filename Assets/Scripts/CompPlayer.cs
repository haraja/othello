using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Notice, that this class proposes moves always for black color only
public class CompPlayer : MonoBehaviour{

	GameController gameController;
	List<Vector2> validMoves = new List<Vector2>();


	public void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent<GameController> ();
		else
			Debug.Log ("CompPlayer::Cannot Find GameController");
	}


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
				if (gameController.IsValidMove (x, y, ChipColor.BLACK))
					validMoves.Add (new Vector2 (x, y));
			}
		}
		Debug.Log ("validMoves count: " + validMoves.Count);

		Vector2? returnVector;
		if (validMoves.Count == 0)
			returnVector = null;
		else {
			int randomMove = Random.Range (0, validMoves.Count);
			 returnVector = validMoves [randomMove];

			// for debug/development purposes, always return the same move
			//returnVector = validMoves [0];
		}
		return returnVector;
	}
}
