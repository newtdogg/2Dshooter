using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

public class AIController : MonoBehaviour
{
    public Tilemap tilemap;

    public float health;
    public string type;
    public float detectionTimer;
    public string status;   
    public float maxHealth;
    public float attackDelay;
    public float damageIndicatorTimer;
    public string title;
    public LootController lootController;
	public float speed;
    public string behaviourState;
    public float defaultSpeed;
    public Vector3 facingDirection;
    public Vector2Int compassFacingDirection;
    public Vector3 startingPosition;
    public Text damageIndicator;
    public float damageIndicatorInt;
    public Transform damageParent;
    public Vector3 playersLastKnownPosition;
    public Transform target;
    private Vector2[] path;
    public Vector2 currentWaypoint;
	private int targetIndex;
    public float distance;
    public Spawner spawner;
    public GameObject bullet;
    public bool inIdleMovement;
    public GameObject player;
    public PlayerController playerController;
    public GameController gameController;
    public Rigidbody2D rbody;
    private float minPathUpdateTime = 0.2f;
	private float pathUpdateMoveThreshold = 0.6f;
    public bool canMove;
    public List<Action> attacks;
    public List<Action> idleBehaviours;
    public Vector3 spawnPosition;

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful) {
		if (pathSuccessful && newPath.Length > 1) {
			path = newPath;
            currentWaypoint = path[0];
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
    }

    public IEnumerator handleIdleBehaviours() {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var randIdleMovement = idleBehaviours[rand.Next(0, idleBehaviours.Count)];
        randIdleMovement();
        yield return null;
    }

    public void setDormant() {
        status = "dormant";
    }

    public IEnumerator UpdatePath() {

		if (Time.timeSinceLevelLoad < 0.3f) {
            Debug.Log("Waiting for level load");
			yield return new WaitForSeconds (0.3f);
		}

		PathRequestController.RequestPath((Vector2)transform.position, (Vector2)player.transform.position, OnPathFound);

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;

		while (true) {
		    Vector3 targetPosOld = player.transform.position;
			yield return new WaitForSeconds (minPathUpdateTime);
			// if ((player.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestController.RequestPath((Vector2)transform.position, (Vector2)player.transform.position, OnPathFound);
				targetPosOld = player.transform.position;
            // }
            if(distance < 2) {
                // Debug.Log("chasing");
                currentWaypoint = player.transform.position;
            }
		}
	}

    public IEnumerator FollowPath() {
		currentWaypoint = path[0];
        var lastPosition = Vector3.zero;
		while (true && canMove) {
            var zomPos = new Vector2(transform.position.x, transform.position.y);
            var target = path[path.Length - 1];
            var currentWaypointInt = new Vector3Int((int)Mathf.Floor(currentWaypoint.x), (int)Mathf.Floor(currentWaypoint.y), 0);
            var lastWaypointInt = new Vector3Int((int)Mathf.Floor(path[path.Length - 1].x), (int)Mathf.Floor(path[path.Length - 1].y), 0);
            var mobPosInt = new Vector3Int((int)Mathf.Floor(zomPos.x), (int)Mathf.Floor(zomPos.y), 0);
            if(distance > playerController.getSneakStat("detectionDistance") * 2f && type != "boss") {
                Debug.Log("out of range :(");
                StopCoroutine("UpdatePath");
                yield break;
            } else if (mobPosInt.x == currentWaypointInt.x && mobPosInt.y == currentWaypointInt.y) {
				targetIndex ++;
                if(targetIndex < path.Length) {
                }
                // if (targetIndex >= path.Length) {
                if (targetIndex >= path.Length || (mobPosInt.x == lastWaypointInt.x && mobPosInt.y == lastWaypointInt.y)) {
                // if (targetIndex >= path.Length || lastPosition == transform.position) {
                    currentWaypoint = player.transform.position;
                } else {
                    // var waypointDirection = (transform.position - new Vector3(path[targetIndex].x, path[targetIndex].y)).normalized;
                    // var playerDirection = (transform.position - player.transform.position).normalized;
                    // if(waypointDirection != playerDirection) {
                        // Debug.Log("target player");
                    //     currentWaypoint = player.transform.position;
                    // } else {
                    //     Debug.Log("target path");
                    currentWaypoint = path[targetIndex];
                    // }
                }
            }
            // tilemap.SetTileFlags(currentWaypointInt, TileFlags.None);
            // tilemap.SetColor(currentWaypointInt, Color.black);
            // rbody.AddForce(new Vector3(currentWaypoint.x * -1, currentWaypoint.y * -1, 0).normalized * 2);
            lastPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, (Vector3)currentWaypoint, Time.deltaTime * (speed/5f) * (playerController.gameController.globalSpeed));
            facingDirection = ((Vector3)currentWaypoint - transform.position).normalized;
            calculateCompassFacingDirection(facingDirection);
            // Debug.Log(90 - (Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg));
			yield return null;
		}
	}

    public IEnumerator PathToLocation(Vector2 location) {
        if (Time.timeSinceLevelLoad < 0.3f) {
            Debug.Log("Waiting for level load");
			yield return new WaitForSeconds (0.3f);
		}

        PathRequestController.RequestPath((Vector2)transform.position, location, OnPathFound);
    }

    public IEnumerator CycleRandomAttacks() {
        while(true) {
            if(attacks.Count > 0) {
                var rand = new System.Random((int)System.DateTime.Now.Ticks);
                var randAttack = attacks[rand.Next(0, attacks.Count)];
                randAttack();
                yield return new WaitForSeconds (attackDelay);
            } else {
                yield return null;
            }
        }
    }

    public void updateDamage(float bulletDamage, string damageModifier = "") {
        var calibratedDamage = bulletDamage;
        var modifierText = damageModifier;
        if(health == maxHealth && behaviourState == "idle" && playerController.sneaking) {
            Debug.Log("sneaky");
            calibratedDamage = bulletDamage * 2f;
            modifierText = "critical";
        }
        health -= calibratedDamage;
        updateDamageUI(calibratedDamage, modifierText);
    }

    private void updateDamageUI(float damage, string modifierText) {
        switch (modifierText) {
            case "critical":
                damageIndicator.color = new Color(0.5f, 0.1f, 1f);
                break;
            case "shielded":
                damageIndicator.color = new Color(0.2f, 0.5f, 0.9f);
                break;
            default: 
                damageIndicator.color = new Color(0.6f, 0f, 0.1f);
                break;
        }
        if(damageIndicatorTimer > 0) {
            damageIndicatorTimer = 0;
            damageIndicatorInt += damage;
            // Debug.Log(damageIndicatorInt);
            damageIndicator.text = $"{Mathf.Round(damageIndicatorInt).ToString()}";
        } else {
            damageParent.GetChild(0).gameObject.SetActive(true);
            damageIndicatorInt = damage;
            damageIndicator.text = $"{Mathf.Round(damageIndicatorInt).ToString()}";
        }
    }

    public void calculateCompassFacingDirection(Vector3 facingDirection) {
        var absX = Mathf.Abs(facingDirection.x);
        var absY = Mathf.Abs(facingDirection.y);
        if(absX > absY) {
            compassFacingDirection = new Vector2Int((int)Mathf.Round(facingDirection.x), 0);
        } else {
            compassFacingDirection = new Vector2Int(0, (int)Mathf.Round(facingDirection.y));
        }
        // Debug.Log(compassFacingDirection);
    }

}