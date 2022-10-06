using UnityEngine;
using Zenject;

public class BaseInitializer : IInitializable
{
    private IslandsAnimator _islandsAnimator;
    private BaseCamera _baseCamera;
    private BaseUI _baseUI;
    private PathChecker _pathChecker;

    private GuideSystem _guideSystem;
    private ReviewRequestPanel _reviewRequestPanel;
    private int _currentLevelNumber;

    private PlayerData _playerData;

    public BaseInitializer(IslandsAnimator islandsAnimator, BaseCamera baseCamera, BaseUI baseUI, PlayerData playerData){
        _baseCamera = baseCamera;
        _baseUI = baseUI;
        _islandsAnimator = islandsAnimator;
        _playerData = playerData;
    }

    [Inject]
    private void InitLevelStarters(GuideSystem guideSystem, ReviewRequestPanel reviewRequestPanel, int currentLevelNumber){
        _guideSystem = guideSystem;
        _reviewRequestPanel = reviewRequestPanel;
        _currentLevelNumber = currentLevelNumber;
    }

    public void Initialize(){
        if(_reviewRequestPanel.IsTargetLevel(_currentLevelNumber) && _playerData.ReviewRequested == false){
            _playerData.ReviewRequested = true;
            _reviewRequestPanel.ShowPanel();
            _reviewRequestPanel.PanelClosed += _guideSystem.BeginGuide;
            return;
        }

        _reviewRequestPanel.Disable();
        _guideSystem.BeginGuide();
    }
    
    [Inject]
    private void InitSignals(SignalBus signalBus, PathChecker pathChecker){
        signalBus.Subscribe<ILevelStarterSignal>(OnLevelStarted);
        signalBus.Subscribe<IslandUpdatedSignal>(pathChecker.CheckPath);

        _pathChecker = pathChecker;
    }
    
    private void OnLevelStarted(){
        _pathChecker.CheckPath();
        _islandsAnimator.Animate();
        _baseCamera.PlayInAnimation();
        _baseUI.ShowMainUI();
    }
}
