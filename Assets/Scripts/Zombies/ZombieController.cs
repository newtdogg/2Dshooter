using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;

public class ZombieController : AIController
{
    // Start is called before the first frame update
    
    public float damage;
    public bool hookAttached;
    public Transform intents;
    public bool clone;
    public bool inContactWithPlayer;
    public GameObject zombieObj;
    public GameObject scrapObject;
    public GameObject recipeObject;
    public Tilemap tilemap;
    public int scrap;
    public ContactController contactController;
    public float xpValue;

    public void defaultZombieAwake(string zombieTitle) {
        var jsonString = File.ReadAllText("./Assets/Scripts/Zombies.json"); 
        var zombieList = JsonUtility.FromJson<Zombies>(jsonString);
        var zombieJsonObject = zombieList.GetType().GetProperty(zombieTitle).GetValue(zombieList, null) as ZombieStats;
        speed = zombieJsonObject.speed;
        damage = zombieJsonObject.damage;
        maxHealth = zombieJsonObject.maxHealth;
        xpValue = zombieJsonObject.xpValue;
        title = zombieTitle;
        status = "idle";
        detectionTimer = -1f;
        canMove = true;
        scrapObject = GameObject.Find("Scrap");
        recipeObject = GameObject.Find("RecipeObject");
        rbody = gameObject.GetComponent<Rigidbody2D>();
        if(gameObject.name == $"{title}(Clone)") {
            spawner = transform.parent.parent.gameObject.GetComponent<Spawner>();
            lootController = transform.parent.parent.gameObject.GetComponent<Spawner>().lootController;
        }
        player = GameObject.Find("Player");
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        contactController = playerController.transform.GetChild(3).GetChild(2).gameObject.GetComponent<ContactController>();
        health = maxHealth;
        // remove when not needed for debugging
        // tilemap = GameObject.Find("MapGridObject").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        // healthBar = gameObject.transform.GetChild(0).gameObject;
        intents = transform.GetChild(1);
    }



    public void idleBehaviour() {
        if(distance < playerController.getSneakStat("attackDistance")) {
            setAttacking();
        }
        if(distance < playerController.getSneakStat("detectionDistance") && detectionTimer < 0) {
            playersLastKnownPosition = playerController.transform.position;
            setAlert();
        }
        if(detectionTimer > 0) {
            detectionTimer -= Time.deltaTime;
            // if(detectionTimer < playerController.getSneakStat("timeUntilDetection")/2f) {
            if(detectionTimer < (playerController.getSneakStat("timeUntilDetection") - 1)) {
                transform.position = Vector3.MoveTowards(transform.position, playersLastKnownPosition, Time.deltaTime * 2f);
            }
            if(detectionTimer <= 0) {
                if(distance < playerController.getSneakStat("detectionDistance")) {
                    setAttacking();
                // } else if (distance > 10) {
                //     setIdle();
                }
            }
        }
        if(detectionTimer <= 0 && status == "alert") {
            Debug.Log("alert here");
            setIdle();
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            inContactWithPlayer = true;
            hookAttached = false;
            playerController.canMove = false;
            contactController.updateBrawlStatus(gameObject);
            StopCoroutine("FollowPath");
        }
    }

    public void onDeath() {
        Destroy(gameObject);
        lootController.dropZombieLoot(transform.position, title);
        playerController.updateXP(xpValue);
        // playerController.gameController.globalSpeedSlow();
    }

}
