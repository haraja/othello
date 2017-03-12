using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Touchpad : MonoBehaviour, IPointerDownHandler {

	public GameController gameController;
	
	public void OnPointerDown (PointerEventData data)
	{
		Debug.Log ("PointerDown: " + data.position);

		gameController.PointerDown (data.position.x, data.position.y);

		//Vector2 posLoc = new Vector2 (posX, posY);
		//Debug.Log ("posLoc: " + posLoc);

		//Vector2 coord = new Vector2 (data.position.x, data.position.y);
		//Debug.Log ("coord: " + coord);

		//gameController.PointerDown ((int)posX, (int)posY);
	}
}
