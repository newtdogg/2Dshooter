using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // Start is called before the first frame update
    public Gun gun;
    void Start() {
        gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        gun.reloadMagazine();
        gun.shootingDirection = Vector2Int.right;
    }

    // Update is called once per frame
    void Update()
    {
        gun.shootingGunCheck();
    }
}
