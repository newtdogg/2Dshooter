using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;

public class LootManager : MonoBehaviour {

    private Dictionary<string, GameObject> lootTypes;
    public Dictionary<string, int> arenaStats;

    void Start() {
        lootTypes = new Dictionary<string, GameObject>() {
            { "scrap", GameObject.Find("Scrap") },
            { "recipe", GameObject.Find("Recipe") }
        };
        arenaStats = new Dictionary<string, int>() {
            { "scrapMin", 3 },
            { "scrapMax", 5 },
            { "scrapValue", 10 },
            { "recipeDropChance", 5 }
        };

    }

    public void spawnLoot(string lootType, Vector3 position, int quantity = 1, int spawnArea = 1) {
        for(var i = 0; i < quantity; i++) {
            var areaRadius = spawnArea * 50;
            var randX = new System.Random((int)System.DateTime.Now.Ticks + i);
            var randY = new System.Random((int)System.DateTime.Now.Ticks - i);
            var spawnX = randX.Next((int)(position.x * 100 - areaRadius), (int)(position.x * 100 + areaRadius));
            var spawnY = randY.Next(((int)position.y * 100 - areaRadius), ((int)position.y * 100 + areaRadius));
            Instantiate(lootTypes[lootType], new Vector3((float)spawnX/100, (float)spawnY/100, 0), Quaternion.identity);
        }
    }

    public void spawnArenaLoot(Vector3 position) {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var scrapQuantity = rand.Next(arenaStats["scrapMin"], arenaStats["scrapMax"]);
        spawnLoot("scrap", new Vector3(position.x, position.y - 0.8f, 0), scrapQuantity);
        var recipeRand = new System.Random((int)System.DateTime.Now.Ticks);
        var recipeChance = recipeRand.Next(0, arenaStats["recipeDropChance"]);
        if(recipeChance == 1) {
            spawnLoot("recipe", position);
        }
    }

}