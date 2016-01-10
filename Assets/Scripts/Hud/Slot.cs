using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class Slot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public CreatureInventoryItem invItem;
	public Image image;
	public Text text;


	private bool longPress = false;
	private float longPressDuration = 0.25f;
	private float tapStartTime = 0;
	private float timeDelta = 0;

	private bool isDown = false;

 
	public void Init (CreatureInventoryItem invItem) {
		this.invItem = invItem;
		this.image = transform.Find("Image").GetComponent<Image>();
		this.image.sprite = invItem.sprite;
		this.text = transform.Find("Text").GetComponent<Text>();
		this.text.text = invItem.ammount > 1 ? invItem.ammount.ToString() : "";

		transform.localPosition = Vector3.zero;

		// adjust image aspect on slot
		AdjustSlotAspect();
	}


	private void AdjustSlotAspect () {
		string type = invItem.item.type;
		string subtype = invItem.item.subtype;

		Vector3 scale = new Vector3(0.6f, 0.6f, 1f);
		Vector3 pos = Vector3.zero - Vector3.up * 32;

		switch (type) {
			case "Armour":
				scale *= 2.5f;
				if (subtype == "Robe") { 
					scale = new Vector3(scale.x, scale.y * 0.75f, scale.z);
				}
				pos += new Vector3(0, -4f, 0);
				break;
			case "Weapon":
				scale *= 2;
				pos += new Vector3(16f, 0, 0);
				break;
			case "Shield":
				scale *= 2;
				pos += new Vector3(-16f, 0, 0);
				break;
			case "Head":
				scale *= 3;
				pos += new Vector3(0, -28f, 0);
				break;
		}

		image.transform.localScale = scale;
		image.transform.localPosition = pos;
	}


	/*public void OnPointerClick (PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			Hud.instance.ApplyItem(this);
		
		} else if (eventData.button == PointerEventData.InputButton.Middle) {
			Debug.Log("Middle click");
		
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			Hud.instance.OpenItemInfo(this);
		}
	}*/


	public void OnPointerDown (PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			Debug.Log("Tap down on slot");
			tapStartTime = Time.time;
			//Hud.instance.ApplyItem(this);
		
		}/* else if (eventData.button == PointerEventData.InputButton.Middle) {
			Debug.Log("Middle click");
		
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			Hud.instance.OpenItemInfo(this);
		}*/
	}


	public void OnPointerUp (PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			Debug.Log("Tap up on slot");
			if (!longPress) {
				Hud.instance.ApplyItem(this);
			}
			longPress = false;
		
		}/* else if (eventData.button == PointerEventData.InputButton.Middle) {
			Debug.Log("Middle click");
		
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			Hud.instance.OpenItemInfo(this);
		}*/
	}


	void Update () {
		if (!isDown) { return; }

		timeDelta = Time.time - tapStartTime;
		if (longPress == false && timeDelta >= longPressDuration) {
			Hud.instance.OpenItemInfo(this);
			longPress = true;
		}

	}

}
