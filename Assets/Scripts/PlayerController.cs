using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController {
    // Start is called before the first frame update
    private Rigidbody2D rbody;
    private Dictionary<string,float> sneak;
    private Gun gun;
    void Start() {		
        rbody = GetComponent<Rigidbody2D>();
        gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        gun.reloadMagazine();
        gun.setPlayerController(this);
        healthBar = gameObject.transform.GetChild(1).gameObject;
        maxHealth = 100;
        health = maxHealth;
        sneak = new Dictionary<string, float>() {
            { "timeUntilDetection", 2f },
            { "detectionDistance", 60f },
            { "attackDistance", 60f }
        };
    }

    // Update is called once per frame
    void Update() {
        // Debug.Log(sneak["dsetectionDistance"]);
        var speed = 6;
        movementControls(speed);
        gun.directionallyShootGun();
    }

    private void movementControls(int speed) {
        var movementVector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.A)) {
            movementVector += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            movementVector += Vector3.right;
        }
        if (Input.GetKey(KeyCode.W)) {
            movementVector += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S)) {
            movementVector += Vector3.down;
        }
        transform.position = Vector2.MoveTowards(transform.position, transform.position + movementVector, Time.deltaTime * speed);
    }

    public Gun getGun() {
        return this.gun;
    }

    public float getSneakStat(string key) {
        return sneak[key];
    }

     public void setSneakStat(string key, float value) {
        sneak[key] = value;
    }

    public void resetSneakStats() {
        sneak = new Dictionary<string, float>() {
            { "timeUntilDetection", 4f },
            { "detectionDistance", 12f },
            { "attackDistance", 8f }
        };
    }
}
