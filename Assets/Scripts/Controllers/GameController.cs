using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public string mode;
    public int[,] activeMap;
    private string[] maps;
    private PlayerController playerController;
    private GameObject door;
    private PersistenceController persistenceController;

    public static GameController controller;

    void Awake() {
        if(controller == null){
            DontDestroyOnLoad(gameObject);
            controller = this;
        }
        else if (controller != this){
            Destroy(gameObject);
        }
        persistenceController = new PersistenceController();
        maps = new string[] { "IntroMap", "DebugMap" };
    }

    void Start() {
        if (SceneManager.GetActiveScene().name != "MainMenu") {
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            playerController.loadData(persistenceController.saveData);
            door = GameObject.Find("Door");
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => saveGame());
        }
    }

    public void nextLevel () {}

    public void returnToIntroArea () {
        Destroy(playerController);
        SceneManager.LoadScene("IntroArea");
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void spawnDoorToNextLevel(Vector3 position) {
        var newDoor = Instantiate(door, position, Quaternion.identity);
        newDoor.GetComponent<Door>().trigger = () => SceneManager.LoadScene("Production");
    }

    public void spawnDoorToIntroArea(Vector3 position) {
        var newDoor = Instantiate(door, position, Quaternion.identity);
        newDoor.GetComponent<Door>().trigger = () => SceneManager.LoadScene("IntroArea");
    }

    public void saveGame() {
        persistenceController.saveData.experienceSpendable = playerController.experienceSpendable;
        persistenceController.saveData.experience = playerController.experience;
        persistenceController.saveData.experienceForNextLevel = playerController.experienceForNextLevel;
        persistenceController.saveData.experienceLevel = playerController.experienceLevel;
        persistenceController.saveGame();
    }
}
