using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZombieController : AIController
{
    // Start is called before the first frame update
    
    public float damage;
    public int scrapDropMin;
    public int scrapDropMax;
    public Transform intents;
    public bool clone;
    public GameObject zombieObj;
    public GameObject scrapObject;
    public GameObject recipeObject;
    public Tilemap tilemap;
    public int scrap;
    public ContactController contactController;



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
            playerController.canMove = false;
            contactController.updateBrawlStatus(gameObject);
            StopCoroutine("FollowPath");
        }
    }

}
