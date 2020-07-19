using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour {

    private GameObject[] zombieTypes;
    private GameObject[] minibossTypes;
    private TileTools tileTools;
    public int id;
    public Transform zombiesList;
    public int width;
    public int height;
    public string type;
    public int initialZombieSpawn;
    public Vector3 centreOfObject;
    public int zombieType;
    public bool battleStarted;
    public bool battleCompleted;
    public List<List<Vector2Int>> walls;
    public LootController lootController;

    void Awake () {
        walls = new List<List<Vector2Int>>();
        battleStarted = false;
        zombieTypes = getSpawnerObjectsOfType("Zombies");
        minibossTypes = getSpawnerObjectsOfType("MiniBosses");
        lootController = GameObject.Find("LootController").GetComponent<LootController>();
        zombiesList = transform.GetChild(0);
        tileTools = GameObject.Find("TileTools").GetComponent<TileTools>();
    }

    void Update() {
        if(battleStarted && !battleCompleted && transform.GetChild(0).childCount == 0) {
            completeBattle();
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
        positionZombies();
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
        lootController.dropArenaLoot(centreOfObject);
        foreach (var wall in walls) {
            foreach (var block in wall) {
                tileTools.removeWallTile(block.x, block.y);
            }
        }
        generateLoot();
    }

    public void generateLoot() {

    }

    public void positionZombies() {
        if(type == "zombie") {
            var distance = width / initialZombieSpawn;
            var seed = Time.time.ToString();
            for(var i = 0; i < initialZombieSpawn; i++) {
                System.Random pseudoRandom = new System.Random(seed.GetHashCode() + i);
                var zombiePosX = pseudoRandom.Next(i * distance, (i + 1) * distance);
                var zombiePosY = pseudoRandom.Next(0, height);
                spawnZombie(zombiePosX, zombiePosY);
            }
        }
        if(type == "miniboss") {
            // var miniboss = 
            spawnMiniboss();
        }
    }

    public void spawnMiniboss() {
        var bossClone = Instantiate(minibossTypes[zombieType - 10], centreOfObject, Quaternion.identity) as GameObject;
        bossClone.transform.SetParent(zombiesList);
    }

    public void spawnZombie(int x, int y) {
        GameObject zombieTypeObject = zombieTypes[zombieType];
        var zombieClone = Instantiate(zombieTypeObject, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity) as GameObject;
        zombieClone.transform.SetParent(zombiesList);
    }



    public void setAttributes(int spawnerInt) {
        var spawnerIntStr = spawnerInt.ToString();
        id = Int16.Parse(spawnerIntStr.Substring(0,1));
        width = Int16.Parse(spawnerIntStr.Substring(1,2));
        height = Int16.Parse(spawnerIntStr.Substring(3,2));
        initialZombieSpawn = Int16.Parse(spawnerIntStr.Substring(5,1));
        zombieType = Int16.Parse(spawnerIntStr.Substring(6, spawnerIntStr.Length - 6));
        if(zombieType < 10) {
            type = "zombie";
        } else {
            type = "miniboss";
        }
        centreOfObject = new Vector3(transform.position.x + (width/2), transform.position.y + (height/2), 0);
    }
}
