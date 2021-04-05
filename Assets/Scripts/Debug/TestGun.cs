using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGun : GunParent
{
    // Start is called before the first frame update
    public Transform UI;
    public bool active;

    void Awake() {
        active = false;
        UI = GameObject.Find("UI").transform;
    }

    public void setGun (string gun) {
        defaultGunAwake(gun);
        active = true;
        shooting = 0.1f;
    }

    // Update is called once per frame
    void Update() {
        if(active) {
            if(reloadTimer > 0f) {
                reloadTimer -= Time.deltaTime;
                if(reloadTimer <= 0f) {
                    reloadTimer = -1f;
                    reloadMagazine();
                }
            }
            if(reloadTimer < 0 && shooting <= 0f) {
                var rand = new System.Random((int)System.DateTime.Now.Ticks);
                var spread = rand.Next(0, (int)currentStats.spread);
                float spreadFloat = ((float)spread - currentStats.spread/2)/1000;
                directionallyShootGun(new Vector2(transform.position.x, transform.position.y + 0.5f), new Vector2(spreadFloat, 1f));
            }
            shooting -= Time.deltaTime;
        }
    }

    public override void shootGun () {
        shooting = currentStats.shotDelay;
        ammoQuantity -= 1;
        ammoCountUI.text = ammoQuantity.ToString();
        if(ammoQuantity == 0) {
            reload();
        }
        currentStats = statsBaseState.duplicateStats();
    }

    public override void fireBullet () {
        GameObject bullet = Instantiate(bulletObject, bulletPosition, Quaternion.identity) as GameObject;
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.properties = bulletProperties;
        // Debug.Log(bulletScript.properties["poison"]);
        bullet.GetComponent<Rigidbody2D>().AddForce(bulletDirection * currentStats.bulletVelocity);
        bulletScript.setLifetime(currentStats.lifetime);
        bulletScript.parent = gameObject.name;
    }

    void OnMouseDown(){
        UI.GetChild(0).gameObject.SetActive(true);
        UI.GetChild(0).gameObject.GetComponent<GunSelection>().activeNode = gameObject;
    }
}
