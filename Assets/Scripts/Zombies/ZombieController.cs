using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZombieController : CharacterController
{
    // Start is called before the first frame update

    public Rigidbody2D rbody;
    public GameObject player;
    public float damage;
    public string status;
    public float detectionTimer;
    public Transform intents;
    public bool clone;
    public PlayerController playerController;
    private Vector3 playersLastKnownPosition;
    public Transform target;
	public float speed;
    public Vector2 currentWaypoint;
    public GameObject zombieObj;
    public GameObject scrapObject;
    public GameObject recipeObject;
	private Vector2[] path;
	private int targetIndex;
    public float distance;
    public Tilemap tilemap;
    public int scrap;
    private float minPathUpdateTime = 0.15f;
	private float pathUpdateMoveThreshold = 1.1f;

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful) {
		if (pathSuccessful && newPath.Length > 1) {
			path = newPath;
            currentWaypoint = path[0];
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

    public IEnumerator FollowPath() {
		currentWaypoint = path[0];
		while (true) {
            var zomPos = new Vector2(transform.position.x, transform.position.y);
            var target = path[path.Length - 1];
            var currentWaypointInt = new Vector3Int((int)Mathf.Floor(currentWaypoint.x), (int)Mathf.Floor(currentWaypoint.y), 0);
            var zombiePosInt = new Vector3Int((int)Mathf.Floor(zomPos.x), (int)Mathf.Floor(zomPos.y), 0);
            // Debug.Log(currentWaypointInt);
            // Debug.Log(zombiePosInt);
            if(distance > playerController.getSneakStat("detectionDistance") * 2f) {
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
            transform.position = Vector3.MoveTowards(transform.position, (Vector3)currentWaypoint, Time.deltaTime * speed/5);
			yield return null;
		}
	}

    public IEnumerator UpdatePath() {

		if (Time.timeSinceLevelLoad < 0.3f) {
			yield return new WaitForSeconds (0.3f);
		}

		PathRequestManager.RequestPath((Vector2)transform.position, (Vector2)player.transform.position, OnPathFound);

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = player.transform.position;

		while (true) {
			yield return new WaitForSeconds (minPathUpdateTime);
			if ((player.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath((Vector2)transform.position, (Vector2)player.transform.position, OnPathFound);
				targetPosOld = player.transform.position;
			}
            if(distance < 5) {
                Debug.Log("chasing");
                currentWaypoint = player.transform.position;
            }
		}
	}
    public void dropScrap() {
        var newScrapObj = Instantiate(scrapObject, transform.position, Quaternion.identity) as GameObject;
        newScrapObj.GetComponent<Scrap>().value = scrap;
    }

    public void dropRecipe() {
        var newRecipeObj = Instantiate(recipeObject, transform.position, Quaternion.identity) as GameObject;
        // newRecipeObj.GetComponent<Re>().value = scrap;
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
        if(col.gameObject.name == "Bullet(Clone)") {
           health -= playerController.getGun().currentStats.damage;
        }
        if(col.gameObject.name == "Player") {
            // playerController.updateHealth(damage);
            Debug.Log("kill");
            playerController.canMove = false;
            StopCoroutine("FollowPath");
        }
    }
    public void setAttacking() {
        Debug.Log("attacking");
        intents.GetChild(0).gameObject.SetActive(false);
        intents.GetChild(1).gameObject.SetActive(true);
        var spawner = transform.parent.parent.gameObject.GetComponent<Spawner>();
        if(!spawner.battleStarted && spawner.walls.Count > 0) {
            spawner.startBattle();
        }
        // startArenaFightCheck();
        status = "attackNow";
    }
    public void setAlert() {
        Debug.Log("alert");
        intents.GetChild(0).gameObject.SetActive(true);
        detectionTimer = playerController.getSneakStat("timeUntilDetection");
    }

    public void setIdle() {
        Debug.Log("idle");
        intents.GetChild(0).gameObject.SetActive(false);
        intents.GetChild(1).gameObject.SetActive(false);
        status = "idle";
        detectionTimer = -1f;
    }

}
