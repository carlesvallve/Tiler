using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class Slot : MonoBehaviour, IPointerClickHandler {
 
	public void OnPointerClick (PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			Hud.instance.ApplyItem(gameObject);
		
		} else if (eventData.button == PointerEventData.InputButton.Middle) {
			Debug.Log("Middle click");
		
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			Hud.instance.OpenItemInfo(gameObject);
		}
	}
}
