using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    public bool active = false;
    // Start is called before the first frame update
    public Gun gun;
    void Start() {
        gun = transform.GetChild(0).gameObject.GetComponent<Gun>();
        gun.reloadMagazine();
        gun.shootingDirection = Vector2Int.right;
    }

    // Update is called once per frame
    void Update() {
        if(active) {
            gun.shootingGunCheck();
        }
    }

    public void startTurret() {
        active = true;
        gun.defaultGunAwake(gun.script);
        transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = gun.title;
    }
}
