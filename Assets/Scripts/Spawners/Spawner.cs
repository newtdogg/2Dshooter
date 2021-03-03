using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum SpawnerType {
  Empty,
  Instant,
  Walled,
  Trap,
  Persistent,
  Source
}

public class Spawner : MonoBehaviour {

    private GameObject[] zombieTypes;
    private GameObject[] minibossTypes;
    private TileTools tileTools;
    public int id;
    public Transform playerTransform;
    public bool isWave;
    public Transform zombiesList;
    public int width;
    public int height;
    public List<string> zombiesToSpawn;
    public int initialZombieSpawn;
    public Vector3 centerOfObject;
    public SpawnerType type;
    public int zombieType;
    public bool battleStarted;
    public bool battleCompleted;
    public GameObject wallClone;
    public GameObject wallNodeClone;
    public bool empty;
    public List<List<Vector2Int>> walls;
    public LootController lootController;
    public GameObject zombieSourceClone;

    void Awake () {
        walls = new List<List<Vector2Int>>();
        battleStarted = false;
        empty = true;
        zombieSourceClone = GameObject.Find("SpawnerSource");
        zombieTypes = getSpawnerObjectsOfType("Zombies");
        minibossTypes = getSpawnerObjectsOfType("MiniBosses");
        wallClone = GameObject.Find("Wall");
        wallNodeClone = GameObject.Find("WallNode");
        lootController = GameObject.Find("LootController").GetComponent<LootController>();
        zombiesList = transform.GetChild(0);
        tileTools = GameObject.Find("TileTools").GetComponent<TileTools>();
    }

    void Update() {
        if(battleStarted && !battleCompleted && transform.GetChild(0).childCount == 0) {
            completeBattle();
        }
        if(!empty && zombiesList.childCount == 0) {
            empty = true;
        }
    }

    public void startSpawnerByType(List<string> zombieGroup) {
        zombiesToSpawn = zombieGroup;
        switch (type) {
            case SpawnerType.Empty:
                break;
            case SpawnerType.Instant:
                spawnZombieGroup();
                break;
            case SpawnerType.Walled:
                spawnZombieGroup();
                break;
            case SpawnerType.Persistent:
                StartCoroutine(spawnZombiesOverTime());
                break;
            case SpawnerType.Source:
                StartCoroutine(spawnZombiesOverTime());
                createZombieSourceObject();
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

    public IEnumerator spawnZombiesOverTime() {
        var zombieCount = 0;
        while (zombieCount < zombiesToSpawn.Count * 2) {
            yield return new WaitForSeconds (2f);
            if(Vector3.Distance(centerOfObject, playerTransform.position) < 20f) {
                System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
                var zombiePosX = pseudoRandom.Next(0, width);
                var zombiePosY = pseudoRandom.Next(0, height);
                var tileInt = tileTools.intMap[(int)Mathf.Floor(zombiePosX) + (int)transform.position.x, (int)Mathf.Floor(zombiePosY) + (int)transform.position.y];
                // Debug.Log($"{tileInt} ( {(int)Mathf.Floor(zombiePosX)} {(int)Mathf.Floor(zombiePosY)} )");
                if(tileInt != 1 && tileInt != 2) {
                    continue;
                }
                var zombieGameObject = Array.Find(zombieTypes, z => z.name == zombiesToSpawn[zombieCount]);
                spawnZombie(zombiePosX, zombiePosY, zombieGameObject);
                zombieCount++;
            }
        }
    }

    private void createZombieSourceObject() {
        var sourceObject = Instantiate(zombieSourceClone, centerOfObject, Quaternion.identity) as GameObject;
        var sourceObjectScript = sourceObject.GetComponent<SpawnerSource>();
        sourceObjectScript.hp = 200f;
        sourceObjectScript.spawner = this;
    }

    public void triggerWalls() {
        Debug.Log("battlestarted");
        battleStarted = true;
        if(type == SpawnerType.Trap) {
            spawnZombieGroup();
        }
        setWalls();

        // foreach (var wall in walls) {
        //     foreach (var block in wall) {
        //         if(tileTools.getTileInt(block.x, block.y) == 1 || tileTools.getTileInt(block.x, block.y) == 2) {
        //             tileTools.setWallTile(block.x, block.y);
        //         }
        //     }
        // }
    }

    public void setWalls() {
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

    public void completeBattle() {
        battleCompleted = true;
        lootController.dropArenaLoot(centerOfObject);
        transform.GetChild(1).gameObject.SetActive(false);
        // foreach (var wall in walls) {
        //     foreach (var block in wall) {
        //         tileTools.removeWallTile(block.x, block.y);
        //     }
        // }
    }

    public void spawnMiniboss() {
        Debug.Log("mini boss spawned");
        var bossClone = Instantiate(minibossTypes[zombieType - 10], centerOfObject, Quaternion.identity) as GameObject;
        bossClone.transform.SetParent(zombiesList);
    }

    public void spawnZombie(int x, int y, GameObject zombieTypeObject) {
        var zombieClone = Instantiate(zombieTypeObject, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity) as GameObject;
        zombieClone.transform.SetParent(zombiesList);
        zombieClone.GetComponent<ZombieController>().setDormant();
    }

    public void spawnZombieGroup() {
        empty = false;
        var quantity = zombiesToSpawn.Count;
        var distance = width / quantity;
        var seed = Time.time.ToString();
        var zombieCount = 0;
        while(zombieCount < quantity) {
            System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
            var zombiePosX = pseudoRandom.Next(zombieCount * distance, (zombieCount + 1) * distance);
            var zombiePosY = pseudoRandom.Next(0, height);
            var tileInt = tileTools.intMap[(int)Mathf.Floor(zombiePosX) + (int)transform.position.x, (int)Mathf.Floor(zombiePosY) + (int)transform.position.y];
            // Debug.Log($"{tileInt} ( {(int)Mathf.Floor(zombiePosX)} {(int)Mathf.Floor(zombiePosY)} )");
            if(tileInt != 1 && tileInt != 2) {
                continue;
            }
            var zombieGameObject = Array.Find(zombieTypes, z => z.name == zombiesToSpawn[zombieCount]);
            spawnZombie(zombiePosX, zombiePosY, zombieGameObject);
            zombieCount++;
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

    public void setAttributes(int spawnerInt, int index, Transform player) {
        playerTransform = player;
        var spawnerIntStr = spawnerInt.ToString();
        id = index;
        width = Int16.Parse(spawnerIntStr.Substring(0,2));
        height = Int16.Parse(spawnerIntStr.Substring(2,2));
        centerOfObject = new Vector3(transform.position.x + (width/2), transform.position.y + (height/2), 0);
        var spawnerTypeList = SpawnerType.GetValues(typeof(SpawnerType)) as SpawnerType[];
        System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
        type = spawnerTypeList[pseudoRandom.Next(0, spawnerTypeList.Length)];
        // type = SpawnerType.Walled;
        // Debug.Log(type);
        var numberOfWalls = Int16.Parse(spawnerIntStr.Substring(4,1));
        for(var i = 0; i < numberOfWalls; i++) {
            createBoundaryWall(Int16.Parse(spawnerIntStr.Substring(5 + i,1)));
        }
        setWallTriggers();
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
        var newWall = Instantiate(wallClone, wallCenter, Quaternion.identity) as GameObject;
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
