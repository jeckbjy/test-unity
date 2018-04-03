using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool : Singleton<Pool>
{
	private Dictionary<ElementType, Queue<GameObject>> _cache;
	private Dictionary<ElementType, GameObject> _prefabCache;
	private bool _inited = false;

	public Pool()
	{
		_cache = new Dictionary<ElementType, Queue<GameObject>>();
		_prefabCache = new Dictionary<ElementType, GameObject>();

	}

	private void Init()
	{
		var names = Enum.GetValues(typeof(ElementType));
		foreach (var nm in names) {
			var n = (ElementType)nm;
			_cache.Add(n, new Queue<GameObject>());
		}

		AddPrefab(ElementType.NORMAL_A, "NormalA");
		AddPrefab(ElementType.NORMAL_B, "NormalB");
		AddPrefab(ElementType.NORMAL_C, "NormalC");
		AddPrefab(ElementType.NORMAL_D, "NormalD");
		AddPrefab(ElementType.NORMAL_E, "NormalE");
		AddPrefab(ElementType.NORMAL_F, "NormalF");

		AddPrefab(ElementType.LINE_H, "LineH");
		AddPrefab(ElementType.LINE_V, "LineV");
		AddPrefab(ElementType.BOMB, "Bomb");
		AddPrefab(ElementType.COLOR_ERASE, "ColorErase");
		AddPrefab(ElementType.SAND_L1, "SAND1");
		AddPrefab(ElementType.SAND_L2, "SAND2");
		AddPrefab(ElementType.SAND_L3, "SAND3");

		AddPrefab(ElementType.CIRRUS_L1, "CIRRUS1");
		AddPrefab(ElementType.CIRRUS_L2, "CIRRUS2");
		AddPrefab(ElementType.ICE_1, "ICE1");
		AddPrefab(ElementType.ICE_2, "ICE2");
		AddPrefab(ElementType.ICE_3, "ICE3");

		AddPrefab(ElementType.JELLY_L1, "JELLY1");
		AddPrefab(ElementType.JELLY_L2, "JELLY3");
		AddPrefab(ElementType.WOOD_1, "WOOD1");
		AddPrefab(ElementType.WOOD_2, "WOOD2");
		AddPrefab(ElementType.WOOD_3, "WOOD3");
	}

	private void AddPrefab(ElementType type, string name) {
		GameObject go = Resources.Load<GameObject>("Match3/Prefabs/Element/" + name);
		if(go != null) {
			_prefabCache.Add(type, go);
		}
	}


	public GameObject Get(ElementType name, Transform parent)
	{
		if (!_inited) {
			Init();
			_inited = true;
		}

		Queue<GameObject> queue = _cache[name];
		GameObject go = null;
		if (queue.Count > 0) {
			go = queue.Dequeue();
			if(parent != null) {
				go.transform.parent = parent;
			}
			go.SetActive(true);
		} else {
			GameObject prefab;
			if (_prefabCache.TryGetValue(name, out prefab)) {
				go = GameObject.Instantiate<GameObject>(prefab, parent);
				go.SetActive(true);
			} else {
				Debug.LogError("can't load element view!");
			}
		}

		return go;
	}

	public void Put(ElementType name, GameObject element)
	{
		if(element == null) {
			return;
		}

		Queue<GameObject> queue = _cache[name];
		element.SetActive(false);
		queue.Enqueue(element);
	}

	public void Clear()
	{
		_cache.Clear();
	}
}