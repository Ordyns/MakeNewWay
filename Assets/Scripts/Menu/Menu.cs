using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("===== Levels =====")]
    [SerializeField] private MenuLevelsView levelsView;

    [Header("===== Environment =====")]
    [SerializeField] private GameObject[] menuIslands;

    private ProjectContext _projectContext;
    private SaveSystem<PlayerData> _saveSystem;
    private PlayerData _data = new PlayerData();

    private void Awake() {
        _saveSystem = new SaveSystem<PlayerData>(_data);
        _data = _saveSystem.LoadData();
    }

    private void Start() {
        ActivateRandomIsland();

        _projectContext = ProjectContext.Instance;
        int currentCompletedLevel = _data.LastUnlockedLevel;
        IList<int> completedLevelsWithBouns = _data.CompletedLevelsWithBonus;
        IList<int> numbersOfLevelsWithBonus = _projectContext.LevelsContainer.NumbersOfLevelsWithBonus;
        int levelsCount = _projectContext.LevelsContainer.LevelsCount;
        
        levelsView.gameObject.SetActive(true);
        levelsView.Init(levelsCount, currentCompletedLevel, completedLevelsWithBouns, numbersOfLevelsWithBonus);
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