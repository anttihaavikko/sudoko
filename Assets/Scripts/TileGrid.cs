﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGrid<T> {

	private T[,] data;
	private int width = 5;
	private int height = 5;

	public TileGrid (int w, int h) {
		width = w;
		height = h;

		data = new T[width,height];

		for (var iy = 0; iy < height; iy++) {
			for (var ix = 0; ix < width; ix++) {
				data[ix, iy] = default;
			}
		}
	}

	public void Set(T value, int x, int y) {
		data[x, y] = value;
	}

	public IEnumerable<T> GetNeighbours(int x, int y, bool includeDiagonals = false)
	{
		var list = new List<T>();
		AddIfNotDefault(x + 1, y, list);
		AddIfNotDefault(x - 1, y, list);
		AddIfNotDefault(x, y + 1, list);
		AddIfNotDefault(x, y - 1, list);

		if (includeDiagonals)
		{
			AddIfNotDefault(x + 1, y + 1, list);
			AddIfNotDefault(x - 1, y - 1, list);
			AddIfNotDefault(x - 1, y + 1, list);
			AddIfNotDefault(x + 1, y - 1, list);
		}
		
		// Debug.Log($"Finding neighbours for ({x},{y}) => {list.Count}");
		return list;
	}

	public bool IsInBounds(int x, int y)
	{
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	private void AddIfNotDefault(int x, int y, ICollection<T> list)
	{
		if (IsInBounds(x, y))
		{
			// Debug.Log("Adding " + x + ", " + y);
			list.Add(data[x, y]);
		}
	}

	public IEnumerable<T> All()
	{
		return data.Cast<T>();
	}

	public T Get(int x, int y, bool loop = false)
	{
		if (loop)
		{
			if (x < 0) x = width - 1;
			if (x > width - 1) x = 0;
			if (y < 0) y = height - 1;
			if (y > height - 1) y = 0;
		}
		
		if (x < 0 || y < 0 || x >= width || y >= height) return default;
		return data[x, y];
	}
	
	public string DataAsString() {
		string str = "";

		string[] values = {
			"_",
			"X",
			"<color=#ff0000>O</color>",
			"<color=#00ff00>O</color>"
		};

		for (int iy = 0; iy < height; iy++) {
			for (int ix = 0; ix < width; ix++) {
				str += data [ix, iy] != null ? "X" : ".";
			}

			if (iy < height - 1) {
				str += "\n";
			}
		}

		return str;
	}
	
	public bool Compare(T first, T second)
	{
		return EqualityComparer<T>.Default.Equals(first, second);
	}

	public void Swap(T first, T second)
	{
		for (var iy = 0; iy < height; iy++) {
			for (var ix = 0; ix < width; ix++) {
				if (Compare(first, data[ix, iy]))
				{
					data[ix, iy] = second;
					continue;
				}
				if (Compare(second, data[ix, iy]))
				{
					data[ix, iy] = first;
				}
			}
		}
	}
}
