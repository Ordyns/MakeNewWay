using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimatedPanel))]
public class MenuLevelsView : MonoBehaviour
{
    [SerializeField] private Transform levelsPanelContent;
    [SerializeField] private LevelButton levelButtonPrefab;
    [Space]
    [SerializeField] private float nextLevelButtonAnimationDelay;

    private List<LevelButton> _levelsButtons;

    private AnimatedPanel _animatedPanel;

    private void OnValidate() {
        _animatedPanel = GetComponent<AnimatedPanel>();
    }

    public void Init(int levelsCount, int currentCompletedLevel, IList<int> completedLevelsWithBonus, IList<int> numbersOfLevelsWithBonus){
        _levelsButtons = new List<LevelButton>();

        for(int i = 1; i < levelsCount + 1; i++){
            LevelButton levelButton = Instantiate(levelButtonPrefab, levelsPanelContent);
            levelButton.SetLevelNumber(i);
            
            if(i < currentCompletedLevel) levelButton.LevelCompleted();
            else levelButton.SetLockedState(i > currentCompletedLevel);

            levelButton.SetBonusActive(numbersOfLevelsWithBonus.Contains(i));
            if(completedLevelsWithBonus.Contains(i)) levelButton.BonusReceived();

            _levelsButtons.Add(levelButton);
        }

        _animatedPanel.CloseInstantly();
    }

    public void Open(){
        _animatedPanel.Open();

        for(int i = 0; i < _levelsButtons.Count; i++)
            _levelsButtons[i].Animate(nextLevelButtonAnimationDelay * i);
    }

    public void Close(){
        _animatedPanel.Close();
    }
}