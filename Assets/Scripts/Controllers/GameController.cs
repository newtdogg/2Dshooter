using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

public class GameController : MonoBehaviour {
    public string mode;
    public int[,] activeMap;
    private string[] maps;
    private Levels levels;
    private int levelIndex;
    public bool waveComplete;
    private bool gameStarted;
    public bool survival;
    public float globalSpeed;
    private PlayerController playerController;
    private UnlocksController unlocksController;
    private GameObject door;
    private PersistenceController persistenceController;
    private MapGenerator mapGenerator;
    public Spawner bossSpawner;
    public List<Spawner> spawners;

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
        persistenceController.loadGame();
        unlocksController = GameObject.Find("Unlocks").transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<UnlocksController>();
        unlocksController.weaponsList = persistenceController.saveData.weapons;
    }

    void Start() {
        if (SceneManager.GetActiveScene().name != "MainMenu") {
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            playerController.weaponsList = persistenceController.saveData.weapons;
            playerController.gameController = this;
            // playerController.loadData(persistenceController.saveData);
            door = GameObject.Find("Door");
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => saveGame());
            mapGenerator = GameObject.Find("MapGridObject").GetComponent<MapGenerator>();
            mapGenerator.gameController = this;
            var dataString = File.ReadAllText("./Assets/Scripts/LevelData.json");
            levels = JsonConvert.DeserializeObject<Levels>(dataString);
            startGame();
        }
    }

    void Update() {
        checkNextLevel();
    }

    public void checkNextLevel () {
        // if(gameStarted) {
        //     var currentZombieCount = 0;
        //     foreach (var spawner in spawners) {
        //         currentZombieCount += spawner.transform.GetChild(0).childCount;
        //     }
        //     if(currentZombieCount == 0 && nextWave) {
        //         nextWave = false;
        //         StartCoroutine("startWave");
        //     }
        // }
    }

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
        persistenceController.saveData.weapons = unlocksController.weaponsList;
        persistenceController.saveGame();
    }

    public IEnumerator StartLevel() {
        // Debug.Log("starting wave");
        // Debug.Log(levelIndex);
        yield return new WaitForSeconds (1f);
        var remainingZombies = 0;
        for(var i = 0; i < mapGenerator.spawners.Count; i++) {
            var rand = new System.Random((int)System.DateTime.Now.Ticks);
            List<string> zombieGroup = levels.easy[rand.Next(levels.easy.Count)];
            remainingZombies += zombieGroup.Count;
            spawners[i].startSpawnerByType(zombieGroup);
        }
        var bossRand = new System.Random((int)System.DateTime.Now.Ticks);
        bossSpawner.boss = "MobEyeSore";
        playerController.setupEnemyIndicators(mapGenerator.spawners.Values.ToList().Where(spawner => !spawner.empty).ToList());
    }

    public void startGame() {
        gameStarted = true;
        levelIndex = 0;
        StartCoroutine("StartLevel");
    }

    public void globalSpeedSlow() {
        StartCoroutine("globalSlowmo");
    }

    public IEnumerator globalSlowmo() {
        globalSpeed = 0.3f;
        while(globalSpeed < 1) {
            globalSpeed += 0.01f;
            yield return new WaitForSeconds (0.005f);
        }
        globalSpeed = 1;
        yield return null;
    }
}
