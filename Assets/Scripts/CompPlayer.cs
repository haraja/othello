//TOD: generalize checking valid moves

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Notice, that this class proposes moves always for black color only
public class CompPlayer : MonoBehaviour{

	GameController gameController;


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
		//Debug.Assert (validMoves.Count == 0, "ERROR::ProposeMove: validMoves List not empty in beginning");

		Vector2? proposedMove = new Vector2 ();
		switch (compStrategy)
		{
		case CompStrategy.RANDOM:
			proposedMove = RandomMode (gameBoard);
			break;
		case CompStrategy.GREEDY:
			proposedMove = GreedyMode (gameBoard);
			break;
		default:
			Debug.Assert (false, "ERROR::IsValidMove: Unexpected condition reached");
			break;
		}

		//validMoves.Clear ();
		return proposedMove;
	}


	// returns place for random suitable move
	public Vector2? RandomMode (GameObject[,] gameboard)
	{
		List<Vector2> validMoves = new List<Vector2>();

		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				if (gameController.IsValidMove (x, y, ChipColor.BLACK) > 0)
					validMoves.Add (new Vector2 (x, y));
			}
		}
		Debug.Log ("validMoves count: " + validMoves.Count);

		Vector2? returnPosition;
		if (validMoves.Count == 0)
			returnPosition = null;
		else {
			int randomMove = Random.Range (0, validMoves.Count);
			 returnPosition = validMoves [randomMove];

			// for debug/development purposes, always return the same move
			//returnPosition = validMoves [0];
		}
		return returnPosition;
	}


	// returns place for move, which brings most chips
	public Vector2? GreedyMode (GameObject[,] gameboard)
	{
		List<Vector3> validMoves = new List<Vector3>();		// all valid moves
		List<Vector2> greedyMoves = new List<Vector2>();	// moves with most chips

		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				int flipCount = gameController.IsValidMove (x, y, ChipColor.BLACK);
				if (flipCount > 0)
					validMoves.Add (new Vector3 (x, y, flipCount)); // x,y are location; z is the count of flipped chips on this location
			}
		}

		Vector2? returnPosition ;
		if (validMoves.Count == 0)
			returnPosition = null;
		else {
			int flipMaxCount = 0;
			//int flipMaxIndex = 0;
			for (int i = 0; i < validMoves.Count; i++) {
				if (validMoves [i].z == flipMaxCount) {
					flipMaxCount = (int)validMoves [i].z;
					greedyMoves.Add (new Vector2 (validMoves [i].x, validMoves [i].y));
				} else if (validMoves [i].z > flipMaxCount) {
					greedyMoves.Clear ();
					flipMaxCount = (int)validMoves [i].z;
					greedyMoves.Add (new Vector2 (validMoves [i].x, validMoves [i].y));
				}
			}

			int randomGreedyMove = Random.Range (0, greedyMoves.Count);
			returnPosition = greedyMoves [randomGreedyMove];
		}

		return returnPosition;
	}
}
