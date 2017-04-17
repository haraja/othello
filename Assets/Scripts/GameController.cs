using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum ChipColor {WHITE, BLACK};
public enum CompStrategy {RANDOM};


public class GameController : MonoBehaviour {

	public Text countWhiteText;
	public Text countBlackText;
	int countWhite;
	int countBlack; 
	bool doneFlipping = false;

	//TODO: this is not needed if found a bug of having several pointer events
	//bool lockPointer = false;

	public GameObject bigButton;
	public enum Player {HUMAN, COMPUTER};
	public float computerTurnWait;
	public float chipTurnWait; 
	public GameObject gameboardImage;
	public GameObject chipBlack;
	public GameObject chipWhite;
	//public ChipColor player1Color = ChipColor.WHITE; 		// initially player1 is hardcoded to play with white
	public Player opponent = Player.HUMAN;
	ChipColor currentPlayer = ChipColor.WHITE;				// white always starts

	CompPlayer compPlayer;

	GameObject[,] gameBoard = new GameObject[8, 8];
	public CompStrategy compStrategy = CompStrategy.RANDOM;

	// used to store chips, which potentially will be turned. Aggregates all directions
	List<GameObject> chipsToFlip = new List<GameObject>();	
	// used to check one direction, whether move is valid
	List<GameObject> chipsOnDirection = new List<GameObject>();	


	/*
	public void StartGame ()
	{
		Debug.Log ("GameController::StartGame");

	}
	*/

	void Start () 
	{
		compPlayer = gameObject.AddComponent<CompPlayer> ();

		InitBoard ();
		UpdateUI ();
	}


	void InitBoard()
	{
		for (int i=0; i<8; i++){
			for (int j = 0; j < 8; j++) {
				gameBoard[i, j] = null;
			}
		}
			
		gameBoard [3, 3] = Instantiate (chipWhite,  GetCoordFromSquare(3, 3), Quaternion.identity) as GameObject;
		gameBoard [4, 4] = Instantiate (chipWhite,  GetCoordFromSquare(4, 4), Quaternion.identity) as GameObject;
		gameBoard [3, 4] = Instantiate (chipBlack,  GetCoordFromSquare(3, 4), Quaternion.identity) as GameObject;
		gameBoard [4, 3] = Instantiate (chipBlack,  GetCoordFromSquare(4, 3), Quaternion.identity) as GameObject;
		//gameBoard [2, 4] = Instantiate (chipBlack,  GetCoordFromSquare(2, 4), Quaternion.identity) as GameObject;
		//gameBoard [1, 4] = Instantiate (chipBlack,  GetCoordFromSquare(1, 4), Quaternion.identity) as GameObject;

		countWhite = 2;
		countBlack = 2;
}


	void UpdateUI ()
	{
		countWhiteText.text = countWhite.ToString ();
		countBlackText.text = countBlack.ToString ();

	}


	void Update ()
	{
		if (doneFlipping)
			StartCoroutine (ChangePlayer ());	
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


	// Used for getting square-position on board for the given chips's transform 
	Vector2 GetSquareFromTransform (Vector3 transformPos)
	{
		// It seems that transform for example for square 0,0 is -5...-3.75
		// the whole board is -5...5
		// formula below normalizes this for 0..10 and then counts 10/8 -> 1.25
		int squareX = (int)((transformPos.x + 5) / 1.25);
		int squareY = (int)((transformPos.z + 5) / 1.25);

		return new Vector2 (squareX, squareY);
	}


	// This function expects, that there is chip on checked square!!!
	ChipColor SquareColor(int squareX, int squareY)
	{
		Debug.Assert (gameBoard[squareX, squareY] != null, "ERROR::SquareColor: Called with null square");

		if (gameBoard[squareX, squareY].tag == "ChipWhite")
			return ChipColor.WHITE;
		else
			return ChipColor.BLACK;
	}


	public void PointerDown(float coordX, float coordY)
	{
		if (currentPlayer == ChipColor.BLACK && opponent == Player.COMPUTER)
			return;

		Vector2 square = GetSquareFromCoord (coordX, coordY);
		int squareX = (int)square.x;
		int squareY = (int)square.y;

		if (IsValidMove (squareX, squareY, currentPlayer)) {
			if (currentPlayer == ChipColor.WHITE) {
				gameBoard [squareX, squareY] = Instantiate (chipWhite, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
				countWhite++;
			} else {
				gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
				countBlack++;
			}
			UpdateUI ();
			StartCoroutine (ChangeTurn ());

		}
	}


	// checkAndFlip parameter defines, whether position is checked and flipped, or only checked
	public bool IsValidMove (int squareX, int squareY, ChipColor color)
	{
		bool isValid = false;
		chipsToFlip.Clear ();

		if (gameBoard [squareX, squareY] != null)
			return false;

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, -1, -1, color)) {
			AddChipsToFlip ();
			isValid = true;
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, -1, 0, color)) {
			AddChipsToFlip ();
			isValid = true;
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, -1, 1, color)) {
			AddChipsToFlip ();
			isValid = true;
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 0, 1, color)) {
			AddChipsToFlip ();
			isValid = true;
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 1, 1, color)) {
			AddChipsToFlip ();
			isValid = true;
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 1, 0, color)) {
			AddChipsToFlip ();
			isValid = true;
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 1, -1, color)) {
			AddChipsToFlip ();
			isValid = true;
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 0, -1, color)) {
			AddChipsToFlip ();
			isValid = true;
		}
		/*
		if (isValid ) {
			if (currentPlayer == ChipColor.WHITE)
				countWhite++;
			else
				countBlack++;
		}
		*/

		UpdateUI ();
		return isValid;
	}


	bool CheckDirection (int squareX, int squareY, int deltaX, int deltaY, ChipColor color, int count = 0)
	{
		int checkX = squareX + deltaX;
		int checkY = squareY + deltaY;

		//TODO: following not possible, as touch area is only on top of the board
		// First check whether this check would be out of bounds
		if (checkX < 0 || checkY < 0 || checkX > 7 | checkY > 7)
			return false;

		if (count == 0) {
			if (gameBoard [checkX, checkY] == null || SquareColor (checkX, checkY) == color)
				return false;
			else {
				count++;
				chipsOnDirection.Add (gameBoard [checkX, checkY]);
				if (CheckDirection (checkX, checkY, deltaX, deltaY, color, count) == false) {
					//chipsToFlip.Clear ();	
					return false;
				}
			}
		} 
		else {
			if (gameBoard [checkX, checkY] == null)
				return false;
			if (SquareColor (checkX, checkY) != color) {
				count++;
				chipsOnDirection.Add (gameBoard [checkX, checkY]);
				if (CheckDirection (checkX, checkY, deltaX, deltaY, color, count) == false) {
					//chipsToFlip.Clear ();
					return false;
				}
			} 
			else
				return true;
		}
			
		return true;
	}


	void AddChipsToFlip ()
	{
		List<GameObject>.Enumerator e = chipsOnDirection.GetEnumerator (); 
		while (e.MoveNext ())
			chipsToFlip.Add (e.Current);
	}


	void ChangeToComputer ()
	{
		chipsToFlip.Clear ();

		Vector2? proposedMove = compPlayer.ProposeMove (gameBoard, compStrategy);
		if (proposedMove != null) {
			int squareX = (int)proposedMove.Value.x;
			int squareY = (int)proposedMove.Value.y;
			if (!IsValidMove (squareX, squareY, ChipColor.BLACK)) // validity is already know, but this also flips needed squares
				Debug.LogAssertion ("ERROR::ChangeTurn: Move not valid, although validated earlier");
			gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			countBlack++;
			UpdateUI ();
		}

		ChangeTurn ();
	}


	IEnumerator ChangeTurn ()
	{
		// Flip chips
		//List<GameObject>.Enumerator e = chipsToFlip.GetEnumerator (); 
		Debug.Log ("chipsToFlip Count: " + chipsToFlip.Count);
		//while (e.MoveNext ()) {
		Debug.Log ("ChangeTurn::Time1: " + Time.time);
		if (chipsToFlip.Count != 0) {
			for (int i = 0; i < chipsToFlip.Count; i++) {

				Debug.Log ("ChangeTurn::Time2: " + Time.time);

				yield return new WaitForSeconds (chipTurnWait);

				Debug.Log ("ChangeTurn::Time3: " + Time.time);

				//Vector2 square = GetSquareFromTransform (e.Current.transform.position);

				if (chipsToFlip [i] == null)
					Debug.LogAssertion ("ERROR::ChangeTurn: list position is empty, i: " + i.ToString ());

				Vector2 square = GetSquareFromTransform (chipsToFlip [i].transform.position);

				int squareX = (int)square.x;
				int squareY = (int)square.y;

				//Destroy (e.Current);
				Destroy (chipsToFlip [i]);

				if (currentPlayer == ChipColor.BLACK) {
					gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
					countBlack++;
					countWhite--;
				} else {
					gameBoard [squareX, squareY] = Instantiate (chipWhite, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
					countWhite++;
					countBlack--;
				}
			}
		}

		UpdateUI ();
		doneFlipping = true;
	}


	IEnumerator ChangePlayer ()
	{
		Debug.Log ("ChangePlayer::Time: " + Time.time);

		doneFlipping = false;
		// Change the color of BigButton, and the player turn
		if (currentPlayer == ChipColor.WHITE) {
			currentPlayer = ChipColor.BLACK;
			if (opponent == Player.COMPUTER) {
				bigButton.gameObject.GetComponent<Renderer> ().material.color = Color.black;

				yield return new WaitForSeconds (computerTurnWait);
				ChangeToComputer ();
				yield break;
			}
		} else {
			bigButton.gameObject.GetComponent<Renderer> ().material.color = Color.white;
			currentPlayer = ChipColor.WHITE;
		}

		chipsToFlip.Clear ();
	}
}
