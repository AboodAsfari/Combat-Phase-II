using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionUtils{
    public static GameController currentController;
    public static Action currentAction;
    public static Unit currentUnit;

    public static Dictionary<Vector2Int, List<Vector2Int>> tilePaths = new Dictionary<Vector2Int, List<Vector2Int>>();

    public static void MoveUnit(Vector2Int pos){
        currentAction.ConsumeTokens(currentUnit, currentController);
        Debug.Log("TEMP");
    }

    private static HashSet<Vector2Int> BasicTileTraversal(Vector2Int initPos, int range, PathfindType pathfindType, Func<Vector2Int, bool> addFilter, Func<Vector2Int, Vector2Int, bool> traverseFilter){
        HashSet<Vector2Int> traversableTiles = new HashSet<Vector2Int>();
        HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
        Queue<Tuple<Vector2Int, int, int>> queue = new Queue<Tuple<Vector2Int, int, int>>();

        queue.Enqueue(new Tuple<Vector2Int, int, int>(initPos, 0, 0));
        visitedTiles.Add(initPos);
        tilePaths.Clear();
        tilePaths.Add(initPos, new List<Vector2Int>());

        while(queue.Count > 0){
            Vector2Int pos = queue.Peek().Item1;
            int currRange = queue.Peek().Item2;
            int currExtend = queue.Dequeue().Item3;

            int effectiveRangeModifier = 0;
            if(pathfindType == PathfindType.RANGED) effectiveRangeModifier = currentController.msc.GetTile(pos).GetTileState().GetElevation() - 
                currentController.msc.GetTile(initPos).GetTileState().GetElevation();

            if(!pos.Equals(initPos) && addFilter(pos) && currRange + effectiveRangeModifier - currExtend <= range) traversableTiles.Add(pos);
            if(currRange + effectiveRangeModifier - currExtend >= range) continue;

            foreach(Direction dir in Enum.GetValues(typeof(Direction))){
                Vector2Int dirVector = dir.GetVector();
                Vector2Int nextPos = new Vector2Int(pos.x + dirVector.x, pos.y + dirVector.y);
                if(visitedTiles.Contains(nextPos)) continue;
                visitedTiles.Add(nextPos);

                if(traverseFilter(pos, nextPos)){
                    int rangeCost = 1;
                    if(pathfindType == PathfindType.TRAVERSAL){
                        int elevationDiff = currentController.msc.GetTile(pos).GetTileState().GetElevation() - 
                            currentController.msc.GetTile(nextPos).GetTileState().GetElevation();
                        if(elevationDiff == 1) rangeCost = 2;
                        else if(elevationDiff == -1) rangeCost = 3;
                        else if(elevationDiff != 0) continue;
                    }else if(pathfindType == PathfindType.MELEE){
                        rangeCost = Math.Abs(currentController.msc.GetTile(pos).GetTileState().GetElevation() - 
                            currentController.msc.GetTile(nextPos).GetTileState().GetElevation()) + 1;
                    }
                    queue.Enqueue(new Tuple<Vector2Int, int, int>(nextPos, currRange + rangeCost, currExtend));
                    List<Vector2Int> nextPath = new List<Vector2Int>(tilePaths[pos]);
                    nextPath.Add(pos);
                    tilePaths.Add(nextPos, nextPath);
                }
            }
        }
        return traversableTiles;
    }

    public static HashSet<Vector2Int> GetTraversableTiles(Vector2Int initPos, int range, PathfindType pathfindType){
        return BasicTileTraversal(initPos, range, pathfindType,
            (pos) => currentController.msc.GetUnit(pos) == null, 
            (pos, nextPos) => currentController.msc.GetTile(nextPos) != null && 
                    (currentController.msc.GetUnit(nextPos) == null || currentController.msc.GetUnit(nextPos).GetUnitInfo().GetCanWalkThrough()));
    }

    public static HashSet<Vector2Int> GetNearbyUnits(Vector2Int initPos, int range, PathfindType pathfindType){
        return BasicTileTraversal(initPos, range, pathfindType,
            (pos) => currentController.msc.GetUnit(pos) != null,
            (pos, nextPos) => currentController.msc.GetTile(nextPos) != null &&  (pos.Equals(initPos) || 
                    currentController.msc.GetUnit(pos) == null || currentController.msc.GetUnit(pos).GetUnitInfo().GetCanWalkThrough()));
    }
}
