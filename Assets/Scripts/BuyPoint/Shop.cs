using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;

public class Shop : BuyPoint {
    // Start is called before the first frame update
    private Weapons weapons;
    private PlayerController playerController;
    private GameObject gunButton;

    void Start()
    {
        title = "Weapons";
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        parentUI = GameObject.Find("BuyParent").transform;
        gunButton = parentUI.GetChild(0).GetChild(0).gameObject;

        // var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/Weapons.json"); 
        // weapons = JsonUtility.FromJson<Weapons>(jsonString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            toggleUI(true);
            displayGuns();
        }
    }

    public void displayGuns() {
        parentUI.GetChild(0).GetChild(2).gameObject.GetComponent<Text>().text = title;
        var index = 0;
        foreach(Transform button in parentUI.GetChild(0).GetChild(4)) {
            Destroy(button.gameObject);
        }
        foreach (var weapon in getAvailableGunsToBuy()) {
            generateGunButtonInShop(weapon, index);
            index++;
        }
    }

    public void generateGunButtonInShop(Weapon weapon, int index) {
        var button = Instantiate(gunButton, new Vector2(2, 0), Quaternion.identity);
        button.transform.SetParent(parentUI.GetChild(0).GetChild(4));
        button.transform.localScale = new Vector3(1, 1, 1);
        button.transform.localPosition = new Vector3(0, 164 - (index * 120));
        button.transform.GetChild(0).gameObject.GetComponent<Text>().text = weapon.title;
        button.transform.GetChild(1).gameObject.GetComponent<Text>().text = weapon.cost.ToString();
        var buttonScript = button.GetComponent<Button>();
        buttonScript.onClick.RemoveAllListeners();
        buttonScript.GetComponent<Button>().onClick.AddListener(() => updateGun(weapon.script, weapon.cost));
    }

    public void updateGun(string gun, int cost) {
        if(playerController.scrap > cost) {
            var gunObject = player.transform.GetChild(0).gameObject;
            Destroy(gunObject.GetComponent<Gun>());
            gunObject.AddComponent(System.Type.GetType(gun));
            playerController.updateScrap(-cost);
        }
    }

    public List<Weapon> getAvailableGunsToBuy() {
        var gunTiers = getWeaponTierCount();
        var availableWeapons = new List<Weapon>();
        Debug.Log(gunTiers["Handgun"]);
        foreach (var weaponObject in playerController.weaponsList.GetType().GetProperties()) {
            var weaponScript = weaponObject.GetValue(playerController.weaponsList) as Weapon;
            if(gunTiers.ContainsKey(weaponScript.group)) {
                if(weaponScript.unlocked && weaponScript.tier <= gunTiers[weaponScript.group]) {
                    Debug.Log(weaponScript.title);
                    availableWeapons.Add(weaponScript);
                }
            }
        }
        return availableWeapons;
    }

    public Dictionary<string, int> getWeaponTierCount() {
        var weaponTiers = new Dictionary<string, int>();
        var weaponRecipesParent = playerController.transform.GetChild(6);
        foreach (Transform recipe in weaponRecipesParent) {
            var recipeWeaponGroup = recipe.gameObject.GetComponent<WeaponRecipe>().gunType;
            if(weaponTiers.ContainsKey(recipeWeaponGroup)) {
                weaponTiers[recipeWeaponGroup]++;
            } else {
                weaponTiers.Add(recipeWeaponGroup, 1);
            }
        }
        return weaponTiers;
    }
}
