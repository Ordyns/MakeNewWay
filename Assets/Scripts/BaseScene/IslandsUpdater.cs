using UnityEngine;

public class IslandsUpdater : MonoBehaviour
{
    public event System.Action IslandUpdating;
    public event System.Action IslandUpdated;
    public event System.Action CantUpdateIsland;

    public bool IsIslandUpdating { get; private set; }
    public bool IsIslandsUpdatingAllowed { get; set; }
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BaseSoundsPlayer soundsPlayer;

    private PlayerInput<Island> _playerInput;

    private PauseManager _pauseManager;

    private Zenject.SignalBus _signalBus;
    
    [Zenject.Inject]
    private void Init(PauseManager pauseManager) {
        _pauseManager = pauseManager;

        _playerInput = new PlayerInput<Island>(mainCamera);
        _playerInput.Click += OnClick;
        _playerInput.Swipe += OnSwipe;

        IsIslandsUpdatingAllowed = true;
    }

    [Zenject.Inject]
    private void InitSignals(Zenject.SignalBus signalBus){
        _signalBus = signalBus;
    }

    private void Update() {
        if(_pauseManager.IsPaused || IsIslandUpdating)
            return;

        _playerInput.Update();
    }

    private void OnClick(Island island){
        if(AreIslandsCanBeUpdated() == false)
            return;

        if(island is IClickHandler handler && handler.OnClick(OnUpdatingFinished)){
            UpdatingStarted();
        }
    }

    private void OnSwipe(Island island, Direction direction){
        if(AreIslandsCanBeUpdated() == false)
            return;

        if(island is ISwipeHandler handler && handler.OnSwipe(direction, OnUpdatingFinished)){
            UpdatingStarted();
        }
    }

    private void OnUpdatingFinished(){
        IsIslandUpdating = false;
        IslandUpdated?.Invoke();

        _signalBus.Fire<IslandUpdatedSignal>();
    }

    public void ExternalUpdateStarted(float duration){
        if(IsIslandUpdating)
            throw new System.Exception("The islands are already being updating");
        
        IsIslandUpdating = true;
        Timer.StartNew(this, duration, OnUpdatingFinished);
    }

    private void UpdatingStarted(){
        IsIslandUpdating = true;
        IslandUpdating?.Invoke();
        _signalBus.Fire<IslandUpdatingSignal>();

        soundsPlayer.PlaySwipeSound();
    }

    private bool AreIslandsCanBeUpdated(){
        if(IsIslandsUpdatingAllowed == false){
            CantUpdateIsland?.Invoke();
            _signalBus.Fire<CantUpdateIslandSignal>();
            return false;
        }

        return true;
    }
}
