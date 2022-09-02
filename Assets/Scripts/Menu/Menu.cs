using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("===== Levels =====")]
    [SerializeField] private MenuLevelsView levelsView;

    [Header("===== Environment =====")]
    [SerializeField] private GameObject[] menuIslands;

    private ProjectContext _projectContext;
    private void Start() {
        ActivateRandomIsland();

        _projectContext = ProjectContext.Instance;
        int currentCompletedLevel = _projectContext.SaveSystem.Data.CurrentLevel;
        IList<int> completedLevelsWithBouns = _projectContext.SaveSystem.Data.CompletedLevelsWithBonus;
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