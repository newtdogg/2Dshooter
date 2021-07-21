using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterController : MonoBehaviour
{
    // Start is called before the first frame update

    public float health;
    public float maxHealth;
    public GameObject healthBar;

    // void Start()
    // {
    //     maxHealth = health;
    // }

    // Update is called once per frame


    public void updateHealth(float healthValue) {
        health += healthValue;
        var healthPercentage = health/maxHealth;
        var tf = healthBar.transform.GetChild(0);
        tf.localScale = new Vector3(healthPercentage, 0.1f, 0);
        tf.position = new Vector3(transform.position.x - ((1.1f - healthPercentage)/2), tf.position.y, 0);
    }

}
