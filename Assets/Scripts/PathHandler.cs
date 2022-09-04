using System;
using UnityEngine;

public class PathHandler : MonoBehaviour
{
    public event Action LevelCompleted;
    public event Action LevelNotPassed;

    [SerializeField] private BaseUI baseUI;
    [SerializeField] private BaseSoundsPlayer baseSoundsPlayer;

    private int _levelNumber;
    private PlayerData _playerData;

    private PathChecker _pathChecker;
    private StepsViewModel _stepsViewModel;
    
    public void Init(PlayerData playerData, StepsViewModel stepsViewModel) {
        _stepsViewModel = stepsViewModel;
        _playerData = playerData;

        _levelNumber = ProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber;

        _pathChecker = LevelContext.Instance.PathChecker;
        _pathChecker.PathChecked += OnPathChecked;
    }

    private void OnPathChecked(bool isPathCorrect){
        if(isPathCorrect)
            OnLevelCompleted();
        else if(_stepsViewModel.StepsLeft == 0)
            OnLevelNotPassed();
    }

    private void OnLevelCompleted(){    
        _playerData.LastUnlockedLevel = _levelNumber + 1;
        if(_stepsViewModel.IsBonusReceived() && _playerData.CompletedLevelsWithBonus.Contains(_levelNumber) == false)
            _playerData.CompletedLevelsWithBonus.Add(_levelNumber);
    
        baseSoundsPlayer.PlayLevelCompletedSound(BaseUI.PanelsAnimationDuration);

        bool isLastLevelCompleted = _levelNumber + 1 > ProjectContext.Instance.LevelsContainer.LevelsCount;
        baseUI.LevelCompleted(isLastLevelCompleted);

        LevelCompleted.Invoke();
    }

    private void OnLevelNotPassed(){
        baseUI.LevelNotPassed();

        LevelNotPassed.Invoke();
    }
}
