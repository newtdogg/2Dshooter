﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PlayerController : PlayableCharacterController {
    // Start is called before the first frame update
    private Rigidbody2D rbody;
    private Dictionary<string,float> sneak;
    private Dictionary<string,float> sneakDefault;

    public Transform armorParent;
    public GameObject armorHead;
    public GameObject armorBody;
    public GameObject armorBoots;

    public bool sneaking;
    private Gun gun;
    private Transform pickupUI;
    public Dictionary<string, int> scrap;
    // private Text scrapText;
    public float speed;
    public float defaultSpeed;
    public float slowTimer;
    private GameObject detection;
    public bool canMove;
    public GameController gameController;
    public float experienceSpendable;
    public float experience;
    public float experienceForNextLevel;
    public int experienceLevel;
    public float[] experienceLevelUpRequirement;
    public Dictionary<string, Weapon> unlockedWeapons;
    private Vector3 lastDirection;
    private GameObject torch;
    public bool invulnerable;
    public float invulnerableTimer;
    public Transform enemyIndicatorParent;
    private List<MobSpawner> activeSpawners;

    void Start() {		
        rbody = GetComponent<Rigidbody2D>();
        gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        detection = transform.GetChild(2).gameObject;
        gun.reloadMagazine();
        gun.setPlayerController(this);
        gun.type = "player";
        // scrapText = transform.GetChild(3).GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        healthBar = transform.GetChild(3).GetChild(1).GetChild(1).gameObject;
        pickupUI = transform.GetChild(8);
        torch = transform.GetChild(9).GetChild(0).gameObject;
        sneaking = false;
        // enemyIndicatorParent = transform.GetChild(3).GetChild(4);
        enemyIndicatorParent = transform.GetChild(10);
        maxHealth = 100;
        experienceLevelUpRequirement = new float[] { 0f, 1000f, 2000f, 5000f, 10000f };
        experienceSpendable = 200f;
        experience = 0f;
        experienceLevel = 0;
        experienceForNextLevel = experienceLevelUpRequirement[experienceLevel + 1];
        updateXP(0f);
        activeSpawners = new List<MobSpawner>();
        canMove = true;
        defaultSpeed = 50f;
        speed = 60f;
        scrap = new Dictionary<string, int>() {
            { "junkMetal", 20 },
            { "brokenGlass", 10 }
        };
        health = maxHealth;
        sneak = new Dictionary<string, float>() {
            { "timeUntilDetection", 6f },
            { "detectionDistance", 15f },
            { "attackDistance", 9f }
        };
        sneakDefault = new Dictionary<string, float>() {
            { "timeUntilDetection", 6f },
            { "detectionDistance", 15f },
            { "attackDistance", 9f }
        };

        armorParent = transform.GetChild(7);
        armorHead = armorParent.GetChild(0).gameObject;
        armorBody = armorParent.GetChild(1).gameObject;
        armorBoots = armorParent.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update() {
        if(gun == null) {
            gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        }
        // TOGGLE TORCH
        if (Input.GetKeyDown(KeyCode.T)) {
            transform.GetChild(9).gameObject.SetActive(!transform.GetChild(9).gameObject.activeSelf);
            // TODO:
            // Handle stealth change with torch toggle
        }
        
    }

    void FixedUpdate() {
        detection.transform.GetChild(0).localScale = new Vector3(sneak["detectionDistance"], sneak["detectionDistance"], 0) * 2;
        detection.transform.GetChild(1).localScale = new Vector3(sneak["attackDistance"], sneak["attackDistance"], 0) * 2;
        var movSpeed = 0f;
        if(slowTimer >= 0) {
            movSpeed = (speed/3) * gameController.globalSpeed;
            slowTimer -= Time.deltaTime;
        } else {
            movSpeed = speed * gameController.globalSpeed;
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            sneaking = true;
            movSpeed = speed/2;
            sneak["detectionDistance"] = sneakDefault["detectionDistance"]/2;
            sneak["attackDistance"] = sneakDefault["attackDistance"]/2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            sneaking = false;
            movSpeed = speed;
            sneak["detectionDistance"] = sneakDefault["detectionDistance"];
            sneak["attackDistance"] = sneakDefault["attackDistance"];
        }
        if(invulnerableTimer > 0 && invulnerable) {
            invulnerableTimer -= Time.deltaTime;
        }
        if(invulnerableTimer <= 0 && invulnerable) {
            invulnerableTimer = -1;
            invulnerable = false;
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        movementControls(movSpeed);
        updateEnemyIndicators();
        gun.shootingGunCheck();
    }

    private void movementControls(float speed) {
        var movementVector = new Vector3(0, 0, 0);
        if(canMove) {
            /*call handle direction only when input is called*/
            if (Input.GetKey(KeyCode.A)) {
                movementVector += Vector3.left;
                handleDirection(movementVector);
            }
            if (Input.GetKey(KeyCode.D)) {
                movementVector += Vector3.right;
                handleDirection(movementVector);
            }
            if (Input.GetKey(KeyCode.W)) {
                movementVector += Vector3.up;
                handleDirection(movementVector);
            }
            if (Input.GetKey(KeyCode.S)) {
                movementVector += Vector3.down;
                handleDirection(movementVector);
            }
            rbody.AddForce(movementVector.normalized * speed);
        }
    }

    private void handleDirection(Vector3 direction) {
        if(direction != lastDirection) {
            var diffAngle = Vector3.Angle(lastDirection, direction);
            var normalAngle = Vector3.Angle(new Vector3(0, 1, 0), direction);
            var targetDirection = direction.x > 0 ? Quaternion.Euler(0, 0, 360 - normalAngle) : Quaternion.Euler(0, 0, normalAngle);
            torch.transform.rotation = targetDirection;
        }
        lastDirection = direction;
    }

    public void setupEnemyIndicators(List<MobSpawner> spawners) {
        activeSpawners = spawners;
        foreach (Transform child in enemyIndicatorParent.GetChild(1)) {
            Destroy(child.gameObject);
        }
        foreach (var spawner in spawners) {
            var zomInd = Instantiate(enemyIndicatorParent.GetChild(0).gameObject, new Vector2(0, 0), Quaternion.identity);
            zomInd.transform.SetParent(enemyIndicatorParent.GetChild(1));
            zomInd.transform.localPosition = Vector3.zero;
            var normalizedDiff = (transform.position - spawner.centerOfObject).normalized;
            var degrees = Mathf.Atan2(normalizedDiff.y, normalizedDiff.x) * Mathf.Rad2Deg;
            var distance = (sneakDefault["attackDistance"] * 2) > 22 ? 22 : sneakDefault["attackDistance"] * 2;
            zomInd.transform.GetChild(0).localPosition = new Vector3(0, distance, 0);
            zomInd.transform.rotation = Quaternion.Euler(0, 0, degrees + 90f);
        }
    }

    public void updateEnemyIndicators() {
        var count = 0;
        foreach (var spawner in activeSpawners) {
            if(spawner.mobsList.childCount != 0) {     
                var normalizedDiff = (transform.position - spawner.centerOfObject).normalized;
                var degrees = Mathf.Atan2(normalizedDiff.y, normalizedDiff.x) * Mathf.Rad2Deg;
                var distance = (sneakDefault["attackDistance"] * 2) > 22 ? 22 : sneakDefault["attackDistance"] * 2;
                enemyIndicatorParent.GetChild(1).GetChild(count).GetChild(0).localPosition = new Vector3(0, distance, 0);
                enemyIndicatorParent.GetChild(1).GetChild(count).transform.rotation = Quaternion.Euler(0, 0, degrees + 90f);
            } else {
                enemyIndicatorParent.GetChild(1).GetChild(count).gameObject.SetActive(false);
            }
            count++;
        }
    }

    public void dealMobDamage(float damage) {
        if(!invulnerable) {
            updateHealth(damage);
            triggerInvulnerability();
        }
    }

    public void triggerInvulnerability() {
        invulnerable = true;
        invulnerableTimer = 1.2f;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.234959f, 0.9339623f, 0.8856441f);
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

    public void updateScrap(Dictionary<string, int> scrapDict) {
        foreach (var scrapObj in scrapDict) {
            if(scrap.ContainsKey(scrapObj.Key)) {
                scrap[scrapObj.Key] += scrapObj.Value;
            } else {
                scrap.Add(scrapObj.Key, scrapObj.Value);
            }
        }
    }

    public bool checkScapAmount(Dictionary<string, int> scrapValues) {
        var hasScrap = true;
        foreach (var scrapAmount in scrapValues) {
            // Debug.Log(scrapAmount.Key);
            if (scrap.ContainsKey(scrapAmount.Key)) {
                if(scrap[scrapAmount.Key] < scrapAmount.Value) {
                    hasScrap = false;
                }
            }
        }
        return hasScrap;
    }

    public void updateHealth(float amount) {
        var healthWithArmor = amount;
        if(amount < 0) {
            healthWithArmor = calculateDamageThroughArmor(amount);
        }
        health = health + amount > maxHealth ? maxHealth : health += healthWithArmor;
        var healthPercentage = health/maxHealth * 100f;
        var maxWidth = 260f;
        float percentageWidth = (maxWidth/100f) * healthPercentage;
        healthBar.GetComponent<RectTransform>().localPosition = new Vector3(-((maxWidth - percentageWidth)/ 2f), 0, 0);
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector3(percentageWidth, 42, 0);
    }

    public void resetSneakStats() {
        sneak = new Dictionary<string, float>() {
            { "timeUntilDetection", 6f },
            { "detectionDistance", 15f },
            { "attackDistance", 9f }
        };
    }

    private int getArmorRating() {
        var armorRating = 0;
        foreach(Transform armorTransform in armorParent) {
            armorRating += armorTransform.gameObject.GetComponent<Armor>().value;
        }
        return armorRating;
    }

    private float calculateDamageThroughArmor(float damage) {
        // var armorRating = getArmorRating();
        var armorRating = 0;
        var correctedDamage = damage + (armorRating/2);
        return correctedDamage = correctedDamage > 0 ? 0.1f : correctedDamage;
    }

    public void updateXP(float xpValue) {
        experience += xpValue;
        experienceSpendable += xpValue;
        var xpDiff = experienceLevelUpRequirement[experienceLevel];
        var experiencePercentage = (experience - xpDiff)/(experienceForNextLevel - xpDiff);
        var experienceBarWidth = experiencePercentage * 300;
        if(experiencePercentage >= 1) {
            experiencePercentage -= 1;
            levelUp();
        }
        var xp = transform.GetChild(3).GetChild(3).GetChild(1).gameObject.GetComponent<RectTransform>();
        xp.sizeDelta = new Vector3(experienceBarWidth, 6.66f, 0);
        xp.localPosition = new Vector3(-150f + ((experienceBarWidth)/2), xp.localPosition.y, 0);
    }

    public void levelUp() {
        experienceLevel++;
        experienceForNextLevel = experienceLevelUpRequirement[experienceLevel + 1];
        transform.GetChild(3).GetChild(3).GetChild(3).gameObject.GetComponent<Text>().text = experienceLevel.ToString();
    }

    public void slow(float time) {
        slowTimer = time;
    }

    public void loadData(SaveData saveData) {
        experienceSpendable = saveData.experienceSpendable;
        experience = saveData.experience;
        experienceForNextLevel = saveData.experienceForNextLevel;
        experienceLevel = saveData.experienceLevel;
    }

    public void pickupScrapUI(Dictionary<string, int> scrapDict) {
        foreach (var scrapObj in scrapDict) {
            if(!checkScrapPickupUI(scrapObj)) {
                var pickupTitleObject = Instantiate(pickupUI.GetChild(0).gameObject, new Vector2(0, 0), Quaternion.identity);
                pickupTitleObject.transform.SetParent(pickupUI);
                pickupTitleObject.name = scrapObj.Key;
                pickupTitleObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0.5f * pickupUI.childCount - 1, 0);
                var newTextScript = pickupTitleObject.GetComponent<PickupText>();
                newTextScript.value += scrapObj.Value;
                pickupTitleObject.GetComponent<Text>().text = $"{scrapObj.Key} x{scrapObj.Value}";
            }
        }
    }

    public bool checkScrapPickupUI(KeyValuePair<string, int> scrapObj) {
        foreach (Transform pickupText in pickupUI) {
            if(pickupText.gameObject.name == scrapObj.Key) {
                Debug.Log("here");
                var textScript = pickupText.gameObject.GetComponent<PickupText>();
                textScript.timer = 0;
                textScript.value += scrapObj.Value;
                pickupText.gameObject.GetComponent<Text>().text = $"{scrapObj.Key} x{textScript.value}";
                return true;
            }
        }
        return false;
    }
}
