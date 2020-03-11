using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : CharacterController
{
    // Start is called before the first frame update

    public Rigidbody2D rbody;
    private GameObject player;
    private float damage;
    private GameController gameController;
    private PlayerController playerController;

    void Start()
    {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        maxHealth = 50;
        damage = 5;
        health = maxHealth;
        healthBar = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update() {
        moveToObject(player);
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
        
        // if(inContact) {
        //     if (collisionSide == "x") {
        //         movementDirection = new Vector2(0, yDir);
        //         wasInContact = true;);
        //     } else if (collisionSide == "y") {
        //         movementDirection = new Vector2(xDir, 0);
        //         wasInContact = true;
        //     }
        // } else {
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

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Bullet(Clone)") {
            Destroy(col.gameObject);
            updateHealth(-playerController.getGun().getStat("damage"));
            rbody.AddForce(col.GetContact(0).normal * 40);
        }
        if(col.gameObject.name == "Player") {
            // var pC = col.gameObject.GetComponent<PlayerController>();
            playerController.updateHealth(-10);
        }
    }
}
