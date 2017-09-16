using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour, IPointerDownHandler {

	public GameController gameController;

	// Use this for initialization
	void Start () {
/*		// get a reference for GameController
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent<GameController> ();
		if (gameController == null)
			Debug.Log ("AsteroidMover::Cannot Find GameController");
*/	}
	

	public void OnPointerDown (PointerEventData data)
	{
//		GameData.initByMenu = true;

		if (data.selectedObject.tag == "MenuStartSingle")
			PlayerPrefs.SetInt("opponent", (int)Player.COMPUTER);
		else
			PlayerPrefs.SetInt("opponent", (int)Player.HUMAN);


		SceneManager.LoadScene("Othello");
	}
}
