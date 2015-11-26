using UnityEngine;
using System.Collections;

public class Ladder : Entity {

	public Sprite spriteUp;
	public Sprite spriteDown;

	public LadderTypes type { get; set; }
	public LadderDirections direction { get; set; }


	public void SetDirection (LadderDirections direction) {
		this.direction = direction;

		if (direction == LadderDirections.Up) {
			img.sprite = spriteUp;
			transform.Translate(0, 0.1f, 0);
		} else {
			img.sprite = spriteDown;
			Tile tile = grid.GetTile(x, y);
			tile.gameObject.SetActive(false);
			transform.Translate(0, -0.4f, 0);
			img.material.SetFloat("_MinY", 0.225f);
		}
	}
}
