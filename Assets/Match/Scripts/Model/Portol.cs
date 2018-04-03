using UnityEngine;
using System.Collections.Generic;

// 传送门信息
public class Portal {
	//private Dictionary<Coord2, Coord2> mIn;
	private Dictionary<Coord2, Coord2> mOut = new Dictionary<Coord2, Coord2>();

	public void Load(Map map) {
		this.mOut.Clear();
		if(map.wormHole.Count == 0) {
			return;
		}

		foreach(var pair in map.wormHole) {
			this.mOut.Add(pair.to, pair.from);
		}
	}

	// 计算传送门
	public Coord2 GetTop(Coord2 co) {
		Coord2 result;
		if(this.mOut.Count > 0 && this.mOut.TryGetValue(co, out result)) {
			return result;
		}

		return co + Direction.TOP;
	}
}