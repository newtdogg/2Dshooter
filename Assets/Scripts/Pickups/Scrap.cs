using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : Pickup {

    public List<string> scrapList;

    void Start() {
        type = "Scrap";
    }

    public Dictionary<string, int> getScrapDictionary() {
        var scrapDict = new Dictionary<string, int>();
        foreach (var scrapStr in scrapList){
             if(scrapDict.ContainsKey(scrapStr)) {
                scrapDict[scrapStr] += 1;
            } else {
                scrapDict.Add(scrapStr, 1);
            }
        }
        return scrapDict;
    }
    protected override void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.name == "Player") {
            var player = col.gameObject.GetComponent<PlayerController>();
            var scrapDict = getScrapDictionary();
            player.updateScrap(scrapDict);
            player.pickupScrapUI(scrapDict);
            Destroy(gameObject);
        }
    }
}
