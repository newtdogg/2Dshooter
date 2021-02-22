using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnlocksOverlay : MonoBehaviour,
                              IPointerEnterHandler,
                              IPointerExitHandler,
                              IPointerDownHandler {

    public List<GameObject> weaponNodes;
    public Color defaultColor;

    void Start() {
        weaponNodes = new List<GameObject>();
        foreach (Transform node in transform.parent.GetChild(0)){
            weaponNodes.Add(node.gameObject);
        }
    }
    public void OnPointerEnter(PointerEventData eventData){
        highlightGunSection();
    }

    public void OnPointerExit(PointerEventData eventData){
        dehighlightGunSection();
    }

    private void highlightGunSection() {
        foreach (var weaponNode in weaponNodes) {
            defaultColor = weaponNode.transform.GetChild(1).gameObject.GetComponent<Image>().color;
            weaponNode.transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.7f);
        }
        transform.parent.GetChild(2).localScale = new Vector3(1f, 1.2f, 0f);
    }

    private void dehighlightGunSection() {
        foreach (var weaponNode in weaponNodes) {
            weaponNode.transform.GetChild(1).gameObject.GetComponent<Image>().color = defaultColor;
        }
        transform.parent.GetChild(2).localScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData eventData){
        selectGunGroup();
    }

    public void selectGunGroup() {
        transform.parent.GetChild(2).gameObject.SetActive(false);
        foreach (Transform gunType in transform.parent.parent) {
            gunType.GetChild(1).gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
        foreach (var weaponNode in weaponNodes) {
            weaponNode.GetComponent<WeaponNode>().setNodeFocused();
        }
    }
}
