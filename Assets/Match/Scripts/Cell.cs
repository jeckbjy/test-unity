using UnityEngine;
using System;
using System.Collections;

// 每一层元素
public class Element
{
	private static uint globalId = 0;
	public static void ResetGlobalId() {
		globalId = 0;
	}

	public readonly uint id;
    public ElementType type;
	public GameObject go;
	public Element(ElementType type, GameObject go) {
		this.id = globalId++;
		this.type = type;
		this.go = go;
	}

	public void Clear() {
		Pool.Instance.Put(this.type, this.go);
		this.go = null;
		this.type = ElementType.EMPTY;
	}

	public override int GetHashCode()
	{
		return this.id.GetHashCode();
	}
}

[Flags]
public enum CellFlag
{
	CanDrop 	= 0x01,	// 能否掉落
	CanSwap 	= 0x02,	// 能否交换
	CanErase 	= 0x04,	// 能否被消除
	CanGen 		= 0x08,	// 能否产生新元素
	HasObject 	= 0x10, // 是否有主元素
	HasJam  	= 0x20,
	GenJam		= 0x40,
	Hopeless	= 0x80,	// 不可达点

	Default 	=  CanDrop | CanSwap | CanErase,
}

public class Cell : IComparable
{
    public Coord2 coord;
    public Vector2 pos;
	private SpriteRenderer background;
	private Element[] elements;

    public int turn; // 标识本轮是否匹配过
	public int drop; // 标识本轮是否drop过,和元素同时操作
	public int bias; // 滑落倾斜方向

	private CellFlag flags = CellFlag.Default;

    public static bool IsNull(Cell cell) {
		return cell == null;
    }

    public static bool IsSameColor(Cell lhs, Cell rhs)
    {
		return lhs.GetObjectType() == rhs.GetObjectType();
    }

	public static Vector2 Coord2Pos(Coord2 co)
	{
		return new Vector2((co.x + 0.5f) * Config.CELL_SIZE, (co.y + 0.5f) * Config.CELL_SIZE);
	}

    public Cell(int x, int y)
    {
        this.coord.x = x;
        this.coord.y = y;
		this.pos = Coord2Pos(this.coord);
        //this.pos.x = vx;
        //this.pos.y = vy;
    }

    public int CompareTo(object obj)
    {
        var rhs = obj as Cell;
        if(this.coord.y == rhs.coord.y) {
            return this.coord.x - rhs.coord.x;
        } else {
            return this.coord.y - rhs.coord.y;
        }
    }

    public void SetBackground(SpriteRenderer sp)
    {
		sp.transform.localPosition = this.pos;
        this.background = sp;
    }

    public Element GetElement(ElementLayer layer)
    {
		if(this.elements == null) {
			return null;
		}

        return this.elements[(int)layer];
    }

    public void SetElement(ElementLayer layer, Element element)
    {
		if(this.elements == null) {
			this.elements = new Element[(int)ElementLayer.COUNT];
		}

        this.elements[(int)layer] = element;
    }

	public void ResetElement(ElementLayer layer) {
		var el = GetElement(layer);
		if (el != null) {
			el.Clear();
			this.elements[(int)layer] = null;
		}
	}

	// 移除但不清空
	public Element RemoveElement(ElementLayer layer) {
		var el = GetElement(layer);
		this.elements[(int)layer] = null;
		return el;
	}

	public Element CreateElement(ElementLayer layer, ElementType type, Transform parent, bool active = true) {
		var go = Pool.Instance.Get(type, parent);
		if(go == null) {
			return null;
		}

		go.name = "elem_" + this.coord.x + "_" + this.coord.y;
		go.transform.localPosition = new Vector3(this.pos.x, this.pos.y, 0);
		go.SetActive(active);
		var el = new Element(type, go);
		SetElement(layer, el);

		return el;
	}

	public void Clear() {
		this.background.gameObject.SetActive(false);
		this.background.transform.parent = null;
		if(this.elements != null) {
			foreach (var el in this.elements) {
				if (el != null) {
					el.Clear();
				}
			}	
		}
	}

	public ElementType GetObjectType() {
		var el = GetElement(ElementLayer.OBJECT);
		if(el != null) {
			return el.type;
		}

		return ElementType.EMPTY;
	}

	public bool HasFlag(CellFlag flag) {
		return (this.flags & flag) == flag;
	}

	public void SetFlag(CellFlag flag, bool value = true) {
		if (value) {
			this.flags |= flag;
		} else {
			this.flags &= ~flag;
		}
	}

	public bool SetHopeless() {
		if(!this.HasFlag(CellFlag.Hopeless)) {
			this.SetFlag(CellFlag.Hopeless);
			return true;
		}

		return false;
	}

	public bool CanErase {
		get {
			return this.HasFlag(CellFlag.CanErase);
		}
	}

	public bool CanSwap {
		get {
			return this.HasFlag(CellFlag.CanSwap);
		}
	}

	public bool CanDrop {
		get {
			return this.HasFlag(CellFlag.CanDrop);
		}
	}

    public bool CanShuffle() {
        return true;
    }

	public bool IsHopeless {
		get {
			return this.HasFlag(CellFlag.Hopeless);
		}
	}

    public bool IsEmpty {
		get {
			return GetObjectType() == ElementType.EMPTY;
		}
    }
}
