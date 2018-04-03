using UnityEngine;
using System.Collections;

public struct Coord2 {
    public int x;
    public int y;

    public Coord2(int x, int y) {
        this.x = x;
        this.y = y;
    }

	public override string ToString()
	{
		return "[" + x + "," + y + "]";
	}

	public override bool Equals(object obj)
	{
		Coord2 co = (Coord2)obj;
		if(co.x == this.x && co.y == this.y) {
			return true;
		}

		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Coord2 co1, Coord2 co2)
	{
		return co1.x == co2.x && co1.y == co2.y;
	}

	public static bool operator !=(Coord2 co1, Coord2 co2)
	{
		return co1.x != co2.x || co1.y != co2.y;
	}

    public static Coord2 operator + (Coord2 co, Direction dir) {
        switch (dir)
        {
            case Direction.TOP:
                return new Coord2(co.x, co.y + 1);
            case Direction.BOTTOM:
                return new Coord2(co.x, co.y - 1);
            case Direction.LEFT:
                return new Coord2(co.x - 1, co.y);
            case Direction.RIGHT:
                return new Coord2(co.x + 1, co.y);
            case Direction.LEFT_TOP:
                return new Coord2(co.x - 1, co.y + 1);
            case Direction.RIGHT_TOP:
                return new Coord2(co.x + 1, co.y + 1);
            case Direction.LEFT_BOTTOM:
                return new Coord2(co.x - 1, co.y - 1);
            case Direction.RIGHT_BOTTOM:
                return new Coord2(co.x + 1, co.y - 1);
            default:
                return co;
        }
    }

    public static Coord2 operator+(Coord2 lhs, Coord2 rhs) {
        return new Coord2(lhs.x + rhs.x, lhs.y + rhs.y);
    }

    public static Coord2 operator-(Coord2 lhs, Coord2 rhs)
    {
        return new Coord2(lhs.x - rhs.x, lhs.y - rhs.y);
    }

	public static Direction MakeDirection(Coord2 co1, Coord2 co2) {
		var co = co2 - co1;
		Direction lr = Direction.NONE;
		if (co.x < 0) {
			lr = Direction.LEFT;
		} else if ( co.x > 0) {
			lr = Direction.RIGHT;
		}

		Direction tb = Direction.NONE;
		if(co.y < 0) {
			tb = Direction.BOTTOM;
		} else if(co.y > 0) {
			tb = Direction.TOP;
		}

		return (Direction)((byte)lr | (byte)tb);
	}
}

