using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;


public enum ChipColor {WHITE, BLACK};
public enum CompStrategy {RANDOM, GREEDY, CALCULATING1};
public enum Player {HUMAN, COMPUTER};
//public enum GameMode {SINGLEPLAYER, MULTIPLAYER};


public class GameController : MonoBehaviour {

	public Text countWhiteText;
	public Text countBlackText;
	public Text Debug1Text;
	public GameObject bigButton;
	public float computerTurnWait;
	public float chipTurnWait; 
	public GameObject gameboardImage;
	public GameObject chipBlack;
	public GameObject chipWhite;
	//public Player opponent = Player.COMPUTER;
	public CompStrategy compStrategy = CompStrategy.RANDOM;

	Player opponent;
	ChipColor currentPlayer = ChipColor.WHITE;	// white always starts
	CompPlayer compPlayer;
	GameObject[,] gameBoard = new GameObject[8, 8];
	bool doneFlipping = false;

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
/*
	public void SetOpponent(Player newOpponent)
	{
		opponent = newOpponent;
	}
*/

	void OnEnable ()
	{
		opponent = (Player)PlayerPrefs.GetInt ("opponent");
	}


	void Start () 
	{
		compPlayer = gameObject.AddComponent<CompPlayer> ();

		InitBoard ();
		UpdateUI ();
	}


	void InitBoard()
	{
		for (int i=0; i<8; i++)
			for (int j = 0; j < 8; j++)
				gameBoard[i, j] = null;
			
		gameBoard [3, 3] = Instantiate (chipWhite,  GetCoordFromSquare(3, 3), Quaternion.identity) as GameObject;
		gameBoard [4, 4] = Instantiate (chipWhite,  GetCoordFromSquare(4, 4), Quaternion.identity) as GameObject;
		gameBoard [3, 4] = Instantiate (chipBlack,  GetCoordFromSquare(3, 4), Quaternion.identity) as GameObject;
		gameBoard [4, 3] = Instantiate (chipBlack,  GetCoordFromSquare(4, 3), Quaternion.identity) as GameObject;
	}


	void UpdateUI ()
	{
		countWhiteText.text = CountChips (ChipColor.WHITE).ToString ();
		countBlackText.text = CountChips (ChipColor.BLACK).ToString ();

		if (currentPlayer == ChipColor.WHITE)
			Debug1Text.text = "CurrentPlayer: WHITE"; 
		else
			Debug1Text.text = "CurrentPlayer: BLACK"; 
	}


	int CountChips (ChipColor color)
	{
		int count = 0;

		for (int i = 0; i < 8; i++)
			for (int j = 0; j < 8; j++)
				if (gameBoard [i, j] != null && 
					((color == ChipColor.WHITE && gameBoard [i, j].tag == "ChipWhite") ||
					 (color == ChipColor.BLACK && gameBoard [i, j].tag == "ChipBlack")))
					count++;

		if (count < 1 && count > 65)
			Debug.LogError ("CountChips::Error");
		//Assert.IsTrue (count > 1 && count < 65);

		return count;
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

		Assert.IsTrue (squareX >= 0 && squareX < 8 && squareY >= 0 && squareY < 8, "X:Y: " + squareX + ":" + squareY);

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

		Assert.IsTrue (squareX >= 0 && squareX < 8 && squareY >= 0 && squareY < 8);

		return new Vector2 (squareX, squareY);
	}


	// This function expects, that there is chip on checked square!!!
	ChipColor SquareColor(int squareX, int squareY)
	{
		Assert.IsNotNull (gameBoard[squareX, squareY]);

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

		if (IsValidMove (squareX, squareY, currentPlayer) > 0) {
			if (currentPlayer == ChipColor.WHITE) {
				gameBoard [squareX, squareY] = Instantiate (chipWhite, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			} else {
				gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			}
			//UpdateUI ();
			StartCoroutine (FlipChips ());

		}
	}


	// returns number of chips, which position would be gaining. 0 means this is not valid move
	public int IsValidMove (int squareX, int squareY, ChipColor color)
	{
		chipsToFlip.Clear ();

		if (gameBoard [squareX, squareY] != null)
			return 0;

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, -1, -1, color)) {
			AddChipsToFlip ();
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, -1, 0, color)) {
			AddChipsToFlip ();
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, -1, 1, color)) {
			AddChipsToFlip ();
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 0, 1, color)) {
			AddChipsToFlip ();
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 1, 1, color)) {
			AddChipsToFlip ();
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 1, 0, color)) {
			AddChipsToFlip ();
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 1, -1, color)) {
			AddChipsToFlip ();
		}

		chipsOnDirection.Clear ();
		if (CheckDirection (squareX, squareY, 0, -1, color)) {
			AddChipsToFlip ();
		}

		return chipsToFlip.Count;
	}


	bool CheckDirection (int squareX, int squareY, int deltaX, int deltaY, ChipColor color, int count = 0)
	{
		int checkX = squareX + deltaX;
		int checkY = squareY + deltaY;

		//TODO: Check if following statement should be possible
		if (checkX < 0 || checkY < 0 || checkX > 7 | checkY > 7) {
			//Debug.LogError ("ERROR::CheckDirection: Out of bounds selected, although should not be possible");
			return false;
		}

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


	List<Vector2> FindValidMoves (ChipColor color)
	{
		List<Vector2> validMoves = new List<Vector2>();

		for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
				//if (IsValidMove (x, y, ChipColor.BLACK) > 0)	// XXX
				if (IsValidMove (x, y, color) > 0)
					validMoves.Add (new Vector2 (x, y));
			}
		}

		return validMoves;
	}


	void AddChipsToFlip ()
	{
		List<GameObject>.Enumerator e = chipsOnDirection.GetEnumerator (); 
		while (e.MoveNext ())
			chipsToFlip.Add (e.Current);
	}


	void ComputerPlay ()
	{
		chipsToFlip.Clear ();

		Vector2? proposedMove = compPlayer.ProposeMove (gameBoard, compStrategy);
		if (proposedMove != null) {
			int squareX = (int)proposedMove.Value.x;
			int squareY = (int)proposedMove.Value.y;
			int count = IsValidMove (squareX, squareY, ChipColor.BLACK); // validity is already know, but this also flips needed squares
			Assert.IsTrue (count > 0);

			gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			//UpdateUI ();
		}

		StartCoroutine (FlipChips ());
	}


	IEnumerator FlipChips ()
	{
		// Flip chips
		//List<GameObject>.Enumerator e = chipsToFlip.GetEnumerator (); 
		//Debug.Log ("chipsToFlip Count: " + chipsToFlip.Count);
		//while (e.MoveNext ()) {
		if (chipsToFlip.Count != 0) {
			for (int i = 0; i < chipsToFlip.Count; i++) {

				yield return new WaitForSeconds (chipTurnWait);

				if (chipsToFlip [i] == null)
					Debug.LogError ("ERROR::FlipChips: list position is empty, i: " + i.ToString ());

				Vector2 square = GetSquareFromTransform (chipsToFlip [i].transform.position);

				int squareX = (int)square.x;
				int squareY = (int)square.y;

				//Destroy (e.Current);
				Destroy (chipsToFlip [i]);

				if (currentPlayer == ChipColor.BLACK)
					gameBoard [squareX, squareY] = Instantiate (chipBlack, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
				else
					gameBoard [squareX, squareY] = Instantiate (chipWhite, GetCoordFromSquare (squareX, squareY), Quaternion.identity) as GameObject;
			}
		}

		doneFlipping = true;
	}


	IEnumerator ChangePlayer ()
	{
		doneFlipping = false;
		chipsToFlip.Clear ();

		//yield return new WaitForSeconds (computerTurnWait);

		// UpdateUI ();

		ChipColor nextPlayer;
		if (currentPlayer == ChipColor.WHITE) 
			nextPlayer = ChipColor.BLACK;
		else
			nextPlayer = ChipColor.WHITE;

		//yield return new WaitForSeconds (computerTurnWait);

		// if opposite player has no valid moved available, don't change turn 
		if (FindValidMoves (nextPlayer).Count > 0) {
			if (currentPlayer == ChipColor.WHITE) {
				currentPlayer = nextPlayer;
				bigButton.gameObject.GetComponent<Renderer> ().material.color = Color.black;
				UpdateUI ();

				if (opponent == Player.COMPUTER) {
					yield return new WaitForSeconds (computerTurnWait);
					ComputerPlay ();
				}
			} else {
				currentPlayer = nextPlayer;
				bigButton.gameObject.GetComponent<Renderer> ().material.color = Color.white;
				UpdateUI ();
				yield return null;
			}
		} else {
			Debug.Log ("ChangePlayer: No moves");
			if (currentPlayer == ChipColor.BLACK && opponent == Player.COMPUTER) {
				yield return new WaitForSeconds (computerTurnWait);
				ComputerPlay ();
			}
		}
	}
}
