using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("===== Levels =====")]
    [SerializeField] private AnimatedPanel levelsPanel;
    [Space]
    [SerializeField] private Transform levelsPanelContent;
    [SerializeField] private LevelButton levelButtonPrefab;
    [Space]
    [SerializeField] private float nextLevelButtonAnimationDelay;

    [Header("===== Environment =====")]
    [SerializeField] private GameObject[] menuIslands;

    private List<LevelButton> _levelsButtons;

    private void Start() {
        ActivateRandomIsland();

        levelsPanel.gameObject.SetActive(true);
        
        CreateLevelsButtons(); 
    }

    private void ActivateRandomIsland(){
        foreach(GameObject island in menuIslands)
            island.SetActive(false);
        menuIslands[Random.Range(0, menuIslands.Length)].SetActive(true);
    }

    private void CreateLevelsButtons(){
        _levelsButtons = new List<LevelButton>();
        int currentUnlockedLevel = ProjectContext.Instance.SaveSystem.Data.CurrentLevel;
        for(int i = 1; i < ProjectContext.Instance.LevelsContainer.LevelsCount + 1; i++){
            LevelButton levelButton = Instantiate(levelButtonPrefab, levelsPanelContent);
            levelButton.SetLevelNumber(i);
            
            if(i < currentUnlockedLevel) levelButton.LevelCompleted();
            else if(currentUnlockedLevel == i) levelButton.SetLockedState(false);
            else levelButton.SetLockedState(true);

            levelButton.SetBonusActive(ProjectContext.Instance.LevelsContainer.NumbersOfLevelsWithBonus.Contains(i));
            if(ProjectContext.Instance.SaveSystem.Data.CompletedLevelsWithBonus.Contains(i)) levelButton.BonusReceived();

            _levelsButtons.Add(levelButton);
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            CloseLevels();
        }
    }

    public void OpenLevels(){
        levelsPanel.Open();
        for(int i = 0; i < _levelsButtons.Count; i++)
            _levelsButtons[i].Animate(nextLevelButtonAnimationDelay * i);
    }

    public void CloseLevels(){
        levelsPanel.Close();
    } 
}