using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsUpdater : MonoBehaviour, IPauseHandler
{
    public event System.Action IslandUpdating;
    public event System.Action IslandUpdated;

    public bool IsIslandUpdating { get; private set; }
    public bool IsIslandsUpdatingAllowed { get; set; } = true;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BaseSoundsPlayer soundsPlayer;

    private StepsViewModel _stepsViewModel;
    private PlayerInput<Island> _playerInput;

    private bool isPaused;
    private PauseManager _pauseManager;
    
    public void Init(StepsViewModel stepsViewModel, PauseManager pauseManager) {
        _stepsViewModel = stepsViewModel;

        _pauseManager = pauseManager;
        _pauseManager.Subscribe(this);

        _playerInput = new PlayerInput<Island>(mainCamera);
        _playerInput.Click += OnClick;
        _playerInput.Swipe += OnSwipe;
    }

    private void Update() {
        if(isPaused || IsIslandUpdating || IsIslandsUpdatingAllowed == false)
            return;

        _playerInput.Update();
    }

    private void OnClick(Island island){
        if(island is IClickHandler handler && handler.OnClick(OnUpdatingFinished)){
            UpdatingStarted();
        }
    }

    private void OnSwipe(Island island, Direction direction){
        if(island is ISwipeHandler handler && handler.OnSwipe(direction, OnUpdatingFinished)){
            UpdatingStarted();
        }
    }

    private void OnUpdatingFinished(){
        IsIslandUpdating = false;
        IslandUpdated?.Invoke();
    }

    public void ExternalUpdate(float duration, StepAction stepAction){
        if(IsIslandUpdating)
            throw new System.Exception("The islands are already being updating");

        if(_stepsViewModel)
            _stepsViewModel.StepsLeft.Value += stepAction == StepAction.Add ? 1 : -1;
        
        IsIslandUpdating = true;
        Timer.StartNew(this, duration, OnUpdatingFinished);
    }

    private void UpdatingStarted(){
        IsIslandUpdating = true;
        IslandUpdating?.Invoke();

        soundsPlayer.PlaySwipeSound();
        
        if(_stepsViewModel)
            _stepsViewModel.StepsLeft.Value--;
    }

    public void SetPaused(bool isPaused){
        this.isPaused = isPaused;
    }

    public enum StepAction{
        Add,
        Substract
    }
}
