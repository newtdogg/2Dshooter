using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetDummy : AIController
{
    // Start is called before the first frame update
    private BoxCollider2D collider2D;
    private float damageOverTime;
    private float dpsTimer;
    private float dps;
    void Start()
    {
        health = 9999999999;
        damageOverTime = 0f;
        dpsTimer = 5f;
        damageParent = transform.GetChild(0);
        collider2D = GetComponent<BoxCollider2D>();
        damageIndicator = damageParent.GetChild(0).gameObject.GetComponent<Text>();
        damageIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -4f, 0);
    }

    void Update() {
        dpsTimer -= Time.deltaTime;
        if(dpsTimer <= 0) {
            dpsTimer = 5f;
            dps = damageOverTime/5f;
            damageIndicator.text = $"{Mathf.Round(dps).ToString()}";
            damageOverTime = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.name.Contains("DoCPlayerBullet")) {
            var projectileScript = col.gameObject.GetComponent<Projectile>();
            // Physics2D.IgnoreCollision(collider, collider2D);
            damageOverTime += projectileScript.damage;
            if(!damageParent.GetChild(0).gameObject.activeSelf) {
                damageParent.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
