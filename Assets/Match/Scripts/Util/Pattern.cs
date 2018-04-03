using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternResult
{
	public PatternType type;
	public Cell[] cells;
	public Cell center;
	//public CellShape shape;
	public int delayTurn;
}

// 匹配模式
public class Pattern
{
    public PatternType type;
    public Coord2[] template;
    public bool fixCenter;
    public Pattern(PatternType type, bool center, int[,] template)
    {
		var len = template.GetLength(0);
        this.type = type;
        this.fixCenter = center;
        this.template = new Coord2[len];
        for (int i = 0; i < len; i++)
        {
            this.template[i] = new Coord2(template[i, 0], template[i, 1]);
        }
    }

    public int Length
    {
        get
        {
            return this.template.Length;
        }
    }

	public PatternResult Match(Cell[,] mapCells, Cell origin) {
        var mapW = mapCells.GetLength(0);
        var mapH = mapCells.GetLength(1);

		List<Cell> elems = new List<Cell>();
		elems.Add(origin);

        var co = origin.coord;
        foreach(var off in this.template) {
            var dst = co + off;
            if (!Util.IsInside(mapW, mapH, dst.x, dst.y)) {
				return null;
            }

			if(mapCells[dst.x, dst.y] == null) {
				return null;
			}

            if(!Cell.IsSameColor(origin, mapCells[dst.x, dst.y])) {
				return null;
            }

			// check can erase
			elems.Add(mapCells[dst.x, dst.y]);
        }

		// return result
		var result = new PatternResult();
		result.type = this.type;
		result.cells = elems.ToArray();

		return result;
    }
}

public static class PatternGroup
{
    private static List<Pattern> _patterns;

    public static List<Pattern> Patterns {
        get {
            return _patterns;
        }
    }

    static PatternGroup()
    {
        _patterns = new List<Pattern>
        {

			// [ + - - - -]
			// [ + - - - -]
			// [ + - - - -]
			// [ + - - - -]
			// [ * - - - -]
			new Pattern(PatternType.FIVE_ONE_LINE, false, new int[,] { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 0, 4 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - - - - ]
			// [ * + + + + ]
			new Pattern(PatternType.FIVE_ONE_LINE, false, new int[,] { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ + + * - - ]
			// [ - - + - - ]
			// [ - - + - - ]
			new Pattern(PatternType.L_STYLE, true, new int[,] { { -2, 0 }, { -1, 0 }, { 0, -1 }, { 0, -2 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ + - - - - ]
			// [ + - - - - ]
			// [ * + + - - ]
			new Pattern(PatternType.L_STYLE, true, new int[,] { { 1, 0 }, { 2, 0 }, { 0, 1 }, { 0, 2 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ * + + - - ]
			// [ + - - - - ]
			// [ + - - - - ]
			new Pattern(PatternType.L_STYLE, true, new int[,] { { 1, 0 }, { 2, 0 }, { 0, -1 }, { 0, -2 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - + - - ]
			// [ - - + - - ]
			// [ + + * - - ]
			new Pattern(PatternType.L_STYLE, true, new int[,] { { -2, 0 }, { -1, 0 }, { 0, 1 }, { 0, 2 } }),


			// [ - - - - - ]
			// [ - - - - - ]
			// [ + * + - - ]
			// [ - + - - - ]
			// [ - + - - - ]
			new Pattern(PatternType.T_STYLE, true, new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, -2 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ + - - - - ]
			// [ * + + - - ]
			// [ + - - - - ]
			new Pattern(PatternType.T_STYLE, true, new int[,] { { 1, 0 }, { 2, 0 }, { 0, 1 }, { 0, -1 } }),


			// [ - - - - - ]
			// [ - - - - - ]
			// [ - + - - - ]
			// [ - + - - - ]
			// [ + * + - - ]
			new Pattern(PatternType.T_STYLE, true, new int[,] { { 0, 1 }, { 0, 2 }, { -1, 0 }, { 1, 0 } }),


			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - + - - ]
			// [ + + * - - ]
			// [ - - + - - ]
			new Pattern(PatternType.T_STYLE, true, new int[,] { { -2, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ - + - - - ]
			// [ + * + - - ]
			// [ - + - - - ]
			new Pattern(PatternType.CROSS_STYLE, true, new int[,] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } }),


			// [ - - - - - ]
			// [ + - - - - ]
			// [ + - - - - ]
			// [ + - - - - ]
			// [ * - - - - ]
			new Pattern(PatternType.FORE_ONE_LINE_V, false, new int[,] { { 0, 1 }, { 0, 2 }, { 0, 3 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - - - - ]
			// [ * + + + - ]
			new Pattern(PatternType.FORE_ONE_LINE_H, false, new int[,] { { 1, 0 }, { 2, 0 }, { 3, 0 } }),


			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - - - - ]
			// [ - - - - - ]
			// [ * + + - - ]
			new Pattern(PatternType.THREE, false, new int[,] { { 1, 0 }, { 2, 0 } }),

			// [ - - - - - ]
			// [ - - - - - ]
			// [ + - - - - ]
			// [ + - - - - ]
			// [ * - - - - ]
			new Pattern(PatternType.THREE, false, new int[,] { { 0, 1 }, { 0, 2 } })
        };
    }
}
