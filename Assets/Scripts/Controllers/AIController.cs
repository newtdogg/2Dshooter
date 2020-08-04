using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIController : MonoBehaviour
{
    public float health;
    public string type;
    public float detectionTimer;
    public string status;   
    public float maxHealth;
    public float attackDelay;
    public string title;
    public LootController lootController;
	public float speed;
    public Vector3 playersLastKnownPosition;
    public Transform target;
    private Vector2[] path;
    public Vector2 currentWaypoint;
	private int targetIndex;
    public float distance;
    public Spawner spawner;
    public GameObject player;
    public PlayerController playerController;
    public Rigidbody2D rbody;
    private float minPathUpdateTime = 0.15f;
	private float pathUpdateMoveThreshold = 1.1f;
    public bool canMove;
    public List<Action> attacks;

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful) {
		if (pathSuccessful && newPath.Length > 1) {
			path = newPath;
            currentWaypoint = path[0];
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

    public void setAttacking() {
        Debug.Log("attacking");
        // intents.GetChild(0).gameObject.SetActive(false);
        // intents.GetChild(1).gameObject.SetActive(true);
        if(!spawner.battleStarted && spawner.walls.Count > 0) {
            spawner.startBattle();
        }
        // startArenaFightCheck();
        status = "attackNow";
    }
    public void setAlert() {
        Debug.Log("alert");
        // intents.GetChild(0).gameObject.SetActive(true);
        detectionTimer = playerController.getSneakStat("timeUntilDetection");
    }

    public void setIdle() {
        Debug.Log("idle");
        // intents.GetChild(0).gameObject.SetActive(false);
        // intents.GetChild(1).gameObject.SetActive(false);
        status = "idle";
        detectionTimer = -1f;
    }

    public IEnumerator FollowPath() {
		currentWaypoint = path[0];
		while (true && canMove) {
            var zomPos = new Vector2(transform.position.x, transform.position.y);
            var target = path[path.Length - 1];
            var currentWaypointInt = new Vector3Int((int)Mathf.Floor(currentWaypoint.x), (int)Mathf.Floor(currentWaypoint.y), 0);
            var zombiePosInt = new Vector3Int((int)Mathf.Floor(zomPos.x), (int)Mathf.Floor(zomPos.y), 0);
            // Debug.Log(currentWaypointInt);
            // Debug.Log(zombiePosInt);
            if(distance > playerController.getSneakStat("detectionDistance") * 2f && type != "boss") {
                setIdle();
                yield break;
            } else if (zombiePosInt.x == currentWaypointInt.x && zombiePosInt.y == currentWaypointInt.y) {
				targetIndex ++;
                if (targetIndex >= path.Length) {
                    currentWaypoint = player.transform.position;
                } else {
                    var waypointDirection = (transform.position - new Vector3(path[targetIndex].x, path[targetIndex].y)).normalized;
                    var playerDirection = (transform.position - player.transform.position).normalized;
                    if(waypointDirection != playerDirection) {
                        currentWaypoint = player.transform.position;
                    } else {
                        currentWaypoint = path[targetIndex];
                    }
                }
            }
            // tilemap.SetTileFlags(currentWaypointInt, TileFlags.None);
            // tilemap.SetColor(currentWaypointInt, Color.black);
            // Debug.Log($"{transform}{gameObject.name}");
            // rbody.AddForce(new Vector3(currentWaypoint.x * -1, currentWaypoint.y * -1, 0).normalized * 2);
            transform.position = Vector3.MoveTowards(transform.position, (Vector3)currentWaypoint, Time.deltaTime * speed/5);
			yield return null;
		}
	}

    public IEnumerator UpdatePath() {

		if (Time.timeSinceLevelLoad < 0.3f) {
			yield return new WaitForSeconds (0.3f);
		}

		PathRequestController.RequestPath((Vector2)transform.position, (Vector2)player.transform.position, OnPathFound);

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = player.transform.position;

		while (true) {
			yield return new WaitForSeconds (minPathUpdateTime);
			if ((player.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestController.RequestPath((Vector2)transform.position, (Vector2)player.transform.position, OnPathFound);
				targetPosOld = player.transform.position;
			}
            if(distance < 5) {
                // Debug.Log("chasing");
                currentWaypoint = player.transform.position;
            }
		}
	}
    public IEnumerator CycleRandomAttacks() {
        while(true) {
            var rand = new System.Random((int)System.DateTime.Now.Ticks);
            var randAttack = attacks[rand.Next(0, attacks.Count)];
            randAttack();
            yield return new WaitForSeconds (attackDelay);

        }
    }

}