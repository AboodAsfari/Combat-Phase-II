using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionUtils{
    public static GameController currentController;
    public static Action currentAction;
    public static Unit currentUnit;

    public static void MoveUnit(Vector2Int pos){
        currentAction.ConsumeTokens(currentUnit, currentController);
        Debug.Log("TEMP");
    }

    public static HashSet<Vector2Int> GetTraversableTiles(Vector2Int initPos, int range){
        HashSet<Vector2Int> traversableTiles = new HashSet<Vector2Int>();
        HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
        Queue<Tuple<Vector2Int, int>> queue = new Queue<Tuple<Vector2Int, int>>();

        queue.Enqueue(new Tuple<Vector2Int, int>(initPos, 0));
        visitedTiles.Add(initPos);

        while(queue.Count > 0){
            Vector2Int pos = queue.Peek().Item1;
            int currRange = queue.Dequeue().Item2;
            if(!pos.Equals(initPos) && currentController.msc.GetUnit(pos) == null) traversableTiles.Add(pos);
            if(currRange == range) continue;

            foreach(Direction dir in Enum.GetValues(typeof(Direction))){
                Vector2Int dirVector = dir.GetVector();
                Vector2Int nextPos = new Vector2Int(pos.x + dirVector.x, pos.y + dirVector.y);
                if(visitedTiles.Contains(nextPos)) continue;
                visitedTiles.Add(nextPos);

                if(currentController.msc.GetTile(nextPos) != null && 
                    (currentController.msc.GetUnit(nextPos) == null || currentController.msc.GetUnit(nextPos).GetUnitInfo().GetCanWalkThrough())) 
                    queue.Enqueue(new Tuple<Vector2Int, int>(nextPos, currRange + 1));
            }
        }

        return traversableTiles;
    }
}
