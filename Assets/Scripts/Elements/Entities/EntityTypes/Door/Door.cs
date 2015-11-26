using UnityEngine;
using System.Collections;

public class Door : Entity {

	public Sprite spriteH;
	public Sprite spriteV;

	public DoorTypes type { get; set; }
	public DoorStates state { get; set; }
	public DoorDirections direction { get; set; }


	public void SetDirection (DoorDirections direction) {
		this.direction = direction;

		if (direction == DoorDirections.Horizontal) {
			img.sprite = spriteH;
		} else {
			img.sprite = spriteV;
			img.transform.Translate(0, 0.2f, 0);
		}
	}


	public void Open () {
		// TODO: In case we want to animate the mask, 
		// in the Update function the animation started flickering, 
		// and thats because the update was being called after the sprite is rendered, 
		// so I had to use the Main Camera OnPreRender event to update the material properties.

		this.state = DoorStates.Open;
		sfx.Play("Audio/Sfx/Door/door-open2", 1f, Random.Range(0.4f, 0.6f));

		Vector3 dir = GetOpendirection();
		img.transform.Translate(dir);
		
		ResizeSpriteMask(dir);
	}


	private Vector3 GetOpendirection () {
		float d ;
		Entity wall = null;
		if (direction == DoorDirections.Horizontal) {
			d = 0.7f;
			// bottom
			wall = grid.GetEntity(x, y - 1);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				return new Vector3(0, -d, 0);
			}
			// top
			wall = grid.GetEntity(x, y + 1);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				return new Vector3(0, d, 0);
			}
		} else {
			d = 0.9f;
			// left
			wall = grid.GetEntity(x - 1, y);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				return new Vector3(-d, 0, 0);
			}
			// right
			wall = grid.GetEntity(x + 1, y);
			if (wall != null && (wall is Obstacle) && ((Obstacle)wall).type == ObstacleTypes.Wall) {
				return new Vector3(d, 0, 0);
			}
		}

		return Vector3.zero;
	}


	private void ResizeSpriteMask (Vector3 dir) {
		if (dir.y > 0) {
			img.material.SetFloat("_MaxY", 0.275f); // moving top
		} else if (dir.y < 0) {
			img.material.SetFloat("_MinY", 0.635f); // moving bottom
		} else if (dir.x > 0) {
			img.material.SetFloat("_MaxX", 0.1f); // moving right
		} else if (dir.x < 0) {
			img.material.SetFloat("_MinX", 0.9f); // moving left
		}
	}


	public void Close () {
		this.state = DoorStates.Closed;
		sfx.Play("Audio/Sfx/Door/door-open2", 1f, Random.Range(0.4f, 0.6f));

		img.transform.Translate(-GetOpendirection());

		img.material.SetFloat("_MinX", 0);
		img.material.SetFloat("_MaxX", 0);
		img.material.SetFloat("_MinY", 0);
		img.material.SetFloat("_MaxY", 0);
	}

	
}
