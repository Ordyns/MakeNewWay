using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("===== Levels =====")]
    [SerializeField] private MenuLevelsView levelsView;

    [Header("===== Environment =====")]
    [SerializeField] private GameObject[] menuIslands;

    private PlayerData _data;

    private LevelsInfoProvider _levelsInfoProvider;
    private Zenject.SignalBus _signalBus;

    [Zenject.Inject]
    private void Init(Zenject.SignalBus signalBus, PlayerData playerData, LevelsInfoProvider levelsInfoProvider){
        _levelsInfoProvider = levelsInfoProvider;
        _data = playerData;
        _signalBus = signalBus;
    }

    private void Start() {
        InitLevelsView();

        ActivateRandomIsland();
    }

    private void InitLevelsView(){
        int currentCompletedLevel = _data.LastUnlockedLevel;
        IList<int> completedLevelsWithBouns = _data.CompletedLevelsWithBonus;
        IList<int> numbersOfLevelsWithBonus = _levelsInfoProvider.NumbersOfLevelsWithBonus;
        int levelsCount = _levelsInfoProvider.LevelsCount;
        
        levelsView.gameObject.SetActive(true);
        levelsView.Init(_signalBus, _levelsInfoProvider, currentCompletedLevel, completedLevelsWithBouns);
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