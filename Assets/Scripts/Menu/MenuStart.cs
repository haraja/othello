using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour, IPointerDownHandler {

	public GameController gameController;

	// Use this for initialization
	void Start () 
	{
	}
	

	public void OnPointerDown (PointerEventData data)
	{
//		GameData.initByMenu = true;

		if (data.selectedObject.tag == "MenuStartSingle_mode1") {
			PlayerPrefs.SetInt ("opponent", (int)Player.COMPUTER);
			PlayerPrefs.SetInt ("strategy", (int)CompStrategy.RANDOM);
		}
		else if (data.selectedObject.tag == "MenuStartSingle_mode2") {
			PlayerPrefs.SetInt ("opponent", (int)Player.COMPUTER);
			PlayerPrefs.SetInt ("strategy", (int)CompStrategy.GREEDY);
		} 
		else if (data.selectedObject.tag == "MenuStartSingle_mode3") {
			PlayerPrefs.SetInt ("opponent", (int)Player.COMPUTER);
			PlayerPrefs.SetInt ("strategy", (int)CompStrategy.CALCULATING1);
		} 
		else {
			PlayerPrefs.SetInt ("opponent", (int)Player.HUMAN);
		}
		SceneManager.LoadScene("Othello");
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
