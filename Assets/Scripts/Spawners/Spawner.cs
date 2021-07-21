using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

    private GameObject[] mobTypes;
    private TileTools tileTools;
    public Vector2Int id;
    public Transform playerTransform;
    public bool isWave;
    public Transform mobsList;
    public int width;
    public int height;
    public List<string> mobsToSpawn;
    public int initialMobSpawn;
    public Action keyPickupCallback;
    public Vector3 centerOfObject;
    public SpawnerType type;
    public int mobType;
    public bool battleStarted;
    public bool battleCompleted;
    public GameObject wallTriggerClone;
    public bool holdsItemKey;
    public Vector2Int cellVector;
    public GameObject wallNodeClone;
    private GameObject roomKey;
    public bool empty;
    public string boss;
    public List<List<Vector2Int>> walls;
    public LootController lootController;
    public GameObject mobSourceClone;

    void Awake () {
        walls = new List<List<Vector2Int>>();
        battleStarted = false;
        empty = true;
        roomKey = GameObject.Find("RoomKey");
        mobSourceClone = GameObject.Find("SpawnerSource");
        mobTypes = getSpawnerObjectsOfType("Mobs");
        wallTriggerClone = GameObject.Find("WallTrigger");
        wallNodeClone = GameObject.Find("WallNode");
        lootController = GameObject.Find("LootController").GetComponent<LootController>();
        mobsList = transform.GetChild(0);
        tileTools = GameObject.Find("TileTools").GetComponent<TileTools>();
    }

    void Update() {
        if(battleStarted && !battleCompleted && transform.GetChild(0).childCount == 0) {
            completeBattle();
        }
        if(!empty && mobsList.childCount == 0) {
            empty = true;
        }
    }

    public void startSpawnerByType(List<string> mobGroup) {
        mobsToSpawn = mobGroup;
        switch (type) {
            case SpawnerType.Empty:
                break;
            case SpawnerType.Default:
                spawnMobGroup();
                break;
            case SpawnerType.Walled:
                spawnMobGroup();
                break;
            case SpawnerType.Persistent:
                StartCoroutine(spawnMobsOverTime());
                break;
            case SpawnerType.Source:
                StartCoroutine(spawnMobsOverTime());
                createMobSourceObject();
                break;
        }
    }

    public GameObject[] getSpawnerObjectsOfType(string type) {
        var mobObjects = GameObject.Find(type);
        var mobTypes = new GameObject[mobObjects.transform.childCount];
        var count = 0;
        foreach (Transform child in mobObjects.transform) {
            mobTypes[count] = child.gameObject;
            count++;
        }
        return mobTypes;
    }

    public IEnumerator spawnMobsOverTime() {
        var mobCount = 0;
        while (mobCount < mobsToSpawn.Count * 2) {
            yield return new WaitForSeconds (2f);
            if(Vector3.Distance(centerOfObject, playerTransform.position) < 20f) {
                System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
                var mobPosX = pseudoRandom.Next(0, width);
                var mobPosY = pseudoRandom.Next(0, height);
                var tileInt = tileTools.intMap[(int)Mathf.Floor(mobPosX) + (int)transform.position.x, (int)Mathf.Floor(mobPosY) + (int)transform.position.y];
                // Debug.Log($"{tileInt} ( {(int)Mathf.Floor(mobPosX)} {(int)Mathf.Floor(mobPosY)} )");
                if(tileInt != 1 && tileInt != 2) {
                    continue;
                }
                var mobGameObject = Array.Find(mobTypes, z => z.name == mobsToSpawn[mobCount]);
                spawnMob(mobPosX, mobPosY, mobGameObject);
                mobCount++;
            }
        }
    }

    private void createMobSourceObject() {
        var sourceObject = Instantiate(mobSourceClone, centerOfObject, Quaternion.identity) as GameObject;
        var sourceObjectScript = sourceObject.GetComponent<SpawnerSource>();
        sourceObjectScript.hp = 200f;
        sourceObjectScript.spawner = this;
    }

    public void triggerWalls() {
        Debug.Log("battlestarted");
        battleStarted = true;
        if(type == SpawnerType.Trap) {
            spawnMobGroup();
        } 
        if (type == SpawnerType.Boss) {
            startBossFight();
        } else {
            setWalls(true);
        }
    }

    // TODO: walls destructable toggle

    public void setWalls(bool destroyable) {
        var wallParent = transform.GetChild(1);
        foreach (List<Vector2Int> wall in walls) {
            foreach(Vector2Int wallNode in wall) {
                if(tileTools.getTileInt(wallNode.x, wallNode.y) != 0) {
                    var wallNodeObj = Instantiate(wallNodeClone, new Vector2(wallNode.x + 0.5f, wallNode.y + 0.5f), Quaternion.identity) as GameObject;
                    wallNodeObj.transform.SetParent(wallParent);
                }
            }
        }
    }

    public void startBossFight() {
        var bossClone = GameObject.Find(boss);
        var bossObject = Instantiate(bossClone, centerOfObject, Quaternion.identity) as GameObject;
        bossClone.transform.SetParent(mobsList);
        setWalls(false);
    }

    public void completeBattle() {
        battleCompleted = true;
        lootController.dropArenaLoot(centerOfObject);
        transform.GetChild(1).gameObject.SetActive(false);
        if (holdsItemKey)
        {
            var roomKeyClone = Instantiate(roomKey, centerOfObject, Quaternion.identity);
            roomKeyClone.GetComponent<Key>().pickupCallback = keyPickupCallback;
        }
        // foreach (var wall in walls) {
        //     foreach (var block in wall) {
        //         tileTools.removeWallTile(block.x, block.y);
        //     }
        // }
    }

    public void spawnMob(int x, int y, GameObject mobTypeObject) {
        var mobClone = Instantiate(mobTypeObject, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity) as GameObject;
        mobClone.transform.SetParent(mobsList);
        mobClone.GetComponent<MobController>().setDormant();
    }

    public void spawnMobGroup() {
        empty = false;
        var quantity = mobsToSpawn.Count;
        var distance = width / quantity;
        var seed = Time.time.ToString();
        var mobCount = 0;
        while(mobCount < quantity) {
            System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
            var mobPosX = pseudoRandom.Next(mobCount * distance, (mobCount + 1) * distance);
            var mobPosY = pseudoRandom.Next(0, height);
            var tileInt = tileTools.intMap[(int)Mathf.Floor(mobPosX) + (int)transform.position.x, (int)Mathf.Floor(mobPosY) + (int)transform.position.y];
            // Debug.Log($"{tileInt} ( {(int)Mathf.Floor(mobPosX)} {(int)Mathf.Floor(mobPosY)} )");
            if(tileInt != 1 && tileInt != 2) {
                continue;
            }
            var mobGameObject = Array.Find(mobTypes, z => z.name == mobsToSpawn[mobCount]);
            spawnMob(mobPosX, mobPosY, mobGameObject);
            mobCount++;
        }
    }

    public void createBoundaryWall(int edge) {
        var edgeWall = new List<Vector2Int>();
        int wallX; int wallY;
        switch (edge) {
            case 1:
                wallY = (int)getTopLeftCorner().y;
                for(wallX = (int)getTopLeftCorner().x; wallX < (int)getTopRightCorner().x; wallX++) {
                    edgeWall.Add(new Vector2Int(wallX, wallY));
                }
                break;
            case 2:
                wallX = (int)getBottomRightCorner().x;
                for(wallY = (int)getBottomRightCorner().y; wallY < (int)getTopRightCorner().y; wallY++) {
                    edgeWall.Add(new Vector2Int(wallX, wallY));
                }
                break;
            case 3:
                wallY = (int)getBottomLeftCorner().y;
                for(wallX = (int)getBottomLeftCorner().x; wallX < (int)getBottomRightCorner().x; wallX++) {
                    edgeWall.Add(new Vector2Int(wallX, wallY));
                }
                break;
            case 4:
                wallX = (int)getBottomLeftCorner().x;
                for(wallY = (int)getBottomLeftCorner().y; wallY < (int)getTopLeftCorner().y; wallY++) {
                    edgeWall.Add(new Vector2Int(wallX, wallY));
                }
                break;
        }
        walls.Add(edgeWall);
    }

    public void setAttributes(int spawnerInt, Vector2Int spawnerId, Transform player) {
        playerTransform = player;
        var spawnerIntStr = spawnerInt.ToString();
        id = spawnerId;
        width = Int16.Parse(spawnerIntStr.Substring(0,2));
        height = Int16.Parse(spawnerIntStr.Substring(2,2));
        centerOfObject = new Vector3(transform.position.x + (width/2), transform.position.y + (height/2), 0);
        // var spawnerTypeList = SpawnerType.GetValues(typeof(SpawnerType)) as SpawnerType[];
        // System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
        // type = spawnerTypeList[pseudoRandom.Next(0, spawnerTypeList.Length - 1)];
        // type = SpawnerType.Walled;
        // Debug.Log(type);
        var numberOfWalls = Int16.Parse(spawnerIntStr.Substring(4,1));
        for(var i = 0; i < numberOfWalls; i++) {
            createBoundaryWall(Int16.Parse(spawnerIntStr.Substring(5 + i,1)));
        }
        setWallTriggers();
    }

    public void setBossSpawnerAttributes(Vector2Int spawnerInt, int index, Transform player) {
        playerTransform = player;
        // id = index;
        width = 64;
        height = 64;
        type = SpawnerType.Boss;
        for(var i = 0; i < 4; i++) {
            createBoundaryWall(i);
        }
    }

    public void setWallTriggers() {
        foreach (var wall in walls) {
            var wallCenterVector2 = wall[(int)(Mathf.Round(wall.Count/2))];
            var primaryDirection = wall[0].x == wall[1].x ? "y" : "x";
            var wallCenter = primaryDirection == "y" ? wall[0].y + wall.Count/2 : wall[0].x + wall.Count/2;
            var wallCenterVector3 = primaryDirection == "y" ? new Vector3(wall[0].x, wallCenter, 0) : new Vector3(wallCenter, wall[0].y, 0);
            var wallDirectionFromCenter = (wallCenterVector3 - centerOfObject).normalized;
            var offset = primaryDirection == "y" ? Mathf.Round(wallDirectionFromCenter.x) : Mathf.Round(wallDirectionFromCenter.y);
            if(primaryDirection == "y") {
                createWallTrigger(wall, new Vector3(wall[0].x - (offset * 2), wallCenter, 0), primaryDirection);
            } else {
                createWallTrigger(wall, new Vector3(wallCenter, wall[0].y - (offset * 2), 0), primaryDirection);
            }
        }
    }

    public void createWallTrigger(List<Vector2Int> wall, Vector2 wallCenter, string primaryDirection) {
        var newWall = Instantiate(wallTriggerClone, wallCenter, Quaternion.identity) as GameObject;
        newWall.GetComponent<BoxCollider2D>().size = primaryDirection == "y" ? new Vector2(1, wall.Count) : new Vector2(wall.Count, 1);
        newWall.transform.SetParent(transform);
    }

    public Vector3 getTopLeftCorner() {
        return new Vector3(transform.position.x, transform.position.y + height, 0);
    }

    public Vector3 getBottomLeftCorner() {
        return transform.position;
    }

    public Vector3 getTopRightCorner() {
        return new Vector3(transform.position.x + width, transform.position.y + height, 0);
    }

    public Vector3 getBottomRightCorner() {
        return new Vector3(transform.position.x + width, transform.position.y, 0);
    }
}
