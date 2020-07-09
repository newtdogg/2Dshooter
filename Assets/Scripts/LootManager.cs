using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;

public class LootManager : MonoBehaviour {

    private Dictionary<string,GameObject> lootTypes;
    public Dictionary<string,Dictionary<string,int>> stats;

    void Start() {
        lootTypes = new Dictionary<string, GameObject>() {
            { "scrap", GameObject.Find("Scrap") },
            { "recipe", GameObject.Find("Recipe") }
        };
        stats = new Dictionary<string,Dictionary<string,int>>() {
            {
                "arena", new Dictionary<string, int>() {
                    { "scrapMin", 3 },
                    { "scrapMax", 5 },
                    { "scrapValue", 10 },
                    { "recipeDropChance", 5 }
                }
            },
            {
                "zombie", new Dictionary<string, int>() {
                    { "scrapMin", 1 },
                    { "scrapMax", 3 },
                    { "scrapValue", 6 },
                    { "recipeDropChance", 10 }
                }
            }
        };
    }

    public void generateLoot(string lootType, Vector3 position, int quantity = 1, int spawnArea = 1) {
        for(var i = 0; i < quantity; i++) {
            var areaRadius = spawnArea * 50;
            var randX = new System.Random((int)System.DateTime.Now.Ticks + i);
            var randY = new System.Random((int)System.DateTime.Now.Ticks - i);
            var spawnX = randX.Next((int)(position.x * 100 - areaRadius), (int)(position.x * 100 + areaRadius));
            var spawnY = randY.Next(((int)position.y * 100 - areaRadius), ((int)position.y * 100 + areaRadius));
            Instantiate(lootTypes[lootType], new Vector3((float)spawnX/100, (float)spawnY/100, 0), Quaternion.identity);
        }
    }

    public void dropArenaLoot(Vector3 position) {
        generateScrapPile("arena", position);
        generateLootFromDropchance("arena", position, "recipe");
    }

    public void dropZombieLoot(Vector3 position) {
        generateScrapPile("zombie", position);
        generateLootFromDropchance("zombie", position, "recipe");
    }

    public void generateScrapPile(string type, Vector3 position) {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var scrapQuantity = rand.Next(stats[type]["scrapMin"], stats[type]["scrapMax"]);
        generateLoot("scrap", new Vector3(position.x, position.y - 0.8f, 0), scrapQuantity);
    }

    public void generateLootFromDropchance(string type, Vector3 position, string loot) {
        var dropchanceKey = $"{loot}DropChance";
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var chance = rand.Next(0, stats[type][dropchanceKey]);
        if(chance == 1) {
            generateLoot(loot, position);
        }
    }

}