using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHandler : MonoBehaviour
{
    [SerializeField] private BaseUI baseUI;
    [SerializeField] private BaseSoundsPlayer baseSoundsPlayer;
    [SerializeField] private StepsViewModel stepsViewModel;

    private int _levelNumber;

    private PathChecker _pathChecker;
    
    private void Awake() {
        _levelNumber = ProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber;

        _pathChecker = LevelContext.Instance.PathChecker;
        _pathChecker.PathChecked += OnPathChecked;
    }

    private void OnPathChecked(bool isPathCorrect){
        if(isPathCorrect)
            LevelCompleted();    
        else if(stepsViewModel.StepsLeft == 0)
            LevelNotPassed();
    }

    private void LevelCompleted(){
        ProjectContext.Instance.SaveSystem.LevelCompleted(_levelNumber, stepsViewModel.IsBonusReceived());
        baseSoundsPlayer.PlayLevelCompletedSound(BaseUI.PanelsAnimationDuration);

        bool isLastLevelCompleted = _levelNumber + 1 > ProjectContext.Instance.LevelsContainer.LevelsCount;
        baseUI.LevelCompleted(isLastLevelCompleted);
    }

    private void LevelNotPassed(){
        baseUI.LevelNotPassed();
    }
}
