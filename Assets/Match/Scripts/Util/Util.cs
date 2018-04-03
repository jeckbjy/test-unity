using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Util {
    public static T Create<T>(Transform parent,string name) where T : Component {
        var go = new GameObject();
        go.name = name;
        go.transform.parent = parent;
        var co = go.AddComponent<T>();
        return co;
    }

    public static bool IsInside(int w, int h, int x, int y) {
        return x >= 0 && y >= 0 && x < w && y < h;
    }

	public static T ParseEnum<T>(string k)
	{
		return (T)Enum.Parse(typeof(T), k);
	}

	public static V GetOrCreate<U, V>(this IDictionary<U, V> dic, U key) where V : new() {
		V val;
		if(!dic.TryGetValue(key, out val)) {
			val = new V();
			dic.Add(key, val);
		}

		return val;
	}

	public static int RandIndexByWeight(int[] weights) {
		if(weights.Length == 0) {
			return -1;
		}

		int total = weights[weights.Length - 1];
		int value = UnityEngine.Random.Range(0, total - 1);
		for (int i = 0; i < weights.Length; i++) {
			if(value <= weights[i]) {
				return i;
			}
		}

		return weights.Length - 1;
	}

	public static T Rand<T>(T[] list) {
		int index = UnityEngine.Random.Range(0, list.Length);
		return list[index];
	}
}