using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsUpdater : MonoBehaviour
{
    public static IslandsUpdater Instance { get; private set; }

    public event System.Action IslandUpdating;
    public event System.Action IslandUpdated;

    public bool IsIslandUpdating { get; private set; }
    //[HideInInspector] 
    public bool IsIslandsUpdatingAllowed { get; set; } = true;

    public int StepsLeft { get; private set; }

    [SerializeField] private Camera mainCamera;

    private PlayerInput<Island> _playerInput;
    private PathChecker _pathChecker;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _pathChecker = PathChecker.Instance;
        StepsLeft = LevelSettings.Instance.Steps;

        _playerInput = new PlayerInput<Island>(mainCamera);
        _playerInput.Click += OnClick;
        _playerInput.Swipe += OnSwipe;
    }

    private void Update() {
        if(IsIslandUpdating || IsIslandsUpdatingAllowed == false)
            return;

        _playerInput.Update();
    }

    private void OnClick(Island island){
        if(island.OnClick(OnUpdatingFinished)){
            UpdatingStarted();
        }
    }

    private void OnSwipe(Island island, Direction direction){
        if(island.OnSwipe(direction, OnUpdatingFinished)){
            UpdatingStarted();
        }
    }

    private void OnUpdatingFinished(){
        IsIslandUpdating = false;
        _pathChecker.CheckPath();
        IslandUpdated?.Invoke();
    }
    public void ExternalUpdate(float duration, int newStepsLeftCount){
        if(IsIslandUpdating)
            throw new System.Exception("The islands are already being updating");
        
        if(Mathf.Abs(StepsLeft - newStepsLeftCount) > 1)
            throw new System.Exception("Incorrect use of ExternalUpdate. (StepsLeft - newStepsLeftCount) must be less than 2");

        StepsLeft = newStepsLeftCount;    
        
        IsIslandUpdating = true;
        TimeOperations.CreateTimer(duration, null, OnUpdatingFinished);
    }

    private void UpdatingStarted(){
        IsIslandUpdating = true;
        IslandUpdating?.Invoke();
        StepsLeft--;
    }
}
