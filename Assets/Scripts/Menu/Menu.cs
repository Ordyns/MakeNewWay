using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("===== Levels =====")]
    [SerializeField] private MenuLevelsView levelsView;

    [Header("===== Environment =====")]
    [SerializeField] private GameObject[] menuIslands;

    private SaveSystem<PlayerData> _saveSystem;
    private PlayerData _data = new PlayerData();

    private LevelsInfoProvider _levelsInfoProvider;
    private System.Action<int>  _loadLevel;

    [Zenject.Inject]
    private void Init(System.Action<int> loadLevel, LevelsInfoProvider levelsInfoProvider){
        _levelsInfoProvider = levelsInfoProvider;
        _loadLevel = loadLevel;
    }

    private void Start() {
        _saveSystem = new SaveSystem<PlayerData>(_data);
        _data = _saveSystem.LoadData();

        InitLevelsView();

        ActivateRandomIsland();
    }

    private void InitLevelsView(){
        int currentCompletedLevel = _data.LastUnlockedLevel;
        IList<int> completedLevelsWithBouns = _data.CompletedLevelsWithBonus;
        IList<int> numbersOfLevelsWithBonus = _levelsInfoProvider.NumbersOfLevelsWithBonus;
        int levelsCount = _levelsInfoProvider.LevelsCount;
        
        levelsView.gameObject.SetActive(true);
        levelsView.Init(_loadLevel, _levelsInfoProvider, currentCompletedLevel, completedLevelsWithBouns);
    }

    private void ActivateRandomIsland(){
        foreach(GameObject island in menuIslands)
            island.SetActive(false);
            
        menuIslands[Random.Range(0, menuIslands.Length)].SetActive(true);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            CloseLevels();
        }
    }

    public void OpenLevels(){
        levelsView.Open();
    }

    public void CloseLevels(){
        levelsView.Close();
    }
}