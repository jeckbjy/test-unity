using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class Map
{
	public static int MAP_VERSION = 1;

	public int version;
	public int width;
	public int height;
	public MapMode mode;
	public int limit;
	public int energyScore;   // the configured energy score
	public int nextEnergyScore;   // the next energy score to trigger using the specific item
	public int energyItemNumber;   // the number of the energy items which will be triggered

	[Serializable]
	public class KeyValue
	{
		public string k;
		public int v;
		public KeyValue(string k, int v)
		{
			this.k = k;
			this.v = v;
		}
	}

	[Serializable]
	public class LayerData
	{
		public string layer;
		public string[] elements;
		public ElementType[] GetElements()
		{
			ElementType[] es = new ElementType[elements.Length];
			for (int i = 0; i < elements.Length; i++) {
				es[i] = Util.ParseEnum<ElementType>(elements[i]);
			}
			return es;
		}
	}
	public List<LayerData> layers;

	[Serializable]
	public class CreatorData
	{
		public List<KeyValue> elements;
	}
	public List<CreatorData> creators;

	[Serializable]
	public class GenPos
	{
		public int x;
		public int y;
		public int creator;
		public GenPos(int x, int y, int creator)
		{
			this.x = x;
			this.y = y;
			this.creator = creator;
		}
	}
	public List<GenPos> genPositions;

	[Serializable]
	public class CollectionTarget
	{
		public bool isTarget;
		public int total;
		public int min;
		public string[] elements;
		public List<Coord2> genPos;
		public List<Coord2> collectPos;
	}

	public List<KeyValue> targets;
	public CollectionTarget collectionTarget;

	[Serializable]
	public struct CoordinatePair
	{
		public Coord2 from;
		public Coord2 to;
	}

	public List<CoordinatePair> wormHole;

	public Map()
	{
		version = MAP_VERSION;
		width = 11;
		height = 9;
		mode = MapMode.MOVE;
		limit = 25;
		energyScore = 0;
		nextEnergyScore = 0;
		layers = new List<LayerData>();
		creators = new List<CreatorData>();
		genPositions = new List<GenPos>();
		targets = new List<KeyValue>();
		collectionTarget = new CollectionTarget();
		collectionTarget.elements = new string[] { };
		collectionTarget.genPos = new List<Coord2>();
		collectionTarget.collectPos = new List<Coord2>();
		wormHole = new List<CoordinatePair>();
	}

	public void VersionFix()
	{
		version = MAP_VERSION;
	}

	public string[] GetLayer(ElementLayer layer)
	{
		for (int i = 0; i < layers.Count; i++) {
			if (layers[i].layer.Equals(layer.ToString())) {
				return layers[i].elements;
			}
		}
		return null;
	}

	public ElementType GetElement(Coord2 co)
	{
		string[] elements = GetLayer(ElementLayer.OBJECT);
		int idx = co.y * width + co.x;
		ElementType element = Util.ParseEnum<ElementType>(elements[idx]);
		return element;
	}

	public List<ElementLayer> GetAllLayers()
	{
		List<ElementLayer> ret = new List<ElementLayer>();
		foreach (var l in layers) {
			ret.Add(Util.ParseEnum<ElementLayer>(l.layer));
		}
		return ret;
	}

	public void SetNextEnergyScore()
	{
		nextEnergyScore += energyScore;
	}

	public void Load(string name) {
		var data = Resources.Load("Levels/" + name, typeof(TextAsset)) as TextAsset;
		JsonUtility.FromJsonOverwrite(data.ToString(), this);
	}
}
