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
        empty = false;
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
            count += 1;
        }
        return mobTypes;
    }

    public void spawnInitialZombies() {
        if((width * height)/ 4 < initialZombieSpawn) {
            Debug.Log("Area too small");
        }
        if(!isWave) {
            positionZombies();
        }
    }

    public void startBattle() {
        battleStarted = true;
        foreach (var wall in walls) {
            foreach (var block in wall) {
                tileTools.setWallTile(block.x, block.y);
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
            for(var i = 0; i < initialZombieSpawn; i++) {
                System.Random pseudoRandom = new System.Random(seed.GetHashCode() + i);
                var zombiePosX = pseudoRandom.Next(i * distance, (i + 1) * distance);
                var zombiePosY = pseudoRandom.Next(0, height);
                spawnZombie(zombiePosX, zombiePosY, zombieTypes[zombieType]);
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

    public void spawnZombie(int x, int y, GameObject zombieTypeArg) {
        GameObject zombieTypeObject = zombieTypeArg;
        var zombieClone = Instantiate(zombieTypeObject, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity) as GameObject;
        zombieClone.transform.SetParent(zombiesList);
        zombieClone.GetComponent<ZombieController>().setDormant();
    }

    public void spawnZombieGroup(int quantity, string zType) {
        var distance = width / quantity;
        var seed = Time.time.ToString();
        for(var i = 0; i < quantity; i++) {
            System.Random pseudoRandom = new System.Random(seed.GetHashCode() + i);
            var zombiePosX = pseudoRandom.Next(i * distance, (i + 1) * distance);
            var zombiePosY = pseudoRandom.Next(0, height);
            var zombieGameObject = Array.Find(zombieTypes, z => z.name == zType);
            spawnZombie(zombiePosX, zombiePosY, zombieGameObject);
        }
    }

    public void setAttributes(int spawnerInt) {
        var spawnerIntStr = spawnerInt.ToString();
        id = Int16.Parse(spawnerIntStr.Substring(0,1));
        width = Int16.Parse(spawnerIntStr.Substring(1,2));
        height = Int16.Parse(spawnerIntStr.Substring(3,2));
        centerOfObject = new Vector3(transform.position.x + (width/2), transform.position.y + (height/2), 0);
        isWave = spawnerIntStr.Substring(spawnerIntStr.Length - 3) == "000";
        if(!isWave) {
            initialZombieSpawn = Int16.Parse(spawnerIntStr.Substring(5,1));
            zombieType = Int16.Parse(spawnerIntStr.Substring(6, spawnerIntStr.Length - 6));
            if(zombieType < 10) {
                type = "zombie";
            } else {
                type = "miniboss";
            }
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
        var newWall = Instantiate(wallClone, wallCenter, Quaternion.identity) as GameObject;
        newWall.GetComponent<BoxCollider2D>().size = primaryDirection == "y" ? new Vector2(1, wall.Count) : new Vector2(wall.Count, 1);
        newWall.transform.SetParent(transform);
    }
}
