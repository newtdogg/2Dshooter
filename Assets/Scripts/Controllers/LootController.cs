using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System.IO;

public class LootController : MonoBehaviour {

    private Dictionary<string,GameObject> lootTypes;
    public DropChanceList stats;
    private AttachmentType[] attachmentList;

    void Start() {
        lootTypes = new Dictionary<string, GameObject>() {
            { "scrap", GameObject.Find("Scrap") },
            { "attachmentRecipe", GameObject.Find("AttachmentRecipe") }
        };
        var dropChanceJSON = File.ReadAllText("./Assets/Scripts/DropChance.json"); 
        stats = JsonUtility.FromJson<DropChanceList>(dropChanceJSON);
        // var attachmentJSON = File.ReadAllText("./Assets/Scripts/Attachments/Attachments.json");
        // var allAttachments = JsonUtility.FromJson<Attachments>(jsonString);
        attachmentList = AttachmentType.GetValues(typeof(AttachmentType)) as AttachmentType[];
    }

    public void generateLoot(string lootType, Vector3 position, int quantity = 1, int spawnArea = 1, int value = 0) {
        for(var i = 0; i < quantity; i++) {
            var areaRadius = spawnArea * 50;
            var randX = new System.Random((int)System.DateTime.Now.Ticks + i);
            var randY = new System.Random((int)System.DateTime.Now.Ticks - i);
            var spawnX = randX.Next((int)(position.x * 100 - areaRadius), (int)(position.x * 100 + areaRadius));
            var spawnY = randY.Next(((int)position.y * 100 - areaRadius), ((int)position.y * 100 + areaRadius));
            var lootItem = Instantiate(lootTypes[lootType], new Vector3((float)spawnX/100, (float)spawnY/100, 0), Quaternion.identity);
            if(lootType == "scrap") {
                lootItem.GetComponent<Scrap>().value = value;
            }
            if(lootType == "attachmentRecipe") {
                lootItem.GetComponent<AttachmentRecipe>().attachment = getRandomAttachmentType();
            }
        }
    }

    public AttachmentType getRandomAttachmentType() {
        System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
        return attachmentList[pseudoRandom.Next(0, attachmentList.Length)];
    }

    public void dropArenaLoot(Vector3 position) {
        generateScrapPile("Arena", position);
        generateLootFromDropchance("Arena", position, "attachmentRecipe");
    }

    public void dropZombieLoot(Vector3 position, string title) {
        generateScrapPile(title, position);
        generateLootFromDropchance(title, position, "attachmentRecipe");
    }

    public void dropMiniBossLoot(Vector3 position) {
        generateScrapPile("Miniboss", position);
    }

    public void generateScrapPile(string type, Vector3 position) {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var lootType = stats.GetType().GetProperty(type).GetValue(stats, null) as DropChance;
        var scrapQuantity = rand.Next(lootType.scrapMin, lootType.scrapMax);
        generateLoot("scrap", new Vector3(position.x, position.y - 0.8f, 0), scrapQuantity, 1, lootType.scrapValue);
    }

    private void generateLootFromDropchance(string type, Vector3 position, string loot) {
        var dropchanceKey = $"{loot}DropChance";
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var lootType = stats.GetType().GetProperty(type).GetValue(stats, null) as DropChance;
        // var dropChanceValue = (int)lootType.GetType().GetProperty(dropchanceKey).GetValue(lootType, null);
        // var chance = rand.Next(0, dropChanceValue);
        var chance = 0;
        if(chance == 0) {
            Debug.Log(loot);
            generateLoot(loot, position);
        }
    }

    public void dropPerk(Vector3 position) {
        
    }

}