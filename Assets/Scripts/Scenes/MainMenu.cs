﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    // private GameController gameController;
    void Start() {
        // gameController = GameObject.Find("GameController").GetComponent<GameController>();
        // foreach (Transform button in transform.GetChild(1)) {
        //     button.gameObject.GetComponent<Button>().onClick.AddListener(() => loadMap(button.gameObject.name));;
        // }
        transform.GetChild(1).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("IntroArea"));
    }

    // private void loadMap(string mapName) {
    //     var mapClass = System.Activator.CreateInstance(System.Type.GetType(mapName)) as MapGrid;
    //     gameController.activeMap = mapClass.map;
    //     SceneManager.LoadScene("Production");
    // }
}