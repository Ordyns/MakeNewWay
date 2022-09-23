using Zenject;

public class TutorialInitializer : IInitializable
{
    private BaseCamera _baseCamera;
    private Tutorial _tutorial;
    private BaseSoundsPlayer _soundsPlayer;

    public TutorialInitializer(Tutorial tutorial, BaseSoundsPlayer soundsPlayer, BaseCamera baseCamera){
        _tutorial = tutorial;
        _baseCamera = baseCamera;
        _soundsPlayer = soundsPlayer;
    }

    [Inject]
    private void InitSignals(SignalBus signalBus, PathChecker pathChecker){
        signalBus.Subscribe<IslandUpdatedSignal>(pathChecker.CheckPath);
        signalBus.Subscribe<LevelCompletedSignal>(OnTutorialFinished);
    }

    public void Initialize(){
        Timer.StartNew(_tutorial, _baseCamera.AnimationDuration, _tutorial.BeginTutorial);
        _baseCamera.PlayInAnimation();
    }

    private void OnTutorialFinished(){
        Analytics.TutorialCompleted();
        _baseCamera.PlayOutAnimation();
        Timer.StartNew(_tutorial, _baseCamera.AnimationDuration, () => _soundsPlayer.PlayLevelCompletedSound());
    }
}
