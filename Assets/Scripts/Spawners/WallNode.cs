using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallNode : MonoBehaviour
{
    // Start is called before the first frame update
    public Spawner spawner;
    public int hp;
    public Text hpText;
    private float timer;
    void Start() {
        if(gameObject.name.Contains("Clone")){
            spawner = transform.parent.parent.gameObject.GetComponent<Spawner>();
        }
        hp = 5;
        hpText = transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        hpText.text = hp.ToString();
        timer = 1f;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update() {
        if(timer <= 0f) {
            timer = 1f;
            loseHpPoint();
        }
        if(hp <= 0f) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.name.Contains("Mob")) {
            loseHpPoint();
        }
        if (col.gameObject.name.Contains("DoC")) {
            loseHpPoint();
        }
    }

    void OnCollisionStay2D(Collision2D col) {
        if (col.gameObject.name.Contains("Mob")) {
            timer -= Time.deltaTime;
        }
    }

    private void loseHpPoint() {
        hp -= 1;
        hpText.text = hp.ToString();
        transform.GetChild(0).gameObject.SetActive(true);
    }
}