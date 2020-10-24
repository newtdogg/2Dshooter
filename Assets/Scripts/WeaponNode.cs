using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponNode : MonoBehaviour,
                          IPointerEnterHandler,
                          IPointerExitHandler,
                          IPointerDownHandler {
    // Start is called before the first frame update
    public Weapon weapon;
    private Transform gunInfoModal;
    public bool active;
    public Color defaultColor;
    public int parentIndex;
    private bool locked;
    void Start() {
        gunInfoModal = transform.parent.parent.parent.parent.parent.parent.GetChild(1);
        defaultColor = transform.GetChild(1).gameObject.GetComponent<Image>().color;
        var closeButton = gunInfoModal.GetChild(1).GetChild(6).gameObject.GetComponent<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => gunInfoModal.gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setupNode(Weapon wp, int pIndex) {
        parentIndex = pIndex;
        weapon = wp;
        if(weapon.isUnlockable()) {
            transform.GetChild(2).gameObject.GetComponent<Text>().text = weapon.title;
            locked = false;
        } else {
            locked = true;
            transform.GetChild(2).gameObject.GetComponent<Text>().text = "Locked";
        }
        transform.GetChild(2).gameObject.SetActive(false);
    }

    public void setNodeFocused() {
        active = true;
        transform.GetChild(2).gameObject.SetActive(true);
        Debug.Log($"{weapon.title}, {weapon.isUnlockable()} | {weapon.unlocked}");
        if(weapon.isUnlockable()) {
            transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(defaultColor.r - 0.1f, defaultColor.g - 0.1f, defaultColor.b - 0.1f, 1f);
        }
        if (weapon.unlocked) {
            transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(defaultColor.r - 0.2f, defaultColor.g - 0.2f, defaultColor.b - 0.2f, 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        Debug.Log("pissssss");
        if(active && !locked) {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(active && !locked) {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        // gunInfoModal.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData){
        if(!locked) {
            setupGunInfoModel();
        }
    }

    public void setupGunInfoModel() {
        gunInfoModal.gameObject.SetActive(true);
        gunInfoModal.GetChild(0).GetChild(1).gameObject.GetComponent<Image>().color = new Color(defaultColor.r - 0.2f, defaultColor.g - 0.2f, defaultColor.b - 0.2f, 1f);
        var gunInfoTransform = gunInfoModal.GetChild(1);
        gunInfoTransform.GetChild(0).gameObject.GetComponent<Text>().text = weapon.title;
        gunInfoTransform.GetChild(1).gameObject.GetComponent<Text>().text = weapon.group;
        var gunInfoTransformStats = gunInfoTransform.GetChild(4);
        var buyButton = gunInfoModal.GetChild(1).GetChild(5).gameObject.GetComponent<Button>();
        if(transform.parent.parent.parent.GetComponent<UnlocksController>().playerController.experienceSpendable > weapon.cost) {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => unlockGun());
        } else {
            buyButton.interactable = false;
        }
        gunInfoTransformStats.GetChild(0).gameObject.GetComponent<Text>().text = weapon.stats.damage.ToString();
        gunInfoTransformStats.GetChild(1).gameObject.GetComponent<Text>().text = weapon.stats.ammoCapacity.ToString();
        gunInfoTransformStats.GetChild(2).gameObject.GetComponent<Text>().text = weapon.stats.reloadSpeed.ToString();
        gunInfoTransformStats.GetChild(3).gameObject.GetComponent<Text>().text = weapon.stats.shotDelay.ToString();
        gunInfoTransformStats.GetChild(4).gameObject.GetComponent<Text>().text = weapon.stats.loudness.ToString();
        gunInfoTransformStats.GetChild(5).gameObject.GetComponent<Text>().text = weapon.stats.getAccuracy().ToString();
        gunInfoTransformStats.GetChild(6).gameObject.GetComponent<Text>().text = weapon.stats.getVelocity().ToString();
        gunInfoTransformStats.GetChild(7).gameObject.GetComponent<Text>().text = $"{weapon.cost}xp";
    }

    public void unlockGun() {
        var unlocks = transform.parent.parent.parent.GetComponent<UnlocksController>();
        Debug.Log($"{unlocks.playerController.experienceSpendable}, {weapon.cost}");
        var unlocksWeapon = unlocks.weaponList.GetType().GetProperty(weapon.script).GetValue(unlocks.weaponList, null) as Weapon;
        gunInfoModal.gameObject.SetActive(false);
        unlocksWeapon.unlocked = true;
        unlocks.playerController.experienceSpendable -= weapon.cost;
        unlocks.xp.text = unlocks.playerController.experienceSpendable.ToString();
        unlocks.initialiseWeaponNodes(parentIndex);
        unlocks.updatePlayerWeapons();
    }
}
