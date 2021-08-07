using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class LootController : MonoBehaviour {

    private Dictionary<string,GameObject> lootTypes;
    public DropChanceList stats;
    private Mobs mobData;
    public int multiplierMin;
    public int multiplierMax;
    private AttachmentType[] attachmentList;

    void Start() {
        lootTypes = new Dictionary<string, GameObject>() {
            { "scrap", GameObject.Find("Scrap") },
            { "attachmentRecipe", GameObject.Find("AttachmentRecipe") }
        };
        multiplierMin = 1;
        multiplierMax = 4;
        var scrapJSON = File.ReadAllText("./Assets/Scripts/Scrap.json"); 
        stats = JsonConvert.DeserializeObject<DropChanceList>(scrapJSON);
        var mobJsonString = File.ReadAllText("./Assets/Scripts/Mobs.json");
        mobData = JsonConvert.DeserializeObject<Mobs>(mobJsonString);
        // var attachmentJSON = File.ReadAllText("./Assets/Scripts/Attachments/Attachments.json");
        // var allAttachments = JsonUtility.FromJson<Attachments>(jsonString);
        attachmentList = AttachmentType.GetValues(typeof(AttachmentType)) as AttachmentType[];
    }

    private void generateScrapObject(List<string> totalScrap, Vector3 position) {
        var pos = new Vector2(Mathf.Floor(position.x) + 0.5f, Mathf.Floor(position.y) + 0.5f);
        var scrapObject = Instantiate(lootTypes["scrap"], pos, Quaternion.identity);
        scrapObject.GetComponent<Scrap>().scrapList = totalScrap;
    }

    public AttachmentType getRandomAttachmentType() {
        System.Random pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
        return attachmentList[pseudoRandom.Next(0, attachmentList.Length)];
    }

    // public void dropArenaLoot(Vector3 position) {
    //     generateScrapPile("Arena", position);
    //     generateLootFromDropchance("Arena", position, "attachmentRecipe");
    // }

    public void dropMobLoot(Vector3 position, string title) {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var lootQuantity = rand.Next(multiplierMin, multiplierMax);
        var allScrap = new List<string>();
        var deadMobData = mobData.GetType().GetProperty(title).GetValue(mobData, null) as MobStats;
        var mobDrops = deadMobData.scrapLoot;

        for(var i = 0; i < lootQuantity; i++) {
            var lootRand = new System.Random((int)System.DateTime.Now.Ticks + 1);
            var rarityInt = rand.Next(0, 100);
            if(rarityInt > 95) {
                allScrap.Add(mobDrops["rare"][lootRand.Next(mobDrops["rare"].Count)]);
            } else if (rarityInt > 75) {
                allScrap.Add(mobDrops["uncommon"][lootRand.Next(mobDrops["uncommon"].Count)]);
            } else {
                allScrap.Add(mobDrops["common"][lootRand.Next(mobDrops["common"].Count)]);
            }
        }
        generateScrapObject(allScrap, position);
    }

    // public void dropMiniBossLoot(Vector3 position) {
    //     generateScrapPile("Miniboss", position);
    // }

    // public void generateScrapPile(string type, Vector3 position) {
    //     var rand = new System.Random((int)System.DateTime.Now.Ticks);
    //     var lootType = stats.GetType().GetProperty(type).GetValue(stats, null) as DropChance;
    //     var scrapQuantity = rand.Next(lootType.scrapMin, lootType.scrapMax);
    //     generateLoot("scrap", new Vector3(position.x, position.y - 0.8f, 0), scrapQuantity, 1, lootType.scrapValue);
    // }

    // private void generateLootFromDropchance(string type, Vector3 position, string loot) {
    //     var dropchanceKey = $"{loot}DropChance";
    //     var rand = new System.Random((int)System.DateTime.Now.Ticks);
    //     var lootType = stats.GetType().GetProperty(type).GetValue(stats, null) as DropChance;
    //     // var dropChanceValue = (int)lootType.GetType().GetProperty(dropchanceKey).GetValue(lootType, null);
    //     // var chance = rand.Next(0, dropChanceValue);
    //     var chance = 0;
    //     if(chance == 0) {
    //         Debug.Log(loot);
    //         generateLoot(loot, position);
    //     }
    // }

    public void dropPerk(Vector3 position) {
        
    }

}