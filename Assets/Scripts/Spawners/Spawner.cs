using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpawnerType {
  Empty,
  Default,
  Walled,
  Trap,
  Persistent,
  Source,
  Boss
}

public class Spawner : MonoBehaviour {
    public SpawnerType type;
    public bool empty;
    public bool battleStarted;
    public bool battleCompleted;
    public GameObject wallNodeClone;
	public List<List<Vector2Int>> walls;
    public GameObject wallTriggerClone;
	public Vector3 centerOfObject;

    public virtual void triggerWalls() {}

    public void createWallTrigger(List<Vector2Int> wall, Vector2 wallCenter, string primaryDirection) {
        var newWall = Instantiate(wallTriggerClone, wallCenter, Quaternion.identity) as GameObject;
        newWall.GetComponent<BoxCollider2D>().size = primaryDirection == "y" ? new Vector2(1, wall.Count) : new Vector2(wall.Count, 1);
        newWall.transform.SetParent(transform);
    }

	public void setWallTriggers() {
        Debug.Log(walls.Count);
        // foreach (var wall in walls) {
        //     var wallCenterVector2 = wall[(int)(Mathf.Round(wall.Count/2))];
        //     var primaryDirection = wall[0].x == wall[1].x ? "y" : "x";
        //     var wallCenter = primaryDirection == "y" ? wall[0].y + wall.Count/2 : wall[0].x + wall.Count/2;
        //     var wallCenterVector3 = primaryDirection == "y" ? new Vector3(wall[0].x, wallCenter, 0) : new Vector3(wallCenter, wall[0].y, 0);
        //     var wallDirectionFromCenter = (wallCenterVector3 - centerOfObject).normalized;
        //     var offset = primaryDirection == "y" ? Mathf.Round(wallDirectionFromCenter.x) : Mathf.Round(wallDirectionFromCenter.y);
        //     if(primaryDirection == "y") {
        //         createWallTrigger(wall, new Vector3(wall[0].x - (offset * 2), wallCenter, 0), primaryDirection);
        //     } else {
        //         createWallTrigger(wall, new Vector3(wallCenter, wall[0].y - (offset * 2), 0), primaryDirection);
        //     }
        // }
    }
}
