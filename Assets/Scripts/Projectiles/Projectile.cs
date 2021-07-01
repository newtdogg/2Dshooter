using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float lifetime;
    public string parent;
    public float damage;
    public bool hitTarget;

    public void setLifetime(float lt) {
        lifetime = lt;
    }

    void Update() {
        lifetime -= Time.deltaTime;
        if(lifetime < 0) {
            Destroy(gameObject);
        }
    }

}
