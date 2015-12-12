using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Container : Entity {

	protected string assetType;
	protected int maxItems = 1;

	
	// =====================================================
	// Container Initialization
	// =====================================================

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {

		base.Init(grid, x, y, scale, asset);

		walkable = false;

		SetImages(scale, Vector3.zero, 0.04f);

		state = EntityStates.Closed;

		breakable = true;

		maxItems = 1;
	}

	protected virtual string GetRandomAssetName () {
		return null;
	}


	public override void SetState (EntityStates state) {
		if (breakable) { 
			state = EntityStates.Closed;
		}

		string id = state == EntityStates.Open ? assetType + "-open" : assetType + "-closed";
		SetAsset(Resources.Load<Sprite>("Tilesets/Container/" + id));
		
		this.state = state;
	}


	// =====================================================
	// Container Actions (Open, Unlock)
	// =====================================================

	public IEnumerator Open (Creature creature, bool hasBeenUnlocked = false) {
		if (breakable) {
			creature.AttackToBreak(this);
		}

		yield return new WaitForSeconds(creature.speed / 2);

		sfx.Play("Audio/Sfx/Door/key", 0.5f, Random.Range(0.4f, 0.6f));
		state = EntityStates.Open;

		SetAsset(Resources.Load<Sprite>("Tilesets/Container/"+ assetType + "-open"));

		// spawn all the items contained by the container
		grid.SetEntity(this.x, this.y, null);
		SpawnItemsFromInventory(this.items, breakable);

		if (breakable) {
			StartCoroutine(Break(Color.gray));
		} else {
			// once the container is open, set the tile to unwalkable
			grid.SetEntity(this.x, this.y, this);

			// and log 'opened' message
			if (creature is Player) {
				if (!hasBeenUnlocked) { 
					Hud.instance.Log("You open the " + assetType + "."); 
				}
			}
		}
	}


	public IEnumerator Unlock (Creature creature) {
		yield return new WaitForSeconds(creature.speed / 2);

		bool success = Random.Range(1, 100) < 75;

		if (success) {
			state = EntityStates.Closed;
			sfx.Play("Audio/Sfx/Door/door-open2", 0.7f, Random.Range(0.8f, 1.2f));
			if (creature is Player) { 
				Speak("Success!", Color.white);
				Hud.instance.Log("You unlock the " + assetType + "."); 
			}

		} else {
			sfx.Play("Audio/Sfx/Door/unlock", 0.7f, Random.Range(0.8f, 1.2f));
			if (creature is Player) { 
				Speak("Locked", Color.white);
				Hud.instance.Log("The " + assetType + " is locked.");
			}
		}

		yield return new WaitForSeconds(0.25f);

		if (success) {
			StartCoroutine(Open(creature, true));
		}
	}

	// =====================================================
	// Container Items
	// =====================================================

	public void SetItems () {
		for (int i = 0; i < maxItems; i++) {
			items.Add(CreateRandomItem());
		}
	}


	protected Item CreateRandomItem () {
		// get item type
		System.Type itemType = GetRandomItemType();
		
		// create item in grid but without applying grid.SetEntity
		Item item = (Item)grid.CreateEntity(itemType, 0, 0, 0.8f, null, false) as Item;

		// put item inside the container
		item.transform.SetParent(transform, false);
		item.transform.localPosition = Vector3.zero;
		item.gameObject.SetActive(false);

		return item;
	}


	protected virtual System.Type GetRandomItemType () {
		// Pick a weighted random item type
		return Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
			{ typeof(Armour), 	0 },
			{ typeof(Weapon), 	0 },
			{ typeof(Treasure), 0 },
			{ typeof(Book), 	0 },
			{ typeof(Food), 	0 },
			{ typeof(Potion), 	0 },
		});
	}


	protected override void SpawnItemsFromInventory (List<Item> allItems, bool useCenterTile = true) {
		base.SpawnItemsFromInventory(allItems, useCenterTile);
		items.Clear();
	}

}
