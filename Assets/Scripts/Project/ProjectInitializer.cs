using Zenject;

public class ProjectInitializer
{
    public ProjectInitializer(SignalBus signalBus, ScenesLoader scenesLoader){
        signalBus.Subscribe<LoadMenuSignal>(scenesLoader.LoadMenu);
        signalBus.Subscribe<LoadNextLevelSignal>(scenesLoader.LoadNextLevel);
        signalBus.Subscribe<ReloadLevelSignal>(scenesLoader.RestartLevel);
        signalBus.Subscribe<LoadLevelSignal>((LoadLevelSignal) => scenesLoader.LoadLevel(LoadLevelSignal.LevelNumber));
    }
}
