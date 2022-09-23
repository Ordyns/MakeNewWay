using System.IO;
using System.Linq;
using UnityEngine;

public class SavesAdapter
{
    private string _targetDirectory;
    private string _fullPath;

    public SavesAdapter(){
        _targetDirectory = Application.isEditor ? Application.dataPath : Application.persistentDataPath;
        _fullPath = Path.Combine(_targetDirectory, "data.sv");
    }

    public bool IsLegacyDataExists(){
        return File.Exists(_fullPath);
    }

    public void Adapt(){
        if(IsLegacyDataExists() == false)
            return;
        
        string dataAsText = File.ReadAllText(_fullPath);
        LegacyData legacyData = JsonUtility.FromJson<LegacyData>(dataAsText);

        AdaptToPlayerData(legacyData);
        AdaptToGuidesData(legacyData);
        AdaptToSettingsData(legacyData);

        File.Delete(_fullPath);
    }
    
    private void AdaptToPlayerData(LegacyData legacyData){
        PlayerData playerData = new PlayerData(){
            LastUnlockedLevel = legacyData.CurrentLevel,
            CompletedLevelsWithBonus = legacyData.CompletedLevelsWithBonus.ToList(),
            TutorialCompleted = legacyData.TutorialCompleted,
            ReviewRequested = legacyData.ReviewRequested
        };
        new SaveSystem<PlayerData>().SaveData(playerData);
    }

    private void AdaptToGuidesData(LegacyData legacyData){
        GuideSystem.Data guidesData = new GuideSystem.Data();
        guidesData.CompletedGuides = legacyData.CompletedLevelsWithGuides.ToList();
        new SaveSystem<GuideSystem.Data>().SaveData(guidesData);
    }

    private void AdaptToSettingsData(LegacyData legacyData){
        Settings.Data settingsData = new Settings.Data(){
            IsMusicEnabled = legacyData.isMusicEnabled,
            IsSoundsEnabled = legacyData.isSoundsEnabled
        };
        new SaveSystem<Settings.Data>().SaveData(settingsData);
    }

    private class LegacyData
    {
        public int CurrentLevel;
        public int[] CompletedLevelsWithBonus;
        public int[] CompletedLevelsWithGuides;
        public bool TutorialCompleted;
        public bool ReviewRequested;
        public bool isAdViewed;
        public bool isMusicEnabled;
        public bool isSoundsEnabled;
    }
}
