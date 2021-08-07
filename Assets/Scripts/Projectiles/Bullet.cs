using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile {
    public Dictionary<string, bool> defaultProperties;
    public Dictionary<string, bool> properties;

    void Start() {
        defaultProperties = new Dictionary<string, bool>() {
            { "poison", false },
            { "sneak", false }
        };
    }
    // Update is called once per frame
    void Update() {
        if(gameObject.name == "Bullet(Clone)") {
            Destroy (gameObject, lifetime);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name != parent) {
            if(gameObject.name == "DoCPlayerBullet(Clone)" && col.gameObject.name != "DoCPlayerBullet(Clone)") {
                Destroy (gameObject);
            }
            if(col.gameObject.name == "Player") {
                var colliderScript = col.gameObject.GetComponent<PlayerController>();
                colliderScript.health -= damage;
            }
        }
    }
}
