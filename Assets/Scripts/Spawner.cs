using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    private GameObject[] zombieTypes;
    public int width;
    public int height;
    public int initialZombieSpawn;
    public int zombieType;

    void Awake () {
        zombieTypes = new GameObject[] {
            GameObject.Find("Lubber"),
            GameObject.Find("Ripper")
        };
    }


    public void spawnInitialZombies() {
        if((width * height)/ 4 < initialZombieSpawn) {
            Debug.Log("Area too small");
        }
        positionZombies();
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

    }
}
