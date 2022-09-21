using UnityEngine;
using Zenject;

public class BaseInitializer
{
    private IslandsAnimator _islandsAnimator;
    private BaseCamera _baseCamera;
    private BaseUI _baseUI;

    public BaseInitializer(IslandsAnimator islandsAnimator, BaseCamera baseCamera, BaseUI baseUI){
        _baseCamera = baseCamera;
        _baseUI = baseUI;
        _islandsAnimator = islandsAnimator;
    }

    [Inject]
    private void InitSignals(SignalBus signalBus, PathChecker pathChecker){
        signalBus.Subscribe<ILevelStarterSignal>(OnLevelStarted);
        signalBus.Subscribe<IslandUpdatedSignal>(pathChecker.CheckPath);
    }
    
    private void OnLevelStarted(){
        _islandsAnimator.Animate();
        _baseCamera.PlayInAnimation();
        _baseUI.ShowMainUI();
    }
}
