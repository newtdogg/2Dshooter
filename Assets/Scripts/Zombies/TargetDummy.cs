using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetDummy : AIController
{
    // Start is called before the first frame update
    void Start()
    {
        health = 9999999999;
        damageIndicatorTimer = 0f;
        damageParent = transform.GetChild(0);
        damageIndicator = damageParent.GetChild(0).gameObject.GetComponent<Text>();
        damageIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -2.34f, 0);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.name.Contains("DoCPlayerBullet")) {
            var projectileScript = col.gameObject.GetComponent<Projectile>();
            updateDamage(projectileScript.damage);
        }
    }
}
