using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PersistenceController {

    [SerializeField] public SaveData saveData;

    public PersistenceController() {
        saveData = new SaveData();
    }
    public void saveGame() {
        var saveJson = JsonUtility.ToJson(saveData, true);
        File.WriteAllText("./Assets/Scripts/SaveData.json", saveJson); 
    }

    public void loadGame() {
        saveData = new SaveData();  
        var saveJson = File.ReadAllText("./Assets/Scripts/SaveData.json"); 
        saveData = JsonUtility.FromJson<SaveData>(saveJson);
    }
}


[System.Serializable]
public class SaveData {
    
    public float experienceSpendable;
    public float experience;
    public float experienceForNextLevel;
    public int experienceLevel;
    public Weapons weapons;

}