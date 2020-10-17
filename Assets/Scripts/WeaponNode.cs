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
    private bool locked;
    void Start() {
        gunInfoModal = transform.parent.parent.parent.parent.parent.parent.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setupNode(Weapon wp) {
        weapon = wp;
        if(weapon.unlocked || weapon.isUnlockable()) {
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
        if(locked) {
            defaultColor = transform.GetChild(1).gameObject.GetComponent<Image>().color;
            transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(defaultColor.r - 0.1f, defaultColor.g - 0.1f, defaultColor.b - 0.1f, 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        Debug.Log("pissssss");
        if(active) {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(active) {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        // gunInfoModal.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData){
        setupGunInfoModel();
    }

    public void setupGunInfoModel() {
        gunInfoModal.gameObject.SetActive(true);
        gunInfoModal.GetChild(0).GetChild(1).gameObject.GetComponent<Image>().color = new Color(defaultColor.r - 0.2f, defaultColor.g - 0.2f, defaultColor.b - 0.2f, 1f);
        var gunInfoTransform = gunInfoModal.GetChild(1);
        gunInfoTransform.GetChild(0).gameObject.GetComponent<Text>().text = weapon.title;
        gunInfoTransform.GetChild(1).gameObject.GetComponent<Text>().text = weapon.group;
        var gunInfoTransformStats = gunInfoTransform.GetChild(4);
        gunInfoTransformStats.GetChild(0).gameObject.GetComponent<Text>().text = weapon.stats.damage.ToString();
        gunInfoTransformStats.GetChild(1).gameObject.GetComponent<Text>().text = weapon.stats.ammoCapacity.ToString();
        gunInfoTransformStats.GetChild(2).gameObject.GetComponent<Text>().text = weapon.stats.reloadSpeed.ToString();
        gunInfoTransformStats.GetChild(3).gameObject.GetComponent<Text>().text = weapon.stats.shotDelay.ToString();
        gunInfoTransformStats.GetChild(4).gameObject.GetComponent<Text>().text = weapon.stats.loudness.ToString();
        gunInfoTransformStats.GetChild(5).gameObject.GetComponent<Text>().text = weapon.stats.getAccuracy().ToString();
        gunInfoTransformStats.GetChild(6).gameObject.GetComponent<Text>().text = weapon.stats.getVelocity().ToString();

    }
}
