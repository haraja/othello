using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Touchpad : MonoBehaviour, IPointerDownHandler {

	public GameController gameController;
	
	public void OnPointerDown (PointerEventData data)
	{
		//Debug.Log ("PointerDown: " + data.position);
		gameController.PointerDown (data.position.x, data.position.y);
	}
}
