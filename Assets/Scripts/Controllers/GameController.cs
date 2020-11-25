using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

public class GameController : MonoBehaviour {
    public string mode;
    public int[,] activeMap;
    private string[] maps;
    private WaveData waveData;
    private int waveIndex;
    public bool nextWave;
    public bool waveComplete;
    private bool gameStarted;
    public bool survival;
    public float globalSpeed;
    private PlayerController playerController;
    private UnlocksController unlocksController;
    private GameObject door;
    private PersistenceController persistenceController;
    private MapGenerator mapGenerator;
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
        unlocksController.weaponList = persistenceController.saveData.weapons;
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
            var waveString = File.ReadAllText("./Assets/Scripts/WaveData.json");
            waveData = JsonUtility.FromJson<WaveData>(waveString);
            startGameSurvival();
        }
    }

    void Update() {
        checkNextLevel();
    }

    public void checkNextLevel () {
        if(gameStarted) {
            var currentZombieCount = 0;
            foreach (var spawner in spawners) {
                currentZombieCount += spawner.transform.GetChild(0).childCount;
            }
            if(currentZombieCount == 0 && nextWave) {
                nextWave = false;
                StartCoroutine("startWave");
            }
        }
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
        persistenceController.saveData.weapons = unlocksController.weaponList;
        persistenceController.saveGame();
    }

    public IEnumerator startWave() {
        Debug.Log("starting wave");
        Debug.Log(waveIndex);
        yield return new WaitForSeconds (1f);
        var currentWaveInfo = waveData.waves[waveIndex];
        var numberList = Enumerable.Range(0, mapGenerator.spawners.Count - 1).ToList();
        var remainingZombies = 0;
        for(var i = 0; i < currentWaveInfo.groups.Count; i++) {
            var zombieGroup = currentWaveInfo.groups[i];
            var rand = new System.Random((int)System.DateTime.Now.Ticks);
            var randomSpawnerIndex = numberList[rand.Next(numberList.Count)];
            Debug.Log(zombieGroup.quantity);
            Debug.Log(zombieGroup.type);
            numberList.Remove(randomSpawnerIndex);
            remainingZombies += zombieGroup.quantity;
            spawners[randomSpawnerIndex].spawnZombieGroup(zombieGroup.quantity, zombieGroup.type);
        }
        nextWave = true;
        waveIndex += 1;
    }

    public void startGameSurvival() {
        gameStarted = true;
        waveIndex = 0;
        StartCoroutine("startWave");
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
