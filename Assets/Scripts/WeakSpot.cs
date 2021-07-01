using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.name.Contains("DoCPlayerBullet")) {
            Debug.Log(1);
            var projectileScript = col.gameObject.GetComponent<Projectile>();
            // transform.parent.gameObject.GetComponent<AIController>().updateDamage(projectileScript.damage);
            projectileScript.hitTarget = true;
            transform.parent.gameObject.GetComponent<AIController>().updateDamage(projectileScript.damage * 2, "critical");
        }
    }
}
