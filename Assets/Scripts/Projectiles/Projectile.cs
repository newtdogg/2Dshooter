using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float lifetime;
    public string parent;
    public float damage;

    public void setLifetime(float lt) {
        lifetime = lt;
    }

}