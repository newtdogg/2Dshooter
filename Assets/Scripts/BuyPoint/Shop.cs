using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

public class Shop : MonoBehaviour {
    // Start is called before the first frame update
    
    private Weapons weapons;
    private PlayerController playerController;
    public GameObject player;
    public Weapons weaponData;
    public Transform parentUI;
    private GameObject gunButton;
    private Text gunInfoTitle;
    private Text gunInfoDamage;
    private Text gunInfoAmmoCap;
    private Button buyButton;

    void Start() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        parentUI = transform;
        gunButton = parentUI.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject;
        var weaponJsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json");
        weaponData = JsonConvert.DeserializeObject<Weapons>(weaponJsonString);
        parentUI.gameObject.SetActive(false);
        gunInfoTitle = parentUI.GetChild(4).GetChild(0).gameObject.GetComponent<Text>();
        gunInfoDamage = parentUI.GetChild(4).GetChild(2).gameObject.GetComponent<Text>();
        gunInfoAmmoCap = parentUI.GetChild(4).GetChild(4).gameObject.GetComponent<Text>();
        buyButton = parentUI.GetChild(4).GetChild(5).gameObject.GetComponent<Button>();
        selectGunGroup("Handgun");
        updateGunInfo(playerController.unlockedWeapons["Pistol9mm"]);
        parentUI.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => selectGunGroup("Handgun"));
        parentUI.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => selectGunGroup("SMG"));
    }

    private void selectGunGroup(string group) {
        var weaponsInGroup = getAvailableGunsToBuy().Where(gunScript => gunScript.group == group);
        var allWeaponsInGroup = weaponData.getWeaponsByGroup(group).Where(weapon => !weaponsInGroup.Contains(weapon));
        foreach (Transform button in gunButton.transform.parent.GetChild(0)) {
            Destroy(button.gameObject);
        }
        var count = 0;
        foreach (var wep in weaponsInGroup) {
            generateGunButtonInShop(wep, count, true);
            count += 1;
        }
        foreach (var wep in allWeaponsInGroup) {
            generateGunButtonInShop(wep, count, false);
            count += 1;
        }
    }

    private void generateGunButtonInShop(Weapon weapon, int index, bool interactable) {
        var button = Instantiate(gunButton, new Vector2(0, 0), Quaternion.identity);
        button.transform.SetParent(gunButton.transform.parent.GetChild(0));
        // button.transform.localScale = new Vector3(1, 1, 1);
        button.transform.localPosition = new Vector3(-10f, 1.5f - (index * 93), 0);
        button.transform.GetChild(1).gameObject.GetComponent<Text>().text = weapon.title;
        var buttonScript = button.GetComponent<Button>();
        if(interactable) {
            buttonScript.onClick.RemoveAllListeners();
            buttonScript.onClick.AddListener(() => updateGunInfo(weapon));
        } else {
            buttonScript.interactable = false;
        }
    }

    public void purchaseGun(string gun, Dictionary<string, int> cost) {
        var gunObject = player.transform.GetChild(0).gameObject;
        Destroy(gunObject.GetComponent<Gun>());
        gunObject.AddComponent(System.Type.GetType(gun));
        playerController.updateScrap(cost);
    }

    public void updateGunInfo(Weapon gun) {
        gunInfoTitle.text = gun.title;
        gunInfoDamage.text = gun.stats.damage.ToString();
        gunInfoAmmoCap.text = gun.stats.ammoCapacity.ToString();
        Debug.Log(gun.cost);
        buyButton.interactable = playerController.checkScapAmount(gun.cost);
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => purchaseGun(gun.script, gun.cost));
    }

    public List<Weapon> getAvailableGunsToBuy() {
        var gunTiers = getWeaponTierCount();
        var availableWeapons = new List<Weapon>();
        foreach (var weaponObject in playerController.unlockedWeapons) {
            var weaponScript = weaponObject.Value;
            if(gunTiers.ContainsKey(weaponScript.group)) {
                if(weaponScript.unlocked && weaponScript.tier <= gunTiers[weaponScript.group]) {
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
        // return new Dictionary<string, int>() {
        //     { "Handgun" , 5 },
        //     { "SMG" , 5 }
        // };
    }
}
