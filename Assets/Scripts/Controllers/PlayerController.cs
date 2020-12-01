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
    public GameObject armorHead;
    public GameObject armorBody;
    public GameObject armorBoots;

    private Gun gun;
    private Transform pickupUI;
    public int scrap;
    private Text scrapText;
    public float speed;
    private GameObject detection;
    public bool canMove;
    public GameController gameController;
    public float experienceSpendable;
    public float experience;
    public float experienceForNextLevel;
    public int experienceLevel;
    public float[] experienceLevelUpRequirement;
    public Weapons weaponsList;
    private Vector3 lastDirection;
    private GameObject torch;

    void Start() {		
        rbody = GetComponent<Rigidbody2D>();
        gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        detection = transform.GetChild(2).gameObject;
        gun.reloadMagazine();
        gun.setPlayerController(this);
        scrapText = transform.GetChild(3).GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        healthBar = transform.GetChild(3).GetChild(1).GetChild(1).gameObject;
        pickupUI = transform.GetChild(8);
        torch = transform.GetChild(9).gameObject;
        maxHealth = 100;
        experienceLevelUpRequirement = new float[] { 0f, 1000f, 2000f, 5000f, 10000f };
        experienceSpendable = 200f;
        experience = 0f;
        experienceLevel = 0;
        experienceForNextLevel = experienceLevelUpRequirement[experienceLevel + 1];
        updateXP(0f);
        canMove = true;
        speed = 45f;
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
        detection.transform.GetChild(0).localScale = new Vector3(sneak["detectionDistance"], sneak["detectionDistance"], 0) * 2;
        detection.transform.GetChild(1).localScale = new Vector3(sneak["attackDistance"], sneak["attackDistance"], 0) * 2;
        var movSpeed = speed * gameController.globalSpeed;
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
        // TOGGLE TORCH
        if (Input.GetKeyDown(KeyCode.T)) {
            transform.GetChild(9).gameObject.SetActive(!transform.GetChild(9).gameObject.activeSelf);
            // TODO:
            // Handle stealth change with torch toggle
        }
        movementControls(movSpeed);
        gun.shootingGunCheck();
    }

    private void movementControls(float speed) {
        var movementVector = new Vector3(0, 0, 0);
        if(canMove) {
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
    public void handleDirection(Vector3 direction) {
        if(direction != lastDirection) {
            var diffAngle = Vector3.Angle(lastDirection, direction);
            var normalAngle = Vector3.Angle(new Vector3(0, 1, 0), direction);
            var targetDirection = direction.x > 0 ? Quaternion.Euler(0, 0, 360 - normalAngle) : Quaternion.Euler(0, 0, normalAngle);
            Debug.Log(targetDirection);
            torch.transform.rotation = targetDirection;
        }
        lastDirection = direction;
    }

    // public IEnumerator updateTorchDirection(Quaternion direction) {
    //     while(transform.rotation.z != direction.z) {
    //         transform.Rotate = Quaternion.Euler(0, 0, transform.rotation.z += 0.1f);
    //         yield return new WaitForSeconds (0.001f);
    //     }
    // }

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

    public void pickupItemUI(Pickup pickup) {
        foreach (Transform pickupText in pickupUI) {
            if(pickupText.gameObject.name == pickup.type) {
                var textScript = pickupText.gameObject.GetComponent<PickupText>();
                textScript.timer = 0;
                textScript.value += pickup.value;
                pickupText.gameObject.GetComponent<Text>().text = $"{pickup.type} x{textScript.value}";
                return;
            }
        }
        var pickupTitleObject = Instantiate(pickupUI.GetChild(0).gameObject, new Vector2(0, 0), Quaternion.identity);
        pickupTitleObject.transform.SetParent(pickupUI);
        pickupTitleObject.name = pickup.type;
        pickupTitleObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0.5f * pickupUI.childCount - 1, 0);
        var newTextScript = pickupTitleObject.GetComponent<PickupText>();
        newTextScript.value += pickup.value;
        pickupTitleObject.GetComponent<Text>().text = $"{pickup.type} x{newTextScript.value}";
    }
}
