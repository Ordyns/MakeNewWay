using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public DataClass Data = new DataClass();
    private string _path;

    private void Awake() {
#if UNITY_ANDROID && !UNITY_EDITOR
        _path = Path.Combine(Application.persistentDataPath, "data.sv");
#else
        _path = Path.Combine(Application.dataPath, "data.sv");
#endif

        if (File.Exists(_path)) Data = JsonUtility.FromJson<DataClass>(File.ReadAllText(_path));
        else SaveAll();
    }

    public void SaveAll() => File.WriteAllText(_path, JsonUtility.ToJson(Data));

    private void OnApplicationPause(bool pause) {
#if !UNITY_EDITOR
        SaveAll();
#endif
    }

    private void OnApplicationQuit() {
        SaveAll();
    }

    public void LevelCompleted(int levelNumber, bool bonusReceived){
        if(levelNumber >= Data.CurrentLevel){
            Data.CurrentLevel++;

            if(bonusReceived && Data.CompletedLevelsWithBonus.Contains(levelNumber) == false){
                Data.CompletedLevelsWithBonus.Add(levelNumber);
            }
        }
    }
    
    [System.Serializable]
    public class DataClass{
        public int CurrentLevel = 1;
        public List<int> CompletedLevelsWithBonus = new List<int>();
        public List<int> CompletedLevelsWithGuides = new List<int>();
        [Space]
        public bool TutorialCompleted;
        public bool ReviewRequested;
        [Space]
        public bool isAdViewed;
        [Space]
        public bool isMusicEnabled = true;
        public bool isSoundsEnabled = true;
    }
}