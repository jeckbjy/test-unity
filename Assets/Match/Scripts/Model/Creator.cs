using UnityEngine;
using System.Collections.Generic;

public class Creator {
	private List<ICreator> _creators = new List<ICreator>();
	private Dictionary<Coord2, int> _points = new Dictionary<Coord2, int>();

	public void Load(Map map) {
		this._creators.Clear();
		this._points.Clear();
		// 普通掉落
		foreach(var creator in map.creators) {
			_creators.Add(new NormalCreator(creator));
		}

		foreach(var gen in map.genPositions) {
			_points.Add(new Coord2(gen.x, gen.y), gen.creator);
		}

		// 收集品
	}

	public bool CanDropElement(Coord2 co) {
		return _points.ContainsKey(co);
	}

	public ElementType Drop(Coord2 co) {
		int idx = 0;
		_points.TryGetValue(co, out idx);
		return _creators[idx].Create();
		//var value = UnityEngine.Random.Range(0, 5);
		//return ElementType.NORMAL_A + value;
	}
}

interface ICreator
{
	ElementType Create();
}

class NormalCreator : ICreator
{
	private ElementType[] _elements;
	private int[] _weights;

	public NormalCreator(Map.CreatorData data) {
		int count = data.elements.Count;
		_elements = new ElementType[count];
		_weights = new int[count];

		int total = 0;
		for (int i = 0; i < count; i++) {
			var e = data.elements[i];
			_elements[i] = Util.ParseEnum<ElementType>(e.k);
			total += e.v;
			_weights[i] = total;
		}
	}

	public ElementType Create()
	{
		int index = Util.RandIndexByWeight(this._weights);
		return _elements[index];
	}
}

class CollectionCreator : ICreator
{
	private int _min;
	private int _total;
	private int _droped;
	private bool _isEnable;
	private ElementType[] _elements;

	public CollectionCreator(Map.CollectionTarget data)
	{
		_elements = new ElementType[data.elements.Length];
		for (int i = 0; i < data.elements.Length; i++) {
			_elements[i] = Util.ParseEnum<ElementType>(data.elements[i]);
		}
		_min = data.min;
		_total = data.total;
		_isEnable = data.isTarget;
	}

	public ElementType Create()
	{
		return Util.Rand(this._elements);
	}
}
