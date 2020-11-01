using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupText : MonoBehaviour {
    public float timer;
    public float value;
    private RectTransform rectTransform;
    void Start() {
        timer = 0;
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    void Update() {
        rectTransform.anchoredPosition = new Vector3(0, 0.5f * transform.GetSiblingIndex() - 0.5f, 0);
        if(gameObject.name != "Default") {
            timer += Time.deltaTime;
        }
        if(timer > 2f) {
            Destroy(gameObject);
        }
    }
}
