//TOD: generalize checking valid moves

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


// Notice, that this class proposes moves always for black color only
public class CompPlayer : MonoBehaviour{

	GameController gameController;


	public void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent<GameController> ();
		else
			Debug.LogError ("ERROR::Start: Cannot Find GameController");
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
		case CompStrategy.CALCULATING1:
			proposedMove = Calculating1Mode (gameBoard);
			break;
		default:
			Debug.LogError ("ERROR::ProposeMove: Unexpected condition reached");
			break;
		}

		//validMoves.Clear ();
		return proposedMove;
	}


	// returns place for random move
	Vector2? RandomMode (GameObject[,] gameboard)
	{
		List<Vector2> validMoves = new List<Vector2>();

		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				if (gameController.IsValidMove (x, y, ChipColor.BLACK) > 0)
					validMoves.Add (new Vector2 (x, y));
			}
		}
		//Debug.Log ("validMoves count: " + validMoves.Count);

		Vector2? returnPosition;
		if (validMoves.Count == 0)
			return null;
		else
			return validMoves [Random.Range (0, validMoves.Count)];
	}


	//
	//	Game Modes
	//


	// returns place for move, which brings most chips
	Vector2? GreedyMode (GameObject[,] gameboard)
	{
		List<Vector3> validMoves = new List<Vector3>();		// all valid moves

		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				int flipCount = gameController.IsValidMove (x, y, ChipColor.BLACK);
				if (flipCount > 0)
					validMoves.Add (new Vector3 (x, y, flipCount)); // x,y are location; z is the count of flipped chips on this location
			}
		}

		if (validMoves.Count == 0)
			return null;
		else
			return SelectMoveFromValid (validMoves);
	}


	// returns place for move, which has most value. Blindly considers position of board, don't check other chips
	Vector2? Calculating1Mode (GameObject[,] gameboard)
	{
		List<Vector3> validMoves = new List<Vector3>();			// all valid moves
		List<Vector2> calculatedMoves = new List<Vector2>();	// moves with best calculated value

		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				int flipCount = gameController.IsValidMove (x, y, ChipColor.BLACK);
				if (flipCount > 0)
				{
					int value = 0;	// default value

					// corners
					if ((x == 0 && y == 0) || (x == 0 && y == 7) || (x == 7 && y == 0) || (x == 7 && y == 7))
						value = 100;

					// diagonal to corners
					else if ((x == 1 && y == 1) || (x == 1 && y == 6) || (x == 6 && y == 1) || (x == 6 && y == 6))
						value = -100;

					// next to corners, not diagonal
					else if (
						(x == 0 && (y == 1 && y == 6)) ||
						(x == 7 && (y == 1 && y == 6)) ||
						(y == 0 && (x == 1 && x == 6)) ||
						(y == 7 && (x == 1 && x == 6)))
						value = -50; 

					// edges excluding corners, next to corners
					else if (
						(x == 0 && (y > 1 && y < 6)) ||
						(x == 7 && (y > 1 && y < 6)) ||
						(y == 0 && (x > 1 && x < 6)) ||
						(y == 7 && (x > 1 && x < 6)))
						value = 30; 
							
					// 1 away from edges - excluding diagonal to corners
					else if (
						(x == 1 && (y > 1 && y < 6)) ||
						(x == 6 && (y > 1 && y < 6)) ||
						(y == 1 && (x > 1 && x < 6)) ||
						(y == 6 && (x > 1 && x < 6)))
						value = -30;

					validMoves.Add (new Vector3 (x, y, value)); // x,y are location; z is the count of flipped chips on this location
				}
			}
		}
			
		if (validMoves.Count == 0)
			return null;
		else
			return SelectMoveFromValid (validMoves);
	}


	//
	//	Helper Functions
	//


	// selects most valuable move from valid moves - random most valuable, if several
	Vector2 SelectMoveFromValid (List<Vector3> validMoves)
	{
		List<Vector2> valuableMoves = new List<Vector2> ();
		int maxValue = (int)validMoves [0].z;

		// fill the array with positions, which have most flips
		for (int i = 0; i < validMoves.Count ; i++) {
			if (validMoves [i].z == maxValue){
				maxValue = (int)validMoves [i].z;
				valuableMoves.Add (new Vector2 (validMoves [i].x, validMoves [i].y));
			} else if (validMoves [i].z > maxValue) {
				valuableMoves.Clear ();
				maxValue = (int)validMoves [i].z;
				valuableMoves.Add (new Vector2 (validMoves [i].x, validMoves [i].y));
			}
		}

		Assert.IsTrue (valuableMoves.Count != 0);

		return valuableMoves [Random.Range (0, valuableMoves.Count)];
	}
}
