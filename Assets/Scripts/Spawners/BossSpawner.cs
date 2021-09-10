using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossSpawner : Spawner {

    void Awake () {
        battleStarted = false;
        empty = true;
        wallTriggerClone = GameObject.Find("WallTrigger");
        wallNodeClone = GameObject.Find("WallNode");
    }

    public void createWallLists(Vector2Int cellVector) {
        walls = new List<List<Vector2Int>>();
        var baseX = cellVector.x * 64;
        var baseY = cellVector.y * 64;
        for(var i = 0; i < 4; i++) {
            var newWall = new List<Vector2Int>();
            var constantVal = i % 2 == 0 ? 59 : 5;
            for(var j = 28; j < 36; j++) {
                if(i > 1) {
                    newWall.Add(new Vector2Int(baseX + j, constantVal));
                } else {
                    newWall.Add(new Vector2Int(constantVal, baseY + j));
                }
            }
            walls.Add(newWall);
        }
    }
}