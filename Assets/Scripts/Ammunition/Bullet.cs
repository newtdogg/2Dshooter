using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime;
    public Dictionary<string, bool> defaultProperties;
    public Dictionary<string, bool> properties;

    void Start() {
        defaultProperties = new Dictionary<string, bool>() {
            { "poison", false }
        };
    }
    // Update is called once per frame
    void Update() {
        if(gameObject.name == "Bullet(Clone)") {
            Destroy (gameObject, lifetime);
        }
    }

    public void setLifetime(float lt) {
        lifetime = lt;
    }

    void OnCollisionEnter2D(Collision2D col) {
        Destroy (gameObject);
    }
}
