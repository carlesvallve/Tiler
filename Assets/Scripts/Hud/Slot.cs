using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class Slot : MonoBehaviour, IPointerClickHandler {

	public CreatureInventoryItem invItem;
	public Image image;
	public Text text;

 
	public void Init (CreatureInventoryItem invItem) {
		this.invItem = invItem;
		this.image = transform.Find("Image").GetComponent<Image>();
		this.image.sprite = invItem.sprite;
		this.text = transform.Find("Text").GetComponent<Text>();
		this.text.text = invItem.ammount > 1 ? invItem.ammount.ToString() : "";

		// adjust image aspect on slot
		AdjustSlotAspect();
	}


	private void AdjustSlotAspect () {
		string type = invItem.item.type;
		string subtype = invItem.item.subtype;

		Vector3 scale = new Vector3(0.6f, 0.6f, 1f);
		Vector3 pos = Vector3.zero;

		switch (type) {
			case "Armour":
				scale = new Vector3(1.5f, 1.5f, 1);
				if (subtype == "Robe") { scale = new Vector3(scale.x, scale.y * 0.75f, scale.z); }
				pos = new Vector3(0, 0, 0);
				break;
			case "Weapon":
				scale = new Vector3(1.25f, 1.25f, 1);
				pos = new Vector3(12f, 0, 0);
				break;
			case "Shield":
				scale = new Vector3(1.25f, 1.25f, 1);
				pos = new Vector3(-12f, 0, 0);
				break;
			case "Head":
				scale = new Vector3(1.75f, 1.75f, 1);
				pos = new Vector3(0, -16f, 0);
				break;
		}

		image.transform.localScale = scale; 
		image.transform.localPosition = pos;
	}


	public void OnPointerClick (PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			Hud.instance.ApplyItem(this);
		
		} else if (eventData.button == PointerEventData.InputButton.Middle) {
			Debug.Log("Middle click");
		
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			Hud.instance.OpenItemInfo(this);
		}
	}
}
