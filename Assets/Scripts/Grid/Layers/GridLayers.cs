using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// </summary>
public class GridLayers : Dictionary<Type, object> {

	/// <summary>
	/// Add an instance, this will throw if the type already exists
	/// </summary>
	/// <param name="layer">A Layer holding an array of type T</param>
	/// <typeparam name="T">The type key in the dictionary</typeparam>
	public void Add<T>(IGridLayer<T> layer) {
		if (this.ContainsKey(typeof(T))) {
			throw new ArgumentException();
		}

		this.Add(typeof(T), layer);
	}

	/// <summary>
	/// Get an instance of IGridLayer
	/// </summary>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public IGridLayer<T> Get<T>() {
		if (!this.ContainsKey(typeof(T))) {
			throw new NullReferenceException();
		}

		IGridLayer<T> layer = this [typeof(T)] as IGridLayer<T>;

		if (layer == null) {
			throw new NullReferenceException();
		}

		return layer;
	}

	/// <summary>
	/// Set an instance, this will overwrite any existing contents
	/// </summary>
	/// <param name="layer">Layer.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void Set<T>(GridLayer<T> layer) {
		if (this.ContainsKey(typeof(T))) {
			this[typeof(T)] = layer;
			return;
		}

		this.Add (typeof(T), layer);
	}

	/// <summary>
	/// Get T in the layer of type T, and at the coordinate x,y
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <typeparam name="T">Type of location to get</typeparam>
	public T Get<T>(int x, int y) {
		if (!this.ContainsKey(typeof(T))) {
			throw new ArgumentException();
		}

		IGridLayer<T> layer = this[typeof(T)] as IGridLayer<T>;

		if (layer == null) {
			throw new NullReferenceException();
		}

		return layer[x, y];
	}

	/// <summary>
	/// Set the specified x and y location in T layer
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <typeparam name="T">The layer that will be set</typeparam>
	public void Set<T>(int x, int y, T item) {
		if (!this.ContainsKey(typeof(T))) {
			throw new ArgumentException();
		}

		IGridLayer<T> layer = this[typeof(T)] as IGridLayer<T>;
		
		if (layer == null) {
			throw new NullReferenceException();
		}

		if (layer.GetUpperBound (0) < x || x < 0) {
			throw new ArgumentException();
		}

		if (layer.GetUpperBound (1) < y || y < 0) {
			throw new ArgumentException();
		}

		layer[x, y] = item;
	}
}

