using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        defaultSpeed = speed;
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

        damageIndicatorTimer = 0f;
        damageParent = transform.GetChild(0);
        damageIndicator = damageParent.GetChild(0).gameObject.GetComponent<Text>();
        damageIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -2.34f, 0);
        damageParent.GetChild(0).gameObject.SetActive(false);
        if(gameObject.name == $"{title}(Clone)") {
            behaviourState = "idle";
        }
        // remove when not needed for debugging
        // tilemap = GameObject.Find("MapGridObject").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        // healthBar = gameObject.transform.GetChild(0).gameObject;
        intents = transform.GetChild(1);
    }

    public void manageBehaviourState() {
        // attacks player if in range
        if(distance < playerController.getSneakStat("attackDistance") && behaviourState != "attacking") {
            // Debug.Log("attacking");
            speed = defaultSpeed;
            behaviourState = "attacking";
            StopCoroutine("UpdatePath");
            StopCoroutine("PathToLocation");
            StartCoroutine("UpdatePath");
            stopAllIdleBehaviours();
        }

        // alerts enemy to players presence
        if(distance < playerController.getSneakStat("detectionDistance") && detectionTimer < 0) {
            // Debug.Log("alert");
            behaviourState = "alert";
            speed = defaultSpeed/2;
            playersLastKnownPosition = playerController.transform.position;
            detectionTimer = playerController.getSneakStat("timeUntilDetection");
            StopCoroutine("PathToLocation");
            StartCoroutine(PathToLocation(playersLastKnownPosition));
            stopAllIdleBehaviours();
        }

        if(detectionTimer > 0 ) {
            detectionTimer -= Time.deltaTime;
        }

        if(detectionTimer <= 0 && behaviourState == "alert") {
            StopCoroutine("PathToLocation");
            detectionTimer = -1f;
            behaviourState = "idle";
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
        StopCoroutine("moveAroundLocationsWithPausesCoroutine");
        StopCoroutine("moveAroundLocationsCoroutine");
        StopCoroutine("shortStationaryWaitingCoroutine");
        StopCoroutine("mediumStationaryWaitingCoroutine");
    }
}