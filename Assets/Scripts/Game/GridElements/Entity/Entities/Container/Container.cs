using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Container : Entity {

	protected string assetType;
	protected int maxItems = 1;

	
	// =====================================================
	// Container Initialization
	// =====================================================

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {

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
			creature.combat.AttackToBreak(this);
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
		ItemGenerator generator = new ItemGenerator();
		generator.GenerateInContainer(this, maxItems);
	}


	public virtual System.Type GetRandomItemType () {
		// Pick a weighted random item type
		return Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
			{ typeof(Equipment), 	80 },
			{ typeof(Treasure), 	20 },
			{ typeof(Food), 		10 },
			{ typeof(Potion), 		5 },
			{ typeof(Book), 		3 },
		});
	}

	
	public override void SpawnItemsFromInventory (List<Item> allItems, bool useCenterTile = true) {
		base.SpawnItemsFromInventory(allItems, useCenterTile);
		items.Clear();
	}

}
