using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public event System.Action TutorialFinished;

    [SerializeField] private TutorialStep[] steps;
    [Space]
    [SerializeField] private TutorialUI ui;

    private int _currentStepIndex;

    private BaseCamera _baseCamera;
    private IslandsUpdater _islandsUpdater;

    private PlayerData _data = new PlayerData();

    private IIslandUpdateHandler _currentIslandUpdateHandler;

    [Zenject.Inject]
    private void Init(Zenject.SignalBus signalBus, PlayerData playerData, IslandsUpdater islandsUpdater, BaseCamera baseCamera){
        _data = playerData;
        
        _islandsUpdater = islandsUpdater;
        _islandsUpdater.IsIslandsUpdatingAllowed = false;
        
        _baseCamera = baseCamera;

        signalBus.Subscribe<IslandUpdatedSignal>(OnIslandUpdated);
        signalBus.Subscribe<IslandUpdatingSignal>(OnIslandUpdating);

        _currentStepIndex = -1;
    }

    public void BeginTutorial(){
        NextStep();
    }

    public void NextStep(){
        Reset();

        _currentStepIndex++;
        
        if(_currentStepIndex >= steps.Length)
            FinishTutorial();
        else
            StartStep(steps[_currentStepIndex]);
    }

    private void StartStep(TutorialStep step){
        if(step is IIslandUpdateHandler){
            _currentIslandUpdateHandler = (IIslandUpdateHandler) step;
            _islandsUpdater.IsIslandsUpdatingAllowed = true;
        }
                
        step.Completed += NextStep;
        Timer.StartNew(this, step.StartDelay, () => {
            step.Enter(ui.Canvas);
        });
    }

    private void Reset(){
        _islandsUpdater.IsIslandsUpdatingAllowed = false;
        _currentIslandUpdateHandler = null;

        if(_currentStepIndex >= 0){
            steps[_currentStepIndex].Completed -= NextStep;
            steps[_currentStepIndex].Exit();
        }
    }

    private void FinishTutorial(){
        _islandsUpdater.IsIslandsUpdatingAllowed = false;

        _data.TutorialCompleted = true;

        _baseCamera.PlayOutAnimation();
        ui.ShowTutorialCompletedPanel(_baseCamera.AnimationDuration);

        TutorialFinished?.Invoke();
    }

    private void OnIslandUpdating() => _currentIslandUpdateHandler?.OnIslandUpdating();
    private void OnIslandUpdated() => _currentIslandUpdateHandler?.OnIslandUpdated();
}
