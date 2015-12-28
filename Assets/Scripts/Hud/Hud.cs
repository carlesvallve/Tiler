using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Hud : MonoSingleton <Hud> {

	protected AudioManager sfx;

	public GameObject labelPrefab;
	public GameObject slotPrefab;
	public float textSpeed = 0.025f;

	private Text playerName;
	private Text playerInfo;
	private Text dungeonLevel;
	private Text gameTurn;

	private Text logText;
	private string lastLog;

	private CanvasGroup overlayGroup;

	private Transform world;
	
	private Transform popupInventory;
	private Transform inventoryItems;
	private Transform inventoryInfo;

	private List<Slot> inventorySlots;
	

	void Awake () {

		// audio
		sfx = AudioManager.instance;

		// hud canvas
		Canvas canvas = GetComponent<Canvas>();
		canvas.sortingLayerName = "Ui";
		canvas.sortingOrder = short.MaxValue;

		// header
		playerName = transform.Find("Header/PlayerName/Text").GetComponent<Text>();
		playerInfo = transform.Find("Header/PlayerXp/Text").GetComponent<Text>();
		dungeonLevel = transform.Find("Header/DungeonLevel/Text").GetComponent<Text>();
		gameTurn = transform.Find("Header/Turn/Text").GetComponent<Text>();

		// footer
		logText = transform.Find("Footer/Log/Text").GetComponent<Text>();

		// inventory
		popupInventory = transform.Find("Popups/PopupInventory");
		popupInventory.gameObject.SetActive(false);

		inventoryItems = popupInventory.Find("Main/Bag/Inventory");
		inventoryItems.gameObject.SetActive(true);

		inventoryInfo = popupInventory.Find("Main/Bag/Info");
		inventoryInfo.gameObject.SetActive(false);

		

		// in-game labels
		overlayGroup = transform.Find("Overlay").GetComponent<CanvasGroup>();
		world = transform.Find("World");
	}


	void Update () {
		if (Input.GetKeyDown(KeyCode.I)) {
			bool value = !popupInventory.gameObject.activeSelf;
			DisplayInventory(value);
			if (value) { sfx.Play("Audio/Sfx/Item/book", 0.15f, Random.Range(0.8f, 1.2f)); }
		}

		if (Input.GetKeyDown(KeyCode.Q)) {
			Navigator.instance.Open("Game");
		}

	}

	// ==============================================================
	// Inventory
	// ==============================================================

	public void DisplayInventory (bool value) {
		// display popup, and escape if it was closed
		popupInventory.gameObject.SetActive(value);
		
		if (!value) {
			if (inventoryInfo.gameObject.activeSelf) {
				CloseItemInfo();
			}  else {
				sfx.Play("Audio/Sfx/Item/book", 0.15f, Random.Range(0.8f, 1.2f));
			}
			return;
		}

		// destroy all inventory slots
		Transform inventoryContainer = transform.Find("Popups/PopupInventory/Main/Bag/Inventory/Container/Slots");
		foreach (Transform child in inventoryContainer) {
			Destroy(child.gameObject);
		}

		// destroy all equipment slots
		Transform equipmentContainer = transform.Find("Popups/PopupInventory/Main/Bag/Equipment/Slots/");
		foreach (Transform category in equipmentContainer) {
			foreach (Transform child in category) {
				if (child != null) { Destroy(child.gameObject); }
			}
		}

		// generate inventory slots from player's CreatureInventory items
		inventorySlots = new List<Slot>();
		foreach(CreatureInventoryItem invItem in Grid.instance.player.inventoryModule.items) {
			if (invItem.equipped) {
				// equipment slots
				Transform container = equipmentContainer.Find(invItem.item.equipmentSlot);
				CreateInventorySlot(container, invItem);
				
				// display 2 handed weapons in bot weapon and shield slots
				if (invItem.item.equipmentSlot == "Weapon" && ((Equipment)invItem.item).hands == 2) {
					Transform shieldContainer = equipmentContainer.Find("Shield");
					CreateInventorySlot(shieldContainer, invItem);
				}
			} else {
				// inventory slots
				inventorySlots.Add(CreateInventorySlot(inventoryContainer, invItem));
			}
		}
	}


	private Slot CreateInventorySlot (Transform parent, CreatureInventoryItem invItem) {
		// instantiate slot prefab
		GameObject obj = (GameObject)Instantiate(slotPrefab);
		obj.transform.SetParent(parent, false);
		obj.transform.localPosition = Vector3.zero;
		obj.name = invItem.sprite.name;

		Slot slot = obj.GetComponent<Slot>();
		slot.Init(invItem);

		return slot;
	}

	
	public void ApplyItem (Slot slot) {
		string id = slot.name;
		CreatureInventoryItem invItem = slot.invItem; //Grid.instance.player.inventoryModule.GetInventoryItemById(id);

		if (invItem == null) {
			Debug.LogError("No item was found by id: " + id);
			return;
		}

		// use item
		if (invItem.item.consumable) {
			Grid.instance.player.inventoryModule.UseItem(invItem);
			DisplayInventory(false);
			return;
		}

		// equip/unequip item
		if (invItem.item.equipmentSlot != null) {
			// equip/unequip given equipment item
			Grid.instance.player.inventoryModule.EquipItem(invItem);
			invItem.item.PlaySoundUse();
			DisplayInventory(true);
			return;
		}
	}


	public void OpenItemInfo (Slot slot) {
		Item item = slot.invItem.item;

		Image image = inventoryInfo.Find("Container/Image/Image").GetComponent<Image>();
		image.sprite = item.asset;

		image.transform.localScale = slot.image.transform.localScale;
		image.transform.localPosition = slot.image.transform.localPosition + new Vector3(24, -24, 0);

		inventoryInfo.Find("Container/Name").GetComponent<Text>().text = slot.name;
		Text info = inventoryInfo.Find("Container/Stats").GetComponent<Text>();

		info.text = 
		item.type + " | " + item.subtype + " | " +
		"rarity: " + item.rarity + "\n\n";

		if (item is Equipment) {
			Equipment equipment = (Equipment)item;
			info.text +=
			"attack: " + equipment.attack + "\n" +
			"defense: " + equipment.defense + "\n" +
			"damage: " + equipment.damage + "\n" +
			"armour: " + equipment.armour + "\n" +
			"range: " + equipment.range + "\n" +
			"hands: " + equipment.hands + "\n" +
			"weight: " + equipment.weight + "\n";
		}


		inventoryItems.gameObject.SetActive(false);
		inventoryInfo.gameObject.SetActive(true);
		sfx.Play("Audio/Sfx/Item/book", 0.15f, Random.Range(0.8f, 1.2f));
	}


	public void CloseItemInfo () {
		inventoryInfo.gameObject.SetActive(false);
		inventoryItems.gameObject.SetActive(true);
		sfx.Play("Audio/Sfx/Item/book", 0.15f, Random.Range(0.8f, 1.2f));
	}


	// ==============================================================
	// Header
	// ==============================================================

	public void LogPlayerName (string str) {
		playerName.text = str;
	}

	public void LogPlayerInfo (string str) {
		playerInfo.text = str;
	}

	public void LogTurn (string str) {
		gameTurn.text = str;
	}


	public void LogDungeon (string str) {
		dungeonLevel.text = str;
	}


	// ==============================================================
	// footer
	// ==============================================================

	public void Log (string str) {
		if (logText == null) { return; }
		if (str == lastLog) { return; }
		if (str == "") {
			logText.text = str;
			return;
		}

		WriteText(logText, str, textSpeed, false);

		lastLog = str;
	}


	// ==============================================================
	// Text Animation
	// ==============================================================

	public void WriteText (Text dialogText, string str, float speed, bool preRender = false) {
		StartCoroutine(AnimateText(dialogText, str, speed));
	}	


	private IEnumerator AnimateText (Text dialogText, string str, float speed, bool preRender = false) {
		//print ("Animating text " + dialogText + " " +  str + " " + speed);

		if (preRender) { 
			dialogText.text = Invisible(str); 
		}

		float textTime = Time.time;
		int progress = 0;

		while (progress < str.Length) {
			while (textTime <= Time.time && progress < str.Length) {
				textTime = textTime + speed;
				progress++;

				dialogText.text = str.Substring(0, progress);
				if (preRender) {
					str += Invisible(str.Substring(progress));
				}
			}

			// if user interacts while writting the text, escape so all text is written instantly
			/*if (interactive && userInteracted == true) {
				break;
			}*/

			yield return null;
		}

		dialogText.text = str;
		lastLog = null;
	}


	private string Invisible (string raw) {
		return "<color=#00000000>" + raw + "</color>";
	}


	// ==============================================================
	// Overlay Fade In/Out
	// ==============================================================

	public IEnumerator FadeIn(float duration, float delay = 0) {
		CanvasGroup group = overlayGroup;

		yield return new WaitForSeconds(delay);

		float elapsedTime = 0;
		while (elapsedTime < duration) {
			float t = elapsedTime / duration;
			group.alpha = Mathf.Lerp(1, 0, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;
	}


	public IEnumerator FadeOut(float duration, float delay = 0) {
		CanvasGroup group = overlayGroup;
		group.interactable = true;
		group.blocksRaycasts = true;

		yield return new WaitForSeconds(delay);
		
		float elapsedTime = 0;
		while (elapsedTime < duration) {
			float t = elapsedTime / duration;
			group.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = 1;
	}


	// ==============================================================
	// UI labels
	// ==============================================================

	// TODO:  We probably want to create a 'Label' class to handle all these
	
	public void CreateLabel (Tile tile, string str, Color color, float delay = 0, bool stick = false, float duration = 1f, float startY = 24) {
		GameObject obj = (GameObject)Instantiate(labelPrefab);
		obj.transform.SetParent(world, false);
		obj.name = "Label";

		Text text = obj.transform.Find("Text").GetComponent<Text>();
		text.color = color;
		text.text = str;

		obj.SetActive(false);

		StartCoroutine(AnimateLabel(tile, obj, stick, duration, delay, startY));
		StartCoroutine(FadeLabel(tile, obj, duration, delay));
	} 

	
	private IEnumerator AnimateLabel(Tile tile, GameObject obj, bool stick, float duration, float delay, float startY) {
		yield return new WaitForSeconds(delay);
		//if (tile == null) { yield break; }

		obj.SetActive(true);

		// we need to use position because tile does not update coords once reparented to a creature
		Vector3 startPos = tile.transform.position; // new Vector3(tile.x, tile.y, 0); //
		float endY = startY + 32;
		float t = 0;
		
		while (t <= 1) {
			t += Time.deltaTime / duration;

			float y = Mathf.Lerp(startY, endY, Mathf.SmoothStep(0f, 1f, t));

			Vector3 pos = Camera.main.WorldToScreenPoint(
				tile != null && stick ? tile.transform.position : startPos
			) + Vector3.up * y;

			if (obj != null) { obj.transform.position = pos; } 
			
			yield return null;
		}
	}


	private IEnumerator FadeLabel(Tile tile, GameObject obj, float duration, float delay) {
		yield return new WaitForSeconds(delay);
		//if (tile == null) { yield break; }

		obj.SetActive(true);

		CanvasGroup group = obj.GetComponent<CanvasGroup>();

		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime / (duration * 0.2f);
			group.alpha = t;
			yield return null;
		}

		yield return new WaitForSeconds(duration * 0.4f);

		t = 0;
		while (t <= 1) {
			t += Time.deltaTime / (duration * 0.4f);
			group.alpha = (1 - t);
			yield return null;
		}

		yield return null;


		Destroy(obj);
	}
}
