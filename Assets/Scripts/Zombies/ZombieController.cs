using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    public int scrap;
    public ContactController contactController;
    public float xpValue;

    public void defaultZombieAwake(string zombieTitle) {
        var jsonString = File.ReadAllText("./Assets/Scripts/Zombies.json"); 
        var zombieList = JsonUtility.FromJson<Zombies>(jsonString);
        var zombieJsonObject = zombieList.GetType().GetProperty(zombieTitle).GetValue(zombieList, null) as ZombieStats;
        startingPosition = transform.position;
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
        idleBehaviours = new List<Action> {
            shortStationaryWaiting,
            mediumStationaryWaiting,
            rightPerimeterWalkWithPauses,
            leftPerimeterWalkWithPauses,
            moveToCenterThenLeftWithPauses
        };
        // remove when not needed for debugging
        // tilemap = GameObject.Find("MapGridObject").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        // healthBar = gameObject.transform.GetChild(0).gameObject;
        intents = transform.GetChild(1);
    }

    public void manageBehaviourState() {
        // attacks player if in range
        if(distance < playerController.getSneakStat("attackDistance")) {
            setAttacking();
            stopAllIdleBehaviours();
        }
        if(distance < playerController.getSneakStat("detectionDistance") && detectionTimer < 0) {
            playersLastKnownPosition = playerController.transform.position;
            setAlert();
            stopAllIdleBehaviours();
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
                    stopAllIdleBehaviours();
                // } else if (distance > 10) {
                //     setIdle();
                }
            }
        }
        if(detectionTimer <= 0 && status == "alert") {
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
        if (col.gameObject.name.Contains("Mob")) {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }

    public void onDeath() {
        Destroy(gameObject);
        lootController.dropZombieLoot(transform.position, title);
        playerController.updateXP(xpValue);
        // playerController.gameController.globalSpeedSlow();
    }




    ////////////////////////////////////////////
    ///////////// IDLE FUNCTIONS ///////////////
    ////////////////////////////////////////////

     public IEnumerator shortStationaryWaitingCoroutine() {
        yield return new WaitForSeconds (3f);
    }

    public IEnumerator mediumStationaryWaitingCoroutine() {
        yield return new WaitForSeconds (5f);
    }

    public IEnumerator longStationaryWaitingCoroutine() {
        yield return new WaitForSeconds (7f);
    }

    public IEnumerator moveAroundLocationsCoroutine(List<Vector3> locations) {
        var waypointIndex = 0;
        while(true) {
            if(waypointIndex == locations.Count) {
                yield break;
            }
            currentWaypoint = locations[waypointIndex];
            if ((Mathf.Floor(transform.position.x) == Mathf.Floor(currentWaypoint.x)) && (Mathf.Floor(transform.position.y) == Mathf.Floor(currentWaypoint.y))) {
                waypointIndex++;
                Debug.Log("updated waypoint");
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Time.deltaTime * 3);
            yield return null;
        }
    }

    public IEnumerator moveAroundLocationsWithPausesCoroutine(List<Vector3> locations, int waitTime) {
        var waypointIndex = 0;
        while(true) {
            if(waypointIndex == locations.Count) {
                yield break;
            }
            currentWaypoint = locations[waypointIndex];
            if ((Mathf.Floor(transform.position.x) == Mathf.Floor(currentWaypoint.x)) && (Mathf.Floor(transform.position.y) == Mathf.Floor(currentWaypoint.y))) {
                waypointIndex++;
                yield return new WaitForSeconds(waitTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Time.deltaTime * 3);
            yield return null;
        }
    }

    public void shortStationaryWaiting() {
        StartCoroutine(shortStationaryWaitingCoroutine());
    }

    public void mediumStationaryWaiting() {
        StartCoroutine(mediumStationaryWaitingCoroutine());
    }

    public void longStationaryWaiting() {
        StartCoroutine(longStationaryWaitingCoroutine());
    }

    public void leftPerimeterWalkWithPauses() {
        var pointList = new List<Vector3>() { 
            spawner.getBottomLeftCorner(),
            spawner.getTopLeftCorner(),
            spawner.getBottomLeftCorner(),
            startingPosition
        };
        StartCoroutine(moveAroundLocationsWithPausesCoroutine(pointList, 2));
    }

    public void rightPerimeterWalkWithPauses() {
        var pointList = new List<Vector3>() { 
            spawner.getBottomRightCorner(),
            spawner.getTopRightCorner(),
            spawner.getBottomRightCorner(),
            startingPosition
        };
        StartCoroutine(moveAroundLocationsWithPausesCoroutine(pointList, 2));
    }

    public void moveToCenterThenLeftWithPauses() {
        var pointList = new List<Vector3>() { 
            spawner.centerOfObject,
            spawner.getBottomLeftCorner()
        };
        StartCoroutine(moveAroundLocationsWithPausesCoroutine(pointList, 1));
    }

    public void stopAllIdleBehaviours() {
        Debug.Log("stopping all coroutines");
        StopCoroutine("moveAroundLocationsWithPausesCoroutine");
        StopCoroutine("moveAroundLocationsCoroutine");
        StopCoroutine("shortStationaryWaitingCoroutine");
        StopCoroutine("mediumStationaryWaitingCoroutine");
    }
}