using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : CharacterController {
    // Start is called before the first frame update
    private Rigidbody2D rbody;
    private Dictionary<string,float> sneak;
    private Dictionary<string,float> sneakDefault;

    public Transform armorParent;
    // public GameObject armorHead;
    // public GameObject armorBody;
    // public GameObject armorBoots;

    private Gun gun;
    public int scrap;
    private Text scrapText;
    public float speed;
    private GameObject detection;
    public bool canMove;
    public float experienceSpendable;
    public float experience;
    public float experienceForNextLevel;
    public int experienceLevel;
    public float[] experienceLevelUpRequirement;

    void Start() {		
        rbody = GetComponent<Rigidbody2D>();
        gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        detection = transform.GetChild(2).gameObject;
        gun.reloadMagazine();
        gun.setPlayerController(this);
        scrapText = transform.GetChild(3).GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        healthBar = transform.GetChild(3).GetChild(1).GetChild(1).gameObject;
        maxHealth = 100;
        experienceLevelUpRequirement = new float[] { 0f, 1000f, 2000f, 5000f, 10000f };
        experienceSpendable = 200f;
        experience = 0f;
        experienceLevel = 0;
        experienceForNextLevel = experienceLevelUpRequirement[experienceLevel + 1];
        updateXP(0f);
        canMove = true;
        speed = 22f;
        scrap = 80;
        health = maxHealth;
        sneak = new Dictionary<string, float>() {
            { "timeUntilDetection", 9f },
            { "detectionDistance", 18f },
            { "attackDistance", 12f }
        };
        sneakDefault = new Dictionary<string, float>() {
            { "timeUntilDetection", 9f },
            { "detectionDistance", 18f },
            { "attackDistance", 12f }
        };

        // armorParent = transform.GetChild(6);
        // armorHead = armorParent.GetChild(0).gameObject;
        // armorBody = armorParent.GetChild(1).gameObject;
        // armorBoots = armorParent.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update() {
        if(gun == null) {
            gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        }
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
        gun.shootingGunCheck();
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
            rbody.AddForce(movementVector.normalized * speed);
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
            { "timeUntilDetection", 9f },
            { "detectionDistance", 18f },
            { "attackDistance", 12f }
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
        var armorRating = getArmorRating();
        var correctedDamage = damage + (armorRating/2);
        return correctedDamage = correctedDamage > 0 ? 0.1f : correctedDamage;
    }

    public void updateXP(float xpValue) {
        experience += xpValue;
        experienceSpendable += xpValue;
        var experiencePercentage = (experience - experienceLevelUpRequirement[experienceLevel])/(experienceForNextLevel - experienceLevelUpRequirement[experienceLevel]);
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
        experienceLevel += 1;
        experienceForNextLevel = experienceLevelUpRequirement[experienceLevel + 1];
        transform.GetChild(3).GetChild(3).GetChild(3).gameObject.GetComponent<Text>().text = experienceLevel.ToString();
    }

    public void loadData(SaveData saveData) {
        experienceSpendable = saveData.experienceSpendable;
        experience = saveData.experience;
        experienceForNextLevel = saveData.experienceForNextLevel;
        experienceLevel = saveData.experienceLevel;
    }
}
