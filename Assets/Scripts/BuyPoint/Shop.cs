using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;

public class Shop : BuyPoint
{
    // Start is called before the first frame update
    private Weapons weapons;
    private PlayerController playerController;
    private GameObject gunButton;
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        gunButton = transform.GetChild(0).gameObject;
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/Weapons.json"); 
        weapons = JsonUtility.FromJson<Weapons>(jsonString);
        if(gameObject.name.Contains("(Clone)")) {
            displayGuns();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayGuns() {
        var index = 0;
        foreach (var weapon in weapons.GetType().GetProperties()) {
            var weaponScript = weapon.GetValue(weapons) as Weapon;
            generateGunButtonInShop(weaponScript, index);
            index += 1;
        }
        Debug.Log(index);
    }

    public void generateGunButtonInShop(Weapon weapon, int index) {
        var button = Instantiate(gunButton, new Vector2(2, 0), Quaternion.identity);
        button.transform.SetParent(transform.GetChild(1));
        button.transform.localScale = new Vector3(1, 1, 1);
        button.transform.localPosition = new Vector3(0, 164 - (index * 120));
        button.transform.GetChild(0).gameObject.GetComponent<Text>().text = weapon.title;
        button.transform.GetChild(1).gameObject.GetComponent<Text>().text = weapon.cost.ToString();
        var buttonScript = button.GetComponent<Button>();
        buttonScript.onClick.RemoveAllListeners();
        Debug.Log(weapon.script);
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
}
