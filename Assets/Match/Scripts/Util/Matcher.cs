using UnityEngine;
using System.Collections.Generic;

// 匹配
public static class Matcher {
	private static int gTurn;

	// 匹配,所有originCells节点
	public static List<PatternResult> Match(Cell[,] mapCells, Cell[] originCells = null) {
		List<PatternResult> results = new List<PatternResult>();

        // find linked
        var allLink = FindAllLinked(mapCells, originCells);
        foreach(var link in allLink) {
			gTurn++;

            // match all patterns
            foreach(var pattern in PatternGroup.Patterns){
                //
                if(link.Count < pattern.Length) {
                    continue;
                }

                // check pattern for all link node
                foreach(var node in link) {
					if(node.turn == gTurn) {
						continue;
					}

					var result = pattern.Match(mapCells, node);
					if (result != null) {
						// mark match
						foreach(var elem in result.cells) {
							elem.turn = gTurn;
						}

						results.Add(result);
					}
                }
            }
        }

		return results;
    }

    // 查找cells中，相互连接的区域，cells为空，查找全部
    private static List<Cell>[] FindAllLinked(Cell[,] mapCells, Cell[] originCells = null)
    {
        gTurn++;

        List<List<Cell>> results = new List<List<Cell>>();
        if (originCells == null)
        {
            // find all
            foreach(var c in mapCells) {
                var ret = FindLinked(mapCells, c);
                if (ret != null) {
                    results.Add(ret);
                }
            }
        }
        else
        {
            foreach (var e in originCells)
            {
                var ret = FindLinked(mapCells, e);
                if (ret != null)
                {
                    results.Add(ret);
                }
            }
        }

        return results.ToArray();
    }

    private static readonly Coord2[] FIND_FOUR_DIR = new Coord2[] { new Coord2(1, 0), new Coord2(0, 1), new Coord2(-1, 0), new Coord2(0, -1) };

    private static List<Cell> FindLinked(Cell[,] mapCells, Cell origin)
    {
        // check valid
		if (origin == null || origin.IsEmpty || origin.turn == gTurn)
        {
            return null;
        }

        var mapW = mapCells.GetLength(0);
        var mapH = mapCells.GetLength(1);

        // find all linked elements
        List<Cell> linked = new List<Cell>();
        Stack<Coord2> queue = new Stack<Coord2>();
        queue.Push(origin.coord);
        while (queue.Count > 0)
        {
            var co = queue.Pop();
            foreach (var dir in FIND_FOUR_DIR)
            {
                var near = co + dir;
                while (Util.IsInside(mapW, mapH, near.x, near.y))
                {
                    var cell = mapCells[near.x, near.y];
					if (cell == null || cell.IsEmpty || cell.turn == gTurn)
                    {
                        break;
                    }
                    // find one
                    linked.Add(cell);
                    queue.Push(near);

                    cell.turn = gTurn;
                    near += dir;
                }
            }
        }

		// 小于三个不会产生任何匹配
		if(linked.Count < 3) {
			return null;
		}

        return linked;
    }
}