using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject zombie;
    public GameObject zombieList;
    private GameObject perk;
    public int wave;
    private float waveDowntime;
    public int zombiesPerWave;
    private MapGenerator mapGenerator;
    public bool debug;


    void Start()
    {
        zombie = GameObject.Find("Zombie1");
        zombieList = GameObject.Find("Zombies");

        perk = GameObject.Find("Perk");
        wave = 0;
        waveDowntime = -1;
        zombiesPerWave = 3;
        mapGenerator = GameObject.Find("Map").GetComponent<MapGenerator>();
    }

    // Update is called once per frame
    void Update() {
        if(!debug) {
            if(mapGenerator.mapGenerated) {
                spawnZombies();
                mapGenerator.mapGenerated = false;
            }
        }
        // if(waveDowntime > 0) {
        //     waveDowntime -= Time.deltaTime;
        //     if (waveDowntime <= 0) {
        //         startWave();
        //     }
        // }
    }


    private void instantiateZombieObject (int x, int y) {
        var zombieClone = Instantiate(zombie, new Vector2(0, 0), Quaternion.identity) as GameObject;
        zombieClone.transform.SetParent(zombieList.transform);
        zombieClone.transform.position = new Vector2(x, y);
    }


    public void checkWaveComplete() {
        if(zombieList.transform.childCount == 1) {
            waveDowntime = 5;
            GameObject newPerk = Instantiate(perk, new Vector2(0, 0), Quaternion.identity) as GameObject;
            newPerk.GetComponent<Perk>().setStatTypeAndValue(wave);
        }
    }

    public void spawnZombies() {
        var map = mapGenerator.map;
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                var tile = map[x, y];
                if(tile.hasZombie) {
                    instantiateZombieObject(x, y);
                }
            }
        }
    }
}
