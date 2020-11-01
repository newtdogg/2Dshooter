using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;

public class BuyPoint : MonoBehaviour
{
    public GameObject player;
    public Transform parentUI;
    public string title;

    public void toggleUI(bool value) {
        parentUI.GetChild(0).gameObject.SetActive(value);
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            toggleUI(true);
        }
    }

    void OnCollisionExit2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            toggleUI(false);
        }
    }
}
