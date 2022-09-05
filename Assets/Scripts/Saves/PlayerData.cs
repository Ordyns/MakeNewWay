using System.Collections.Generic;

public class PlayerData : ISaveable
{
    [UnityEngine.SerializeField] private int _lastUnlockedLevel = 1;
    public int LastUnlockedLevel {
        get => _lastUnlockedLevel;
        set => _lastUnlockedLevel = (value > _lastUnlockedLevel ? value : _lastUnlockedLevel);
    }

    public List<int> CompletedLevelsWithBonus = new List<int>();

    public bool TutorialCompleted;
    public bool ReviewRequested;

    public string FileName => "player_data";
}
