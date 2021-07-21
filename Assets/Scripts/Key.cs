using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    // Start is called before the first frame update
    public Action pickupCallback;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            pickupCallback();
            Destroy(gameObject);
        }
    }
}
