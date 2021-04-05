using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

public class GunSelection : MonoBehaviour {
    // Start is called before the first frame update
    public Weapons weaponsList;
    public GameObject selectGunButton;
    public Transform gunButtonList;
    public GameObject activeNode;
    void Awake()
    {
        var gunSelectionUI = gameObject;
        selectGunButton = gunSelectionUI.transform.GetChild(2).gameObject;
        gunButtonList = gunSelectionUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json"); 
        weaponsList = JsonConvert.DeserializeObject<Weapons>(jsonString);
        var count = 0;
        foreach (var weaponObject in weaponsList.GetType().GetProperties()) {
            var weaponScript = weaponObject.GetValue(weaponsList) as Weapon;
            var button = Instantiate(selectGunButton, new Vector2(0, 0), Quaternion.identity);
            button.transform.SetParent(gunButtonList.transform.parent.GetChild(0));
            // button.transform.localScale = new Vector3(1, 1, 1);
            button.transform.localPosition = new Vector3(270f, -90f - (count * 150), 0);
            button.transform.GetChild(1).gameObject.GetComponent<Text>().text = weaponScript.title;
            var buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() => selectGun(weaponScript));
            count += 1;
        }
    }

    public void selectGun(Weapon weapon) {
        gameObject.SetActive(false);
        activeNode.GetComponent<TestGun>().setGun(weapon.script);
    }

}
