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
    public Vector3Int mapSE;
    public Vector3Int mapNE;
    public Vector3Int mapSW;
    public Vector3Int mapNW;

    void Start()
    {
        zombie = GameObject.Find("Zombie1");
        zombieList = GameObject.Find("Zombies");
        perk = GameObject.Find("Perk");

        wave = 0;
        waveDowntime = -1;
        zombiesPerWave = 3;
        startWave();
    }

    // Update is called once per frame
    void Update()
    {
        if(waveDowntime > 0) {
            waveDowntime -= Time.deltaTime;
            if (waveDowntime <= 0) {
                startWave();
            }
        }
    }

    public void startWave () {
        wave += 1;
        Debug.Log(wave);
        waveDowntime = -1;;
        var zombieCount = wave * zombiesPerWave;
        for (int i = 0; i < zombieCount; i++) {
            spawnZombie();
        }
    }

    private void spawnZombie () {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var axisRand = rand.Next(0, 4);
        if(axisRand == 0) {
            instantiateZombieObject(mapSE.y, mapNE.y, mapSE.x, false);
        } else if (axisRand == 1) {
            instantiateZombieObject(mapSW.y, mapNW.y, mapSW.x, false);
        } else if (axisRand == 2) {
            instantiateZombieObject(mapSW.x, mapSE.x, mapSW.y, true);
        } else if (axisRand == 3) {
            instantiateZombieObject(mapNW.x, mapNE.x, mapNE.y, true);
        }
    }

    private void instantiateZombieObject (int a, int b, int c, bool y) {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var randBetweenAxes = rand.Next(a, b);
        var zombiePosition = y == true ? new Vector2(randBetweenAxes, c) : new Vector2(c, randBetweenAxes);
        var zombieClone = Instantiate(zombie, new Vector2(0, 0), Quaternion.identity) as GameObject;
        zombieClone.transform.SetParent(zombieList.transform);
        zombieClone.transform.position = zombiePosition;
    }


    public void checkWaveComplete() {
        if(zombieList.transform.childCount == 1) {
            waveDowntime = 5;
            GameObject newPerk = Instantiate(perk, new Vector2(0, 0), Quaternion.identity) as GameObject;
            newPerk.GetComponent<Perk>().setStatTypeAndValue(wave);
        }
    }
}
