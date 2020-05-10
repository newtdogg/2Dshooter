using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : CharacterController {
    // Start is called before the first frame update
    private Rigidbody2D rbody;
    private Dictionary<string,float> sneak;
    private Dictionary<string,float> sneakDefault;
    private Gun gun;
    public int scrap;
    private Text scrapText;
    private float speed;
    private GameObject detection;
    public bool canMove;
    void Start() {		
        rbody = GetComponent<Rigidbody2D>();
        gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        detection = transform.GetChild(3).gameObject;
        gun.reloadMagazine();
        gun.setPlayerController(this);
        scrapText = transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>();
        healthBar = gameObject.transform.GetChild(1).gameObject;
        maxHealth = 100;
        canMove = true;
        speed = 6;
        scrap = 20;
        health = maxHealth;
        sneak = new Dictionary<string, float>() {
            { "timeUntilDetection", 6f },
            { "detectionDistance", 12f },
            { "attackDistance", 9f }
        };
        sneakDefault = new Dictionary<string, float>() {
            { "timeUntilDetection", 6f },
            { "detectionDistance", 12f },
            { "attackDistance", 9f }
        };
    }

    // Update is called once per frame
    void Update() {
        detection.transform.GetChild(0).localScale = new Vector3(sneak["detectionDistance"], sneak["detectionDistance"], 0) * 2;
        detection.transform.GetChild(1).localScale = new Vector3(sneak["attackDistance"], sneak["attackDistance"], 0) * 2;
        var movSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            movSpeed = speed/2;
            sneak["detectionDistance"] = sneakDefault["detectionDistance"]/2;
            sneak["attackDistance"] = sneakDefault["attackDistance"]/2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            movSpeed = speed;
            sneak["detectionDistance"] = sneakDefault["detectionDistance"];
            sneak["attackDistance"] = sneakDefault["attackDistance"];
        }
        movementControls(movSpeed);
        gun.directionallyShootGun();
    }

    private void movementControls(float speed) {
        var movementVector = new Vector3(0, 0, 0);
        if(canMove) {
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

    public void updateScrap(int amount) {
        scrap += amount;
        scrapText.text = scrap.ToString();
    }
    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Scrap(Clone)") {
            Debug.Log(col.gameObject.GetComponent<Scrap>().value);
            updateScrap(col.gameObject.GetComponent<Scrap>().value);
            Destroy(col.gameObject);
        }
    }

    public void resetSneakStats() {
        sneak = new Dictionary<string, float>() {
            { "timeUntilDetection", 4f },
            { "detectionDistance", 12f },
            { "attackDistance", 8f }
        };
    }
}
