using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Chest : Entity {

	private string assetType;
	

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetImages(scale, Vector3.zero, 0.04f);

		state = EntityStates.Closed;
	}


	public void SetChestAssetType (string assetType) {
		this.assetType = assetType;
		asset = Resources.Load<Sprite>("Tilesets/Chest/" + assetType + "-closed");
		SetAsset(asset);

		breakable = assetType == "chest" ? false : true;
		
	}


	public override void SetState (EntityStates state) {
		if (breakable) { 
			state = EntityStates.Closed;
		} 

		string id = state == EntityStates.Open ? assetType + "-open" : assetType + "-closed";
		SetAsset(Resources.Load<Sprite>("Tilesets/Chest/" + id));
		
		this.state = state;
	}


	public IEnumerator Open (Creature creature, bool hasBeenUnlocked = false) {
		if (breakable) {
			creature.AttackToBreak(this);
		}

		yield return new WaitForSeconds(creature.speed / 2);

		sfx.Play("Audio/Sfx/Door/key", 0.5f, Random.Range(0.4f, 0.6f));
		state = EntityStates.Open;

		SetAsset(Resources.Load<Sprite>("Tilesets/Chest/"+ assetType + "-open"));

		// spawn all the items contained by the chest
		grid.SetEntity(this.x, this.y, null);
		SpawnItemsFromInventory(this.items, breakable);

		if (breakable) {
			StartCoroutine(Break(Color.gray));
		} else {
			// once the chest is open, set the tile to unwalkable
			grid.SetEntity(this.x, this.y, this);

			// and log 'opened' message
			if (creature is Player) {
				if (!hasBeenUnlocked) { 
					Hud.instance.Log("You open the chest."); 
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
				Hud.instance.Log("You unlock the chest."); 
			}

		} else {
			sfx.Play("Audio/Sfx/Door/unlock", 0.7f, Random.Range(0.8f, 1.2f));
			if (creature is Player) { 
				Speak("Locked", Color.white);
				Hud.instance.Log("The chest is locked.");
			}
		}

		yield return new WaitForSeconds(0.25f);

		if (success) {
			StartCoroutine(Open(creature, true));
		}
	}


	public void SetRandomItems (int maxItems) {
		//int maxItems = Random.Range(1, 5);
		for (int i = 0; i < maxItems; i++) {
			items.Add(CreateRandomItem());
		}
	}


	private Item CreateRandomItem () {
		// Pick a random item type
		List<System.Type> types = new List<System.Type>() { 
			typeof(Food), typeof(Treasure), typeof(Potion), typeof(Book), typeof(Weapon), typeof(Armour)
		};
		System.Type itemType = types[Random.Range(0, types.Count)];
		
		// create item in grid but without applying grid.SetEntity
		Item item = (Item)grid.CreateEntity(itemType, 0, 0, 0.8f, null, false) as Item;

		// put item inside the chest
		item.transform.SetParent(transform, false);
		item.transform.localPosition = Vector3.zero;
		item.gameObject.SetActive(false);

		return item;
	}


	protected override void SpawnItemsFromInventory (List<Item> allItems, bool useCenterTile = true) {
		base.SpawnItemsFromInventory(allItems, useCenterTile);
		items.Clear();
	}

}
