﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : CharacterController
{
    // Start is called before the first frame update

    public Rigidbody2D rbody;
    public GameObject player;
    public float damage;
    public string status;
    public float detectionTimer;
    public Transform intents;
    public GameController gameController;
    public bool clone;
    public PlayerController playerController;
    private Vector3 playersLastKnownPosition;
    public Transform target;
	public float speed;
	private List<Vector2> path;
	private int targetIndex;

    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update() {
        switch (status) {
            case "attackNow":
                moveToObject(player);
                status = "attacking";
                break;
            case "idle":
                idleBehaviour();
                break;
        }
        if(health <= 0) {
            Destroy(gameObject);
            gameController.checkWaveComplete();
        }
    }

    public void moveToObject(GameObject obj) {
        PathRequestManager.RequestPath(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(obj.transform.position.x, obj.transform.position.y),
            OnPathFound
        );
    }

    public void OnPathFound(List<Vector2> newPath, bool pathSuccessful) {
		if (pathSuccessful) {
            Debug.Log("OnPathFound");
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

    public IEnumerator FollowPath() {
		Vector2 currentWaypoint = path[0];
		while (true) {
            var zomPos = new Vector2(transform.position.x, transform.position.y);
            var target = path[path.Count - 1];
            if ((Mathf.Floor(zomPos.x) == Mathf.Floor(currentWaypoint.x)) &&
                (Mathf.Floor(zomPos.y) == Mathf.Floor(currentWaypoint.y))) {
				targetIndex ++;
                if ((Mathf.Floor(player.transform.position.x) != Mathf.Floor(currentWaypoint.x)) &&
                    (Mathf.Floor(player.transform.position.y) != Mathf.Floor(currentWaypoint.y))) {
                    path.Add(new Vector2(player.transform.position.x, player.transform.position.y));
                }
				if (targetIndex >= path.Count) {
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWaypoint.x, currentWaypoint.y, 0), Time.deltaTime * speed/5);
			yield return null;
		}
	}

    private void idleBehaviour() {
        float dist = Vector3.Distance(playerController.transform.position, transform.position);
        if(dist < playerController.getSneakStat("attackDistance")) {
            setAttacking();
        }
        if(dist < playerController.getSneakStat("detectionDistance") && detectionTimer < 0) {
            playersLastKnownPosition = playerController.transform.position;
            setAlert();
        }
        if(detectionTimer > 0) {
            detectionTimer -= Time.deltaTime;
            if(detectionTimer < playerController.getSneakStat("timeUntilDetection")/2f) {
                Debug.Log("moving to location");
                transform.position = Vector3.MoveTowards(transform.position, playersLastKnownPosition, Time.deltaTime * 2f);
            }
            if(detectionTimer <= 0) {
                if( dist < playerController.getSneakStat("detectionDistance")) {
                    setAttacking();
                } else if (dist > 10) {
                    setIdle();
                }
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Bullet(Clone)") {
            Destroy(col.gameObject);
            updateHealth(-playerController.getGun().getStat("damage"));
        }
        if(col.gameObject.name == "Player") {
            // var pC = col.gameObject.GetComponent<PlayerController>();
            playerController.updateHealth(damage);
        }
    }

    private void setAttacking() {
        intents.GetChild(0).gameObject.SetActive(false);
        intents.GetChild(1).gameObject.SetActive(true);
        status = "attackNow";
    }
    private void setAlert() {
        Debug.Log("?????");
        intents.GetChild(0).gameObject.SetActive(true);
        detectionTimer = playerController.getSneakStat("timeUntilDetection");
    }

    private void setIdle() {
        intents.GetChild(0).gameObject.SetActive(false);
        intents.GetChild(1).gameObject.SetActive(false);
        status = "idle";
        detectionTimer = -1f;
    }

}