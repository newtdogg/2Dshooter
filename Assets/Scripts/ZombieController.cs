using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : CharacterController
{
    // Start is called before the first frame update

    public Rigidbody2D rbody;
    private GameObject player;
    private float damage;
    private string status;
    private float detectionTimer;
    private Transform intents;
    private GameController gameController;
    private PlayerController playerController;
    private Vector3 playersLastKnownPosition;

    void Start()
    {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        maxHealth = 50;
        damage = 15;
        health = maxHealth;
        healthBar = gameObject.transform.GetChild(0).gameObject;
        status = "idle";
        detectionTimer = -1f;
        intents = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update() {

        switch (status) {
            case "attacking":
                moveToObject(player);
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
        // rounds the positon floats 
        var oX = Mathf.Round(obj.transform.position.x * 10f) / 10f;
        var oY = Mathf.Round(obj.transform.position.y * 10f) / 10f;
        var nX = Mathf.Round(transform.position.x * 10f) / 10f;
        var nY = Mathf.Round(transform.position.y * 10f) / 10f;
        
        var xDir = nX < oX ? 0.1f : -0.1f;
        var yDir = nY < oY ? 0.1f : -0.1f;

        var primaryDirection = (Mathf.Abs(oX) - Mathf.Abs(nX)) > (Mathf.Abs(oY) - Mathf.Abs(nY)) ? "x" : "y";
        var movementDirection = new Vector2(xDir, yDir);
        if(nX == oX) {
            movementDirection = new Vector2(0, yDir);
        } else if (nY == oY ) {
            movementDirection = new Vector2(xDir, 0);
        }
        // }
        if(gameObject.name != "Zombie1")
        rbody.MovePosition(rbody.position + movementDirection * Time.deltaTime * 40);
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
        status = "attacking";
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
