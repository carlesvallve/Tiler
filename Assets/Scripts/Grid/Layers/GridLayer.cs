using UnityEngine;
using System.Collections;

public class GridLayer<T> : IGridLayer<T> {
	private T[,] layer;


	public GridLayer(int dx, int dy) {
		layer = new T[dx, dy];
	}

	public T this[int x, int y] {
		get {
			return layer[x,y];
		}
		set {
			layer[x,y] = value;
		}
	}

	public int GetUpperBound(int dimension) {
		return layer.GetUpperBound(dimension);
	}
}
