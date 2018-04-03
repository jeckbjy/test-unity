using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class DropInfo {
	const float DROP_TIME = 0.2f;

	public struct Move {
		public int turn;
		public bool isGen;

		public Coord2 from;
		public Coord2 to;

		public Direction GetDirection() {
			return Coord2.MakeDirection(this.from, this.to);
		}
	}

	private Element _elem;
	private List<Move> _drops = new List<Move>();

	public DropInfo(Element el) {
		this._elem = el;
	}

	public void AddDrop(int turn, bool isGen, Coord2 from, Coord2 to) {
		var info = new Move();
		info.turn = turn;
		info.isGen = isGen;
		info.from = from;
		info.to = to;
		_drops.Add(info);
	}

	public IAnim GetAnim() {
		List<Move> moves = this._drops;
		if (moves.Count <= 0)
			return null;

		var go = this._elem.go;
		var tran = go.transform;
		tran.localPosition = Cell.Coord2Pos(moves[0].from);

		Sequence sq = DOTween.Sequence();

		var move = moves[0];
		var turn = 0;

		//sq.AppendCallback(() => {
		//	Debug.Log("eid:" + _elem.id);
		//	Debug.Log("beg:" + Time.time);
		//});

		if(move.isGen) {
			if(move.turn != 0) {
				//Debug.Log("insert:" + move.turn);
				sq.AppendInterval(move.turn * DROP_TIME);
			}
			sq.AppendCallback(() => {
				//Debug.Log("ins:" + Time.time);
				go.SetActive(true);
			});
			turn = move.turn;
		}

		var last = 0;
		while(last < moves.Count) {
			move = moves[last++];
			// 插入等待
			if(move.turn != turn) {
				var start = move.turn - turn;
				//Debug.Log("insert:" + start);
				sq.AppendInterval(start * DROP_TIME);
				turn = move.turn;
			}

			// 自身移动时间
			turn++;

			// 查找相同方向，相同类型的
			int delay = 1;
			var dir = move.GetDirection();
			var dst = move.to;
			for (; last < moves.Count; last++) {
				var next = moves[last];
				if(next.GetDirection() != dir || 
				   next.from != moves[last - 1].to || 
				   next.turn != turn) {
					break;
				}
				dst = next.to;
				delay++;
				turn++;
			}

			//Debug.Log("delay:"+ delay);

			var tweener = tran.DOLocalMove(Cell.Coord2Pos(dst), delay * DROP_TIME);
			tweener.SetEase(Ease.Linear);
			sq.Append(tweener);
		}

		//sq.AppendCallback(() => {
		//	Debug.Log("end:"+Time.time);
		//});

		return new AnimDoTween(sq);
	}
}
