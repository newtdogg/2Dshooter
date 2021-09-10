using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=UnityEngine.Random;
using System.Linq;

public class MobSpawner : Spawner {

    private GameObject[] mobTypes;
    private TileTools tileTools;
    public Vector2Int id;
    public Transform playerTransform;
    public bool isWave;
    public Transform mobsList;
    public int width;
    public int spawnableWidth => width - 1;
    public int height;
    public int spawnableHeight => height - 1;
    public List<string> mobsToSpawn;
    public int initialMobSpawn;
    public Action keyPickupCallback;
    public int mobType;
    public bool holdsItemKey;
    public Vector2Int cellVector;
    private GameObject roomKey;
    public string boss;
    public bool droppedKey = false;
    public bool active = true;
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

    public void mobDeathCallback() {
        if(mobsList.childCount == 1 && holdsItemKey) {
            var roomKeyClone = Instantiate(roomKey, centerOfObject, Quaternion.identity);
            roomKeyClone.GetComponent<Key>().pickupCallback = keyPickupCallback;
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
        active = true;
        while (mobCount < mobsToSpawn.Count * 2) {
            yield return new WaitForSeconds (2f);
            if(Vector3.Distance(centerOfObject, playerTransform.position) < 20f) {
                var mobPosX = Random.Range(0, width);
                var mobPosY = Random.Range(0, height);
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

    public override void triggerWalls() {
        Debug.Log("battlestarted");
        battleStarted = true;
        if(type == SpawnerType.Trap) {
            spawnMobGroup();
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

    public void completeBattle() {
        battleCompleted = true;
        // lootController.dropArenaLoot(centerOfObject);
        transform.GetChild(1).gameObject.SetActive(false);
        // foreach (var wall in walls) {
        //     foreach (var block in wall) {
        //         tileTools.removeWallTile(block.x, block.y);
        //     }
        // }
    }

    public void spawnMob(int x, int y, GameObject mobTypeObject) {
        var mobClone = Instantiate(mobTypeObject, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity) as GameObject;
        mobClone.transform.SetParent(mobsList);
        mobClone.name = $"{mobClone.name}{mobClone.transform.parent.childCount}";
        mobClone.GetComponent<MobController>().setDormant();
        mobClone.GetComponent<MobController>().deathCallback = mobDeathCallback;
    }

    public void spawnMobGroup() {
        empty = false;
        var quantity = mobsToSpawn.Count;
        var distance = spawnableWidth / quantity;
        var seed = Time.time.ToString();
        var mobCount = 0;
        while(mobCount < quantity) {
            var mobPosX = Random.Range(mobCount * distance, (mobCount + 1) * distance);
            var mobPosY = Random.Range(0, spawnableHeight);
            var tileInt = tileTools.intMap[(int)Mathf.Floor(mobPosX) + (int)transform.position.x, (int)Mathf.Floor(mobPosY) + (int)transform.position.y];
            // Debug.Log($"{tileInt} ( {(int)Mathf.Floor(mobPosX)} {(int)Mathf.Floor(mobPosY)} )");
            if(tileInt != 1 && tileInt != 2) {
                continue;
            }
            var mobGameObject = Array.Find(mobTypes, z => z.name == mobsToSpawn[mobCount]);
            spawnMob(mobPosX, mobPosY, mobGameObject);
            mobCount++;
        }
        active = true;
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


        var spawnerTypeList = SpawnerType.GetValues(typeof(SpawnerType)) as SpawnerType[];
        System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
        type = spawnerTypeList[pseudoRandom.Next(0, spawnerTypeList.Length - 1)];
        type = SpawnerType.Walled;
        Debug.Log(type);


        var numberOfWalls = Int16.Parse(spawnerIntStr.Substring(4,1));
        for(var i = 0; i < numberOfWalls; i++) {
            createBoundaryWall(Int16.Parse(spawnerIntStr.Substring(5 + i,1)));
        }
        setWallTriggers();
    }

    public void setBossSpawnerAttributes(int index, Transform player) {
        playerTransform = player;
        // id = index;
        width = 64;
        height = 64;
        type = SpawnerType.Boss;
        for(var i = 0; i < 4; i++) {
            createBoundaryWall(i);
        }
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
