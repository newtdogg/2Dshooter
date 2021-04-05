using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;

public class BuyPoint : MonoBehaviour {

    public GameObject UIobject;

    public void toggleUI(bool value) {
        UIobject.SetActive(value);
    }
    
    void OnCollisionEnter2D(Collision2D col) {
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
