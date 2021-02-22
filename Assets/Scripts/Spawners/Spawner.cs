using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour {

    private GameObject[] zombieTypes;
    private GameObject[] minibossTypes;
    private TileTools tileTools;
    public int id;
    public bool isWave;
    public Transform zombiesList;
    public int width;
    public int height;
    public string type;
    public int initialZombieSpawn;
    public Vector3 centerOfObject;
    public int zombieType;
    public bool battleStarted;
    public bool battleCompleted;
    public GameObject wallClone;
    public bool empty;
    public List<List<Vector2Int>> walls;
    public LootController lootController;

    void Awake () {
        walls = new List<List<Vector2Int>>();
        battleStarted = false;
        empty = true;
        zombieTypes = getSpawnerObjectsOfType("Zombies");
        minibossTypes = getSpawnerObjectsOfType("MiniBosses");
        wallClone = GameObject.Find("Wall");
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

    public void spawnInitialZombies() {
        // if((width * height)/ 4 < initialZombieSpawn) {
        //     Debug.Log("Area too small");
        // }
        if(!isWave) {
            positionZombies();
        }
    }

    public void startBattle() {
        battleStarted = true;
        foreach (var wall in walls) {
            foreach (var block in wall) {
                if(tileTools.intMap[block.x, block.y] == 1 || tileTools.intMap[block.x, block.y] == 2) {
                    tileTools.setWallTile(block.x, block.y);
                }
            }
        }
        if(type == "miniboss"){
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<MiniBoss>().startFight();
        }
    }

    public void completeBattle() {
        battleCompleted = true;
        lootController.dropArenaLoot(centerOfObject);
        foreach (var wall in walls) {
            foreach (var block in wall) {
                tileTools.removeWallTile(block.x, block.y);
            }
        }
    }

    public void positionZombies() {
        if(type == "zombie") {
            var distance = width / initialZombieSpawn;
            var seed = Time.time.ToString();
            var zombieCount = 0;
            while(zombieCount < initialZombieSpawn) {
                System.Random pseudoRandom = new System.Random(seed.GetHashCode() + id + zombieCount);
                var zombiePosX = pseudoRandom.Next(zombieCount * distance, (zombieCount + 1) * distance);
                var zombiePosY = pseudoRandom.Next(0, height);
                var tileInt = tileTools.intMap[(int)Mathf.Floor(zombiePosX), (int)Mathf.Floor(zombiePosY)];
                if(tileInt != 1) {
                    continue;
                }
                spawnZombie(zombiePosX, zombiePosY, zombieTypes[zombieType]);
                zombieCount++;
            }
        }
        if(type == "miniboss") {
            // var miniboss = 
            spawnMiniboss();
        }
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

    public void spawnZombieGroup(List<string> zombieGroup) {
        empty = false;
        var quantity = zombieGroup.Count;
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
            var zombieGameObject = Array.Find(zombieTypes, z => z.name == zombieGroup[zombieCount]);
            spawnZombie(zombiePosX, zombiePosY, zombieGameObject);
            zombieCount++;
        }
    }

    public void createBoundaryWall(int edge) {
        Debug.Log($"boundary wall: side = {edge}");
        var edgeWall = new List<Vector2Int>();
        if(edge == 1) {
            var wallY = (int)getTopLeftCorner().y;
            for(var wallX = (int)getTopLeftCorner().x; wallX < (int)getTopRightCorner().x; wallX++) {
                edgeWall.Add(new Vector2Int(wallX, wallY));
            }
        } else if (edge == 2) {
            var wallX = (int)getBottomRightCorner().x;
            for(var wallY = (int)getBottomRightCorner().y; wallY < (int)getTopRightCorner().y; wallY++) {
                edgeWall.Add(new Vector2Int(wallX, wallY));
            }
        } else if (edge == 3) {
            var wallY = (int)getBottomLeftCorner().y;
            for(var wallX = (int)getBottomLeftCorner().x; wallX < (int)getBottomRightCorner().x; wallX++) {
                edgeWall.Add(new Vector2Int(wallX, wallY));
            }
        } else if (edge == 4) {
            var wallX = (int)getBottomLeftCorner().x;
            for(var wallY = (int)getBottomLeftCorner().y; wallY < (int)getTopLeftCorner().y; wallY++) {
                edgeWall.Add(new Vector2Int(wallX, wallY));
            }
        }
        walls.Add(edgeWall);
    }

    public void setAttributes(int spawnerInt, int index) {
        var spawnerIntStr = spawnerInt.ToString();
        id = index;
        width = Int16.Parse(spawnerIntStr.Substring(0,2));
        height = Int16.Parse(spawnerIntStr.Substring(2,2));
        centerOfObject = new Vector3(transform.position.x + (width/2), transform.position.y + (height/2), 0);
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
