using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Entity {

	public PlayerTypes type { get; set; }

	public override void Init (Grid grid, int x, int y, Color color) {
		base.Init(grid, x, y, color);
		
 		Camera2D.target = this.gameObject;
	}
}
