using UnityEngine;
using System.Collections;

public interface IGridLayer<T> {
	T this [int x, int y] { get; set; }
	int GetUpperBound(int dimension);
}
