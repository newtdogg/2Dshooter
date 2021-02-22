using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DoorToScene : MonoBehaviour {
    public string scene;
    public string map;
    public string gameMode;
    private GameController gameController;

    void Start() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.name == "Player") {
            if(map != "") {
                var mapClass = System.Activator.CreateInstance(System.Type.GetType(map)) as MapGrid;
                gameController.activeMap = mapClass.map;
                SceneManager.LoadScene("Production");
            }
            if(scene != "") {
                SceneManager.LoadScene(scene);
            }
            gameController.mode = gameMode;
        }
    }
}
