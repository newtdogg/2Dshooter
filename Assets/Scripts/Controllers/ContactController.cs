using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactController : MonoBehaviour {
    private bool inBrawl;
    private GameObject brawlUI;
    private List<GameObject> zombiesInBrawl;
    public int difficulty;
    public float sectionTimerDefault;
    private PlayerController playerController;
    public int zombieCount;
    private List<KeyCode> keys;
    public int counter;
    private int keyIndex;
    private float timer;

    void Start() {
        difficulty = 4;
        inBrawl = false;
        brawlUI = transform.GetChild(0).gameObject;
        zombiesInBrawl = new List<GameObject>();
        playerController = transform.parent.parent.gameObject.GetComponent<PlayerController>();
        keys = new List<KeyCode>() {
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow
        };
    }

    void Update() {
        zombieCount = zombiesInBrawl.Count;
    }
    public void updateBrawlStatus(GameObject zombie) {
        zombiesInBrawl.Add(zombie);
        zombieCount += 1;
        if(zombieCount == 1 && !inBrawl) {
            startBrawl();
        };
    }

    public void startBrawl() {
        inBrawl = true;
        brawlUI.SetActive(true);
        StartCoroutine("manageZombiesAttackingPlayer");
    }

    public IEnumerator manageZombiesAttackingPlayer() {
        keyIndex = singleZombieAttack();
        counter = 0;
        timer = 0f;
        while(zombiesInBrawl.Count > 0) {
            if(timer > 1.5f) {
                handleBrawlComplete(-10f);
                yield return new WaitForSeconds (0.2f);
            }
            if(Input.GetKey(keys[keyIndex])) {
                handleBrawlComplete(-5f);
                yield return new WaitForSeconds (0.2f);
            } else if(Input.GetKey(keys[0]) || Input.GetKey(keys[1]) || Input.GetKey(keys[2]) || Input.GetKey(keys[3])) {
                handleBrawlComplete(-10f);
                yield return new WaitForSeconds (0.2f);
            }
            yield return new WaitForSeconds (0.01f);
            timer += 0.01f;
        }
        playerController.canMove = true;
        brawlUI.SetActive(false);
        inBrawl = false;
        yield return null;
    }

    private void handleBrawlComplete(float health) {
        completeStageOfBrawl(keyIndex);
        keyIndex = singleZombieAttack();
        playerController.updateHealth(health);
    }

    private void completeStageOfBrawl(int keyIndex) {
        timer = 0;
        brawlUI.transform.GetChild(keyIndex).gameObject.SetActive(false);
        counter += 1;
        if(difficulty / counter == 1) {
            counter = 0;
            Destroy(zombiesInBrawl[0]);
            zombiesInBrawl.Remove(zombiesInBrawl[0]);
        }
    }

    public int singleZombieAttack() {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        var keyIndex = rand.Next(0, keys.Count);
        brawlUI.transform.GetChild(keyIndex).gameObject.SetActive(true);
        return keyIndex;
    }
}
