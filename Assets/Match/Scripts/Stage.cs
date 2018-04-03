using UnityEngine;
using System;
using System.Collections.Generic;

public class Stage : MonoBehaviour
{
	public static readonly string LevelName = "Level4";
	private static readonly Direction[] SIDE_DIR = { Direction.LEFT, Direction.RIGHT};


	public Sprite tileA;
	public Sprite tileB;
	public Transform groundRoot;
	public Transform elementRoot;

    private Cell[,] mCells;
	private Map mMap;
	private Creator mCreator = new Creator();
	private Portal mPortol = new Portal();
	private AnimationPlayer mAnimPlayer = new AnimationPlayer();

    public int Width {
        get {
            return mCells.GetLength(0);
        }
    }

    public int Height {
        get {
            return mCells.GetLength(1);
        }
    }

    public bool IsInside(Coord2 co) {
        return Util.IsInside(Width, Height, co.x, co.y);
    }

    public Cell GetCell(int x, int y) {
        return mCells[x, y];
    }

    public Cell GetCell(Coord2 co) {
        return mCells[co.x, co.y];
    }
	// Use this for initialization
	void Start()
	{
        Debug.Log("create");
		this.Load();
		Drop();
		//Invoke("Drop", 1);
		//Invoke("Match", 1);
        //this.Create(Config.DEFAULT_MAP_W, Config.DEFAULT_MAP_H);
	}

	// Update is called once per frame
	void Update()
	{
		mAnimPlayer.Update();
	}

	public void Load() {
		Element.ResetGlobalId();

		mMap = new Map();
		mMap.Load(LevelName);
		mCreator.Load(mMap);
		mPortol.Load(mMap);

		// create cells
		Create(mMap.width, mMap.height);
		// create elements 
		foreach(var layer in mMap.layers) {
			var ltype = Util.ParseEnum<ElementLayer>(layer.layer);
			var elements = layer.GetElements();
			for (var x = 0; x < mMap.width; x++) {
				for (var y = 0; y < mMap.height; y++) {
					var idx = y * mMap.width + x;
					var el = elements[idx];
					if(el == ElementType.EMPTY) {
						continue;
					}

					if(el == ElementType.NULL) {
						if(ltype == ElementLayer.OBJECT) {
							mCells[x, y].Clear();
							mCells[x, y] = null;
						}
						continue;
					}

					if(el == ElementType.RANDOM) {
						if(ltype == ElementLayer.OBJECT) {
							// random
							el = mCreator.Drop(new Coord2(x, y));
							mCells[x, y].CreateElement(ltype, el, this.elementRoot);
						}
						continue;
					}

					// create special
					mCells[x, y].CreateElement(ltype, el, this.elementRoot);
				}
			}
		}

		// 还需要计算可达性
	}

	private void Create(int w, int h) {
        this.mCells = new Cell[w, h];

        var offsetX = w * Config.CELL_SIZE * 0.5f;
        var offsetY = h * Config.CELL_SIZE * 0.5f;
		this.transform.localPosition = new Vector3(-offsetX, -offsetY);

        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h;y++){
                var cell = new Cell(x, y);
                this.mCells[x, y] = cell;

                // create background
                Sprite sp;
                if ((x + y) % 2 == 0) {
					sp = this.tileA;
                }else {
                    sp = this.tileB;
                }

				var co = Util.Create<SpriteRenderer>(this.groundRoot, "cell_"+x+"_"+y);
                co.sprite = sp;
                co.gameObject.layer = 8;
                var tran = co.transform;
                tran.localScale = new Vector3(1.4f, 1.4f, 0);
                cell.SetBackground(co);
            }
        }
    }

    // 交换
    public bool TrySwap(Coord2  co1, Coord2 co2) {
        var c1 = GetCell(co1);
        var c2 = GetCell(co2);
		if(!c1.CanSwap || !c2.CanDrop) {
            return false;
        }

        Matcher.Match(mCells, new Cell[] { c1, c2 });

        // process

        return true;
    }

	//private void Process() {
	//	Match();
	//	Drop();
	//}

    // 匹配一轮
    private void Match() {
        // step1: match all
        var results = Matcher.Match(this.mCells);

		if(results.Count == 0) {
			// do shuffle;
			return;
		}
		Debug.Log("match:" + results.Count);

        // step2: process erase，简单的直接删除
		foreach(var result in results) {
			foreach(var cell in result.cells) {
				cell.ResetElement(ElementLayer.OBJECT);
			}
		}

		// step3:Drop();
		//Drop();
		Invoke("Drop", 0.1f);
    }

	public class CellComparer : IComparer<Cell>
	{
		//static int toggle = 0;
		public int Compare(Cell a, Cell b)
		{
			return b.coord.y - a.coord.y;
			//if (b.coordinate.y == a.coordinate.y) {
			//	toggle = (toggle + 1) % 3;
			//	return toggle - 2;
			//} else {
			//	return b.coordinate.y - a.coordinate.y;
			//}
		}
	}

	private void Drop() {
		Debug.Log("drop");
		var dropMap = new Dictionary<uint, DropInfo>();
		var emptyCells = new PriorityQueue<Cell>(new CellComparer());

		// find all empty cell
		for (var x = 0; x < this.Width; x++) {
			for (var y = 0; y < this.Height; y++) {
				var cell = GetCell(x, y);
				if(cell != null) {
					cell.drop = -1;
					cell.SetFlag(CellFlag.Hopeless, false);
					if (cell.IsEmpty) {
						emptyCells.Push(cell);
					}
				}
			}
		}
		// 查找一次hopeless??

		// do drop round
		for (var turn = 0; turn < 10000 ; turn++) {
			var hasDrop = DropRound(emptyCells, dropMap, turn);
			if(!hasDrop) {
				break;
			}
		}

		// to anim
		if(dropMap.Count > 0) {
			foreach(var item in dropMap) {
				mAnimPlayer += item.Value.GetAnim();
			}
			Debug.Log("play");
			mAnimPlayer.Play();
			mAnimPlayer.OnFinsh(() => {
				this.Match();
			});
		} else {
			this.Match();
		}
	}

	private bool DropRound(PriorityQueue<Cell> emptyCells, Dictionary<uint, DropInfo> dropMap, int turn) {
		var nextCells = new List<Cell>();
		// 向两侧掉落
		var slidings = new PriorityQueue<Cell>(new CellComparer());
		//var slidings = new HashSet<Coord2>();
		var waitings = new List<Cell>();
		var hasDrop = false;
		//var hopeless = false;

		while(emptyCells.Count > 0) {
			// process line,只处理直线掉落
			while(emptyCells.Count > 0) {
				var cell = emptyCells.Pop();
				var co = cell.coord;

				DropMode mode;

				var top = mPortol.GetTop(co);
				if(!cell.IsEmpty) {
					continue;
				}

				if(cell.drop == turn) {
					// 说明处理过了
					mode = DropMode.WAIT;
				} else if(mCreator.CanDropElement(co)) {
					mode = DropMode.GENERATE;
				} else {
					mode = GetDropMode(top, turn);
				}

				switch(mode) {
					case DropMode.GENERATE:
						// 直接生成
						hasDrop = true;
						DropOne(dropMap, turn, true, cell, cell);
						break;
					case DropMode.DROP:
						// 持续向上找掉落
						hasDrop = true;
						DropLine(emptyCells, dropMap, turn, cell, top);
						break;
					case DropMode.STUCK:
					case DropMode.WAIT:
						// 等待尝试两边滑落
						for (var i = 0; i < 2; i++) {
							var coord = cell.coord + (SIDE_DIR[i] | Direction.TOP);
							mode = GetDropMode(coord, turn);
							if(mode == DropMode.DROP) {
								slidings.Push(GetCell(coord));
								//slidings.Add(coord);
							}
						}

						waitings.Add(cell);
						break;
					//case DropMode.STUCK:
					//	// 尝试两侧滑落，记录并延迟计算
					//	var stuck = 0;
					//	for (var i = 0; i < 2; i++) {
					//		var coord = cell.coord + (SIDE_DIR[i] | Direction.TOP);
					//		mode = GetDropMode(coord, turn);
					//		if(mode == DropMode.DROP) {
					//			slidings.Add(coord);
					//			//sidings[coord] |= SIDE_DIR[i];
					//		} else if(mode == DropMode.STUCK) {
					//			// 记录两侧
					//			stuck++;
					//		}
					//	}

					//	// 记录hopelsess
					//	if(stuck == 2) {
					//		// 回溯最大范围的标记hopeless??
					//		if(cell.SetHopeless()) {
					//			hopeless = true;
					//		}
					//	} else {
					//		// 等待slide
					//		waitings.Add(cell);
					//	}
					//	break;
					//case DropMode.WAIT:
						//// 等待下次尝试
						//nextCells.Add(cell);
						//break;
				}
			}

			// 处理两侧掉落
			while(slidings.Count > 0) {
				var cell = slidings.Pop();
				if (cell.IsHopeless) {
					continue;
				}
				// 非空位置,不能掉落
				if (cell.IsEmpty) {
					continue;
				}
				// 已经掉落过了
				if (cell.drop == turn) {
					continue;
				}

				// 优先尝试预定方向，每次交换一个方向
				for (var i = 0; i < 2; i++) {
					var idx = (i + cell.bias) % 2;
					var testCoord = cell.coord + (SIDE_DIR[idx] | Direction.BOTTOM);
					if (!IsInside(testCoord)) {
						continue;
					}
					var testCell = GetCell(testCoord);
					// 校验目标，TODO:gen
					if (testCell == null || !testCell.IsEmpty) {
						continue;
					}

					// 可以滑落
					DropOne(dropMap, turn, false, cell, testCell);
					hasDrop = true;
					// swap bias
					cell.bias = (idx + 1) % 2;

					// 产生新的空格子,尝试竖着掉落
					var top = mPortol.GetTop(cell.coord);
					var mode = GetDropMode(top, turn);
					if(mode == DropMode.DROP) {
						DropLine(emptyCells, dropMap, turn, cell, top);
					} else {
						emptyCells.Push(cell);
					}
					//emptyCells.Push(cell);
					break;
				}
			}

			//foreach(var coord in slidings) {
			//	var cell = GetCell(coord);

			//	if(cell.IsHopeless) {
			//		continue;
			//	}
			//	// 非空位置,不能掉落
			//	if(cell.IsEmpty) {
			//		continue;
			//	}
			//	// 已经掉落过了
			//	if(cell.drop == turn) {
			//		continue;
			//	}

			//	// 优先尝试预定方向，每次交换一个方向
			//	for (var i = 0; i < 2; i++) {
			//		var idx = (i + cell.bias) % 2;
			//		var testCoord = coord + (SIDE_DIR[idx] | Direction.BOTTOM);
			//		if(!IsInside(testCoord)) {
			//			continue;
			//		}
			//		var testCell = GetCell(testCoord);
			//		// 校验目标，TODO:gen
			//		if(!testCell.IsEmpty) {
			//			continue;
			//		}

			//		// 可以滑落
			//		DropOne(dropMap, turn, false, cell, testCell);
			//		hasDrop = true;
			//		// swap bias
			//		cell.bias = (idx + 1) % 2;

			//		// 产生新的空格子,竖着掉落
			//		//emptyCells.Push(cell);
			//		break;
			//	}
			//}

			// 没有滑落成功的，等待下一次掉落
			foreach(var cell in waitings) {
				if(!cell.IsHopeless && cell.IsEmpty) {
					nextCells.Add(cell);
				}
			}

			slidings.Clear();
			waitings.Clear();

			// 需要重新计算一次
			//if(emptyCells.Count == 0 && hopeless) {
			//	hopeless = false;
			//	foreach (var c in nextCells) {
			//		emptyCells.Push(c);
			//	}
			//	nextCells.Clear();
			//}
		}

		foreach(var c in nextCells) {
			emptyCells.Push(c);
		}

		return hasDrop;
	}

	// 直线掉落
	private void DropLine(PriorityQueue<Cell> emptyCells, Dictionary<uint, DropInfo> dropMap, int turn, Cell cell, Coord2 top) {
		DropMode mode;
		var curCell = cell;
		while (true) {
			var topCell = GetCell(top);
			DropOne(dropMap, turn, false, topCell, curCell);
			curCell = topCell;

			// 下一次处理
			top = mPortol.GetTop(curCell.coord);
			mode = GetDropMode(top, turn);
			if (mode != DropMode.DROP) {
				// gen??
				emptyCells.Push(curCell);
				break;
			}
		}
	}

	private void DropOne(Dictionary<uint, DropInfo> dropMap, int turn,bool isGen, Cell from, Cell to)
	{
		if(isGen) {
			var type = mCreator.Drop(to.coord);
			var el = to.CreateElement(ElementLayer.OBJECT, type, this.elementRoot, false);
			to.drop = turn;

			// 向上移动一个位置掉落??Direction.TOP
			GetDropInfo(dropMap, el).AddDrop(turn, true, from.coord + Direction.TOP, to.coord);
		} else {
			var el = from.RemoveElement(ElementLayer.OBJECT);
			to.SetElement(ElementLayer.OBJECT, el);
			to.drop = turn;

			GetDropInfo(dropMap, el).AddDrop(turn, false, from.coord, to.coord);
		}
	}

	private DropInfo GetDropInfo(Dictionary<uint, DropInfo> dropMap, Element el) {
		DropInfo info;
		if(!dropMap.TryGetValue(el.id, out info)) {
			info = new DropInfo(el);
			dropMap[el.id] = info;
		}

		return info;
	}

    private enum DropMode
    {
        GENERATE,   // 生成新的
        DROP,       // 可以掉落
        STUCK,      // 卡主不能掉落
		WAIT,       // 上边为空，等待下一次
		//OUTSIDE,	// 越界了,不用校验两边
    }

    private DropMode GetDropMode(Coord2 co, int turn)
    {
		if(!IsInside(co)) {
			return DropMode.STUCK;
		}

		var cell = GetCell(co);
		if(cell == null || !cell.CanDrop) {
			return DropMode.STUCK;
		}

		// 说明已经处理掉落过了,等待下一次处理
		if(cell.drop == turn) {
			return DropMode.WAIT;
		}

		if(cell.IsEmpty) {
			if(cell.HasFlag(CellFlag.Hopeless)) {
				return DropMode.STUCK;
			} else {
				return DropMode.WAIT;
			}
		}

		return DropMode.DROP;
    }
}
