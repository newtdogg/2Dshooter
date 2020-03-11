using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perk : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController playerController;
    public string stat;
    public float increase;
    void Start() {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update() {
        
    }

    public void setStatTypeAndValue(float value) {
        increase = value;
        var perkList = new List<string>() { "damage", "ammoCapacity" };
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var index = rand.Next(perkList.Count);
        stat = perkList[index];

    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player" || col.gameObject.name == "Bullet(Clone)"){
            playerController.getGun().setStat(stat, playerController.getGun().getStat(stat) + increase);
            Debug.Log($"{stat} stat increased by {increase}");
            Destroy(this.gameObject);
        }
    }
}
