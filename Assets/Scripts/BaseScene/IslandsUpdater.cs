using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsUpdater : MonoBehaviour
{
    public event System.Action IslandUpdating;
    public event System.Action IslandUpdated;

    public bool IsIslandUpdating { get; private set; }
    [HideInInspector] public bool IsIslandsUpdatingAllowed { get; set; } = true;
    
    [SerializeField] private StepsViewModel stepsViewModel;

    [SerializeField] private Camera mainCamera;

    private PlayerInput<Island> _playerInput;
    private PathChecker _pathChecker;
    

    private void Start() {
        _pathChecker = LevelContext.Instance.PathChecker;

        _playerInput = new PlayerInput<Island>(mainCamera);
        _playerInput.Click += OnClick;
        _playerInput.Swipe += OnSwipe;
    }

    private void Update() {
        if(IsIslandUpdating || IsIslandsUpdatingAllowed == false)
            return;

        if(ProjectContext.Instance.PauseManager.IsPaused)
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
        _pathChecker.CheckPath();
        IslandUpdated?.Invoke();
    }

    public void ExternalUpdate(float duration, StepAction stepAction){
        if(IsIslandUpdating)
            throw new System.Exception("The islands are already being updating");

        if(stepsViewModel)
            stepsViewModel.StepsLeft.Value += stepAction == StepAction.Add ? 1 : -1;
        
        IsIslandUpdating = true;
        Timer.StartNew(this, duration, OnUpdatingFinished);
    }

    private void UpdatingStarted(){
        IsIslandUpdating = true;
        IslandUpdating?.Invoke();
        
        if(stepsViewModel)
            stepsViewModel.StepsLeft.Value--;
    }

    public enum StepAction{
        Add,
        Substract
    }
}
