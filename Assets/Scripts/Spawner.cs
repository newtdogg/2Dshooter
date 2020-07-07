using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour {

    private GameObject[] zombieTypes;
    private TileTools tileTools;
    public int id;
    public Transform zombiesList;
    public int width;
    public int height;
    public int initialZombieSpawn;
    public int zombieType;
    public bool battleStarted;
    public bool battleCompleted;
    public List<List<Vector2Int>> walls;

    void Awake () {
        walls = new List<List<Vector2Int>>();
        battleStarted = false;
        zombieTypes = new GameObject[] {
            GameObject.Find("Lubber"),
            GameObject.Find("Ripper")
        };
        zombiesList = transform.GetChild(0);
        tileTools = GameObject.Find("TileTools").GetComponent<TileTools>();
    }

    void Update() {
        if(battleStarted && !battleCompleted && transform.GetChild(0).childCount == 0) {
            completeBattle();
        }
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
    }

    public void completeBattle() {
        battleCompleted = true;
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
        var distance = width / initialZombieSpawn;
        var seed = Time.time.ToString();
        for(var i = 0; i < initialZombieSpawn; i++) {
            System.Random pseudoRandom = new System.Random(seed.GetHashCode() + i);
            var zombiePosX = pseudoRandom.Next(i * distance, (i + 1) * distance);
            var zombiePosY = pseudoRandom.Next(0, height);
            spawnZombie(zombiePosX, zombiePosY, zombieType);
        }
    }

    public void spawnZombie(int x, int y, int type) {
        GameObject zombieTypeObject = zombieTypes[type];
        var zombieClone = Instantiate(zombieTypeObject, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity) as GameObject;
        zombieClone.transform.SetParent(zombiesList);
    }

    public void setAttributes(int spawnerInt) {
        var spawnerIntStr = spawnerInt.ToString();
        id = Int16.Parse(spawnerIntStr.Substring(0,1));
        width = Int16.Parse(spawnerIntStr.Substring(1,2));
        height = Int16.Parse(spawnerIntStr.Substring(3,2));
        initialZombieSpawn = Int16.Parse(spawnerIntStr.Substring(5,1));
        zombieType = Int16.Parse(spawnerIntStr.Substring(6,1));
    }
}
