using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Tilemaps;

public class MobController : AIController
{
    // Start is called before the first frame update
    
    public float damage;
    public bool hookAttached;
    public Transform intents;
    public bool clone;
    public bool inContactWithPlayer;
    public GameObject mobObj;
    public GameObject scrapObject;
    public GameObject recipeObject;
    public int scrap;
    public ContactController contactController;
    public float xpValue;

    public void defaultMobAwake(string mobTitle) {
        var jsonString = File.ReadAllText("./Assets/Scripts/Mobs.json"); 
        var mobList = JsonUtility.FromJson<Mobs>(jsonString);
        var mobJsonObject = mobList.GetType().GetProperty(mobTitle).GetValue(mobList, null) as MobStats;
        startingPosition = transform.position;
        speed = mobJsonObject.speed;
        defaultSpeed = speed;
        damage = mobJsonObject.damage;
        maxHealth = mobJsonObject.maxHealth;
        xpValue = mobJsonObject.xpValue;
        title = mobTitle;
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
        // idleBehaviours = new List<Action> {
        //     shortStationaryWaiting,
        //     mediumStationaryWaiting,
        //     rightPerimeterWalkWithPauses,
        //     leftPerimeterWalkWithPauses,
        //     moveToCenterThenLeftWithPauses
        // };

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
        // if(distance < playerController.getSneakStat("attackDistance") && behaviourState == "attacking") {
        if(distance < playerController.getSneakStat("attackDistance") && behaviourState == "alert") {
            speed = defaultSpeed;
            behaviourState = "attacking";
            StopCoroutine("UpdatePath");
            StopCoroutine("PathToLocation");
            StartCoroutine("UpdatePath");
            StartCoroutine("CycleRandomAttacks");
            stopAllIdleBehaviours();
        }

        // alerts enemy to players presence
        if((distance < playerController.getSneakStat("detectionDistance") && detectionTimer < 0) || 
        // enemy deaggros if player is too far away
        (distance > playerController.getSneakStat("detectionDistance") && behaviourState == "attacking")) {
        // (distance > playerController.getSneakStat("detectionDistance") && (behaviourState == "alert" || behaviourState == "attacking"))) {
            stopAttackingBehaviours();
            triggerAlertState();
        }

        // enemy stops moving after being alerted
        if(detectionTimer <= 0 && behaviourState == "alert") {
            StopCoroutine("PathToLocation");
            detectionTimer = -1f;
            behaviourState = "idle";
        }

        if(detectionTimer > 0 ) {
            detectionTimer -= Time.deltaTime;
        }
    }

    public void stopAttackingBehaviours() {
        Debug.Log("stopAttackingBehaviours");
        StopCoroutine("FollowPath");
        StopCoroutine("UpdatePath");
        StopCoroutine("CycleRandomAttacks");
    }

    public void triggerAlertState() {
        behaviourState = "alert";
        speed = defaultSpeed/2;
        playersLastKnownPosition = playerController.transform.position;
        detectionTimer = playerController.getSneakStat("timeUntilDetection");
        StopCoroutine("PathToLocation");
        StartCoroutine(PathToLocation(playersLastKnownPosition));
        stopAllIdleBehaviours();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.name == "Player") {
            inContactWithPlayer = true;
            playerController.canMove = false;
            contactController.updateBrawlStatus(gameObject);
            StopCoroutine("FollowPath");
            StopCoroutine("CycleRandomAttacks");
        }
        if (col.gameObject.name.Contains("Mob")) {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
        if (col.gameObject.name.Contains("DoCPlayerBullet")) {
            var projectileScript = col.gameObject.GetComponent<Projectile>();
            updateDamage(projectileScript.damage);
        }
    }

    public void defaultUpdate() {
        if(health <= 0) {
            onDeath();
        }
        if(damageParent.GetChild(0).gameObject.activeSelf) {
            damageIndicatorTimer += Time.deltaTime;
        }
        if(damageIndicatorTimer > 2f) {
            damageParent.GetChild(0).gameObject.SetActive(false);
            damageIndicatorTimer = 0;
        }
    }

    public void onDeath() {
        Destroy(gameObject);
        lootController.dropMobLoot(transform.position, title);
        playerController.updateXP(xpValue);
        // playerController.gameController.globalSpeedSlow();
    }

    public IEnumerator lunge(float magnitude, int accuracy) {
        // canMove = false;
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var offset = rand.Next(0, 10 - accuracy)/50;
        var distanceVector = (player.transform.position - transform.position).normalized;
        var distanceVectorWithAccuracy = new Vector3(distanceVector.x + offset, distanceVector.y + offset, 0);
        facingDirection = distanceVector;
        calculateCompassFacingDirection(facingDirection);
        rbody.AddForce(distanceVectorWithAccuracy * 100 * rbody.mass * magnitude);
        yield return new WaitForSeconds (0.8f);
        rbody.velocity = Vector3.zero;
        rbody.angularVelocity = 0f; 
        // canMove = true;
        yield return null;
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

    // public IEnumerator moveAroundLocationsCoroutine(List<Vector3> locations) {
    //     var waypointIndex = 0;
    //     while(true) {
    //         if(waypointIndex == locations.Count) {
    //             yield break;
    //         }
    //         currentWaypoint = locations[waypointIndex];
    //         if ((Mathf.Floor(transform.position.x) == Mathf.Floor(currentWaypoint.x)) && (Mathf.Floor(transform.position.y) == Mathf.Floor(currentWaypoint.y))) {
    //             waypointIndex++;
    //             Debug.Log("updated waypoint");
    //         }
    //         transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Time.deltaTime * 3);
    //         yield return null;
    //     }
    // }

    // public IEnumerator moveAroundLocationsWithPausesCoroutine(List<Vector3> locations, int waitTime) {
    //     var waypointIndex = 0;
    //     while(true) {
    //         if(waypointIndex == locations.Count) {
    //             yield break;
    //         }
    //         currentWaypoint = locations[waypointIndex];
    //         if ((Mathf.Floor(transform.position.x) == Mathf.Floor(currentWaypoint.x)) && (Mathf.Floor(transform.position.y) == Mathf.Floor(currentWaypoint.y))) {
    //             waypointIndex++;
    //             yield return new WaitForSeconds(waitTime);
    //         }
    //         transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Time.deltaTime * 3);
    //         yield return null;
    //     }
    // }

    // public void shortStationaryWaiting() {
    //     StartCoroutine(shortStationaryWaitingCoroutine());
    // }

    // public void mediumStationaryWaiting() {
    //     StartCoroutine(mediumStationaryWaitingCoroutine());
    // }

    // public void longStationaryWaiting() {
    //     StartCoroutine(longStationaryWaitingCoroutine());
    // }

    // public void leftPerimeterWalkWithPauses() {
    //     var pointList = new List<Vector3>() { 
    //         spawner.getBottomLeftCorner(),
    //         spawner.getTopLeftCorner(),
    //         spawner.getBottomLeftCorner(),
    //         startingPosition
    //     };
    //     StartCoroutine(moveAroundLocationsWithPausesCoroutine(pointList, 2));
    // }

    // public void rightPerimeterWalkWithPauses() {
    //     var pointList = new List<Vector3>() { 
    //         spawner.getBottomRightCorner(),
    //         spawner.getTopRightCorner(),
    //         spawner.getBottomRightCorner(),
    //         startingPosition
    //     };
    //     StartCoroutine(moveAroundLocationsWithPausesCoroutine(pointList, 2));
    // }

    // public void moveToCenterThenLeftWithPauses() {
    //     var pointList = new List<Vector3>() { 
    //         spawner.centerOfObject,
    //         spawner.getBottomLeftCorner()
    //     };
    //     StartCoroutine(moveAroundLocationsWithPausesCoroutine(pointList, 1));
    // }

    public void stopAllIdleBehaviours() {
        StopCoroutine("moveAroundLocationsWithPausesCoroutine");
        StopCoroutine("moveAroundLocationsCoroutine");
        StopCoroutine("shortStationaryWaitingCoroutine");
        StopCoroutine("mediumStationaryWaitingCoroutine");
    }
}