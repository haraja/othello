using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

	enum chipColor {WHITE, BLACK};

	public GameObject gameboardImage;
	public GameObject chipBlack;
	public GameObject chipWhite;

	GameObject[,] gameBoard = new GameObject[8, 8];
	List<GameObject> chipsToTurn = new List<GameObject>();	// used to store chips, whicih potentially will be turned
	chipColor playerColor = chipColor.WHITE; 				// initially player is hardcoded to play with white

	void Start () {
		InitBoard ();
	}


	void InitBoard()
	{
		for (int i=0; i<8; i++){
			for (int j = 0; j < 8; j++) {
				gameBoard[i, j] = null;
			}
		}

/*		
		gameBoard [0, 0] = Instantiate (chipWhite,  GetCoordFromSquare(0, 0), Quaternion.identity) as GameObject;
		gameBoard [1, 1] = Instantiate (chipWhite,  GetCoordFromSquare(1, 1), Quaternion.identity) as GameObject;
		gameBoard [2, 2] = Instantiate (chipWhite,  GetCoordFromSquare(2, 2), Quaternion.identity) as GameObject;
		gameBoard [3, 3] = Instantiate (chipWhite,  GetCoordFromSquare(3, 3), Quaternion.identity) as GameObject;
		gameBoard [4, 4] = Instantiate (chipWhite,  GetCoordFromSquare(4, 4), Quaternion.identity) as GameObject;
		gameBoard [5, 5] = Instantiate (chipWhite,  GetCoordFromSquare(5, 5), Quaternion.identity) as GameObject;
		gameBoard [6, 6] = Instantiate (chipWhite,  GetCoordFromSquare(6, 6), Quaternion.identity) as GameObject;
		gameBoard [7, 7] = Instantiate (chipWhite,  GetCoordFromSquare(7, 7), Quaternion.identity) as GameObject;
*/

/*
		gameBoard [0, 2] = Instantiate (chipWhite,  GetCoordFromSquare(0, 2), Quaternion.identity) as GameObject;
		gameBoard [1, 2] = Instantiate (chipWhite,  GetCoordFromSquare(1, 2), Quaternion.identity) as GameObject;
		gameBoard [2, 2] = Instantiate (chipWhite,  GetCoordFromSquare(2, 2), Quaternion.identity) as GameObject;
		gameBoard [3, 2] = Instantiate (chipWhite,  GetCoordFromSquare(3, 2), Quaternion.identity) as GameObject;
		gameBoard [4, 2] = Instantiate (chipWhite,  GetCoordFromSquare(4, 2), Quaternion.identity) as GameObject;
		gameBoard [5, 2] = Instantiate (chipWhite,  GetCoordFromSquare(5, 2), Quaternion.identity) as GameObject;

		gameBoard [2, 1] = Instantiate (chipBlack,  GetCoordFromSquare(2, 1), Quaternion.identity) as GameObject;
		gameBoard [3, 1] = Instantiate (chipBlack,  GetCoordFromSquare(3, 1), Quaternion.identity) as GameObject;
		gameBoard [4, 1] = Instantiate (chipBlack,  GetCoordFromSquare(4, 1), Quaternion.identity) as GameObject;
		gameBoard [3, 3] = Instantiate (chipBlack,  GetCoordFromSquare(3, 3), Quaternion.identity) as GameObject;
		gameBoard [4, 3] = Instantiate (chipBlack,  GetCoordFromSquare(4, 3), Quaternion.identity) as GameObject;

		gameBoard [0, 5] = Instantiate (chipBlack,  GetCoordFromSquare(0, 5), Quaternion.identity) as GameObject;
		gameBoard [1, 5] = Instantiate (chipBlack,  GetCoordFromSquare(1, 5), Quaternion.identity) as GameObject;
		gameBoard [2, 5] = Instantiate (chipBlack,  GetCoordFromSquare(2, 5), Quaternion.identity) as GameObject;
		gameBoard [3, 5] = Instantiate (chipBlack,  GetCoordFromSquare(3, 5), Quaternion.identity) as GameObject;
		gameBoard [5, 5] = Instantiate (chipBlack,  GetCoordFromSquare(5, 5), Quaternion.identity) as GameObject;
*/
		gameBoard [3, 3] = Instantiate (chipWhite,  GetCoordFromSquare(3, 3), Quaternion.identity) as GameObject;
		gameBoard [4, 4] = Instantiate (chipWhite,  GetCoordFromSquare(4, 4), Quaternion.identity) as GameObject;
		gameBoard [3, 4] = Instantiate (chipBlack,  GetCoordFromSquare(3, 4), Quaternion.identity) as GameObject;
		gameBoard [4, 3] = Instantiate (chipBlack,  GetCoordFromSquare(4, 3), Quaternion.identity) as GameObject;


		//gameBoard [3, 3] = Instantiate (chipWhite, new Vector3(-0.63f, 0.0f, 0.63f) , Quaternion.identity) as GameObject;
		//gameBoard [4, 4] = Instantiate (chipWhite, new Vector3(1,0,1), Quaternion.identity) as GameObject;
		//gameBoard [3, 4] = Instantiate (chipBlack, new Vector3(1,0,-1), Quaternion.identity) as GameObject;
		//gameBoard [4, 3] = Instantiate (chipBlack, new Vector3(-1,0,-1), Quaternion.identity) as GameObject;
	}


	// Returns screen coordinates for given board square
	Vector3 GetCoordFromSquare (int squareX, int squareY)
	{
		float magicScaleNumber = 8.20f;

		return new Vector3 (
			(squareX - 3.5f) * gameboardImage.transform.lossyScale.x / magicScaleNumber,
			0,
			(squareY - 3.5f) * gameboardImage.transform.lossyScale.y / magicScaleNumber
		);
	}


	// Returns board square from given screen coordinates 
	Vector2 GetSquareFromCoord(float coordX, float coordY)
	{
		// Check which square on board click is hitting
		// Following calculation expects that board is on center, and goes from edge-to-edge on portrait-mode
		float squareX = coordX / (Screen.width / 8);
		float squareY = (coordY + Screen.width / 2 - Screen.height / 2) / (Screen.width / 8);

		return new Vector2 ((int)squareX, (int)squareY);
	}


	Vector2 GetSquareFromTransform (Vector3 transformPos)
	{
		// It seems that transform for example for square 0,0 is -5...-3.75
		// the whole board is -5...5
		// formula below normalizes this for 0..10 and then counts 10/8 -> 1.25
		int squareX = (int)((transformPos.x + 5) / 1.25);
		int squareY = (int)((transformPos.z + 5) / 1.25);

		return new Vector2 (squareX, squareY);
	}


	bool IsValidMove (int squareX, int squareY, chipColor color)
	{
		bool isValid = false;

		if (gameBoard [squareX, squareY] != null)
			return false;

		if (CheckDirection (squareX, squareY, -1, -1, color)) {
			isValid = true;
			FlipChips ();
		}
		if (CheckDirection (squareX, squareY, -1,  0, color)) {
			isValid = true;
			FlipChips ();
		}
		if (CheckDirection (squareX, squareY, -1,  1, color)) {
			isValid = true;
			FlipChips ();
		}
		if (CheckDirection (squareX, squareY,  0,  1, color)) {
			isValid = true;
			FlipChips ();
		}
		if (CheckDirection (squareX, squareY,  1,  1, color)) {
			isValid = true;
			FlipChips ();
		}
		if (CheckDirection (squareX, squareY,  1,  0, color)) {
			isValid = true;
			FlipChips ();
		}
		if (CheckDirection (squareX, squareY,  1, -1, color)) {
			isValid = true;
			FlipChips ();
		}
		if (CheckDirection (squareX, squareY,  0, -1, color)) {
			isValid = true;
			FlipChips ();
		}

		return isValid;

		/*
		while (checkX < 2) {
			if (CheckDirection (checkX, checkY, color))
				isValid = true;
			if (checkX == checkY)
				checkX++;
			else
				checkY++;
		}
		*/
	}


	bool CheckDirection (int squareX, int squareY, int deltaX, int deltaY, chipColor color, int count = 0)
	{
		int checkX = squareX + deltaX;
		int checkY = squareY + deltaY;
		// First check whether this check would be out of bounds
		if (checkX < 0 || checkY < 0 || checkX > 7 | checkY > 7)
			return false;

		if (count == 0) {
			if (gameBoard [checkX, checkY] == null || SquareColor (checkX, checkY) == color)
				return false;
			else {
				count++;
				chipsToTurn.Add (gameBoard[checkX, checkY]);
				if (CheckDirection (checkX, checkY, deltaX, deltaY, color, count) == false) {
					chipsToTurn.Clear ();	
					return false;
				}
			}
		} else {
			if (gameBoard [checkX, checkY] == null)
				return false;
			else if (SquareColor (checkX, checkY) != color) {
				count++;
				chipsToTurn.Add (gameBoard[checkX, checkY]);
				if (CheckDirection (checkX, checkY, deltaX, deltaY, color, count) == false) {
					chipsToTurn.Clear ();
					return false;
				}
			} else if (SquareColor (checkX, checkY) == color)
				return true;
			else {
				Debug.Assert (false, "ERROR::IsValidMove: Unexpected condition reached");
			}
		}

		return true;
	}


	// This function expects, that there is chip on checked square!!!
	chipColor SquareColor(int squareX, int squareY)
	{
		Debug.Assert (gameBoard[squareX, squareY] != null, "ERROR::SquareColor: Called with null square");

		if (gameBoard[squareX, squareY].tag == "ChipWhite")
			return chipColor.WHITE;
		else
			return chipColor.BLACK;
	}


	public void PointerDown(float coordX, float coordY)
	{
		Vector2 square = GetSquareFromCoord (coordX, coordY);
		int squareX = (int)square.x;
		int squareY = (int)square.y;

		if (IsValidMove (squareX, squareY, playerColor)) {
			if (playerColor == chipColor.WHITE)
				gameBoard [squareX, squareY] = Instantiate (chipWhite, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			else
				gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			FlipChips ();
			ChangeTurn ();
		
		} else
			chipsToTurn.Clear ();
	}

	void ChangeTurn ()
	{
		if (playerColor == chipColor.WHITE)
			playerColor = chipColor.BLACK;
		else
			playerColor = chipColor.WHITE;
	}

	void FlipChips()
	{
		List<GameObject>.Enumerator e = chipsToTurn.GetEnumerator (); 

		int count = chipsToTurn.Count; // just to see on debugger

		while (e.MoveNext ()) {
			GameObject oldChip = e.Current;
			chipColor oldColor;
			if (oldChip.tag == "ChipWhite")
				oldColor = chipColor.WHITE;
			else
				oldColor = chipColor.BLACK;

			Vector2 square = GetSquareFromTransform (oldChip.transform.position);
			int squareX = (int)square.x;
			int squareY = (int)square.y;

			Destroy (e.Current);

			if (oldColor == chipColor.WHITE)
				gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			else
				gameBoard [squareX, squareY] = Instantiate (chipWhite, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
		}

		chipsToTurn.Clear ();
	}
}
