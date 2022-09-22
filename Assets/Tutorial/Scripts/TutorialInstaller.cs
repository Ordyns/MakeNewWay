using UnityEngine;
using Zenject;

public class TutorialInstaller : MonoInstaller
{
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private IslandsProvider islandsProvider;
    [SerializeField] private BaseCamera baseCamera;
    [SerializeField] private LevelSettings levelSettings;
    [SerializeField] private BaseSoundsPlayer soundsPlayer;

    public override void InstallBindings(){
        Container.DeclareSignal<LevelCompletedSignal>();

        Container.Bind<PathChecker>().AsSingle().NonLazy();

        Container.BindInstance(tutorial).AsSingle();
        Container.BindInstance(islandsProvider).AsSingle();
        Container.BindInstance(levelSettings).AsSingle();
        Container.BindInstance(baseCamera).AsSingle();
        Container.BindInstance(soundsPlayer).AsSingle().NonLazy();

        BindIslandsUpdater();

        Container.BindInterfacesAndSelfTo<TutorialInitializer>().AsSingle().NonLazy();
    }

    private void BindIslandsUpdater(){
        Container.Bind<PauseManager>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<IslandsUpdater>().AsSingle().WithArguments((MonoBehaviour) this, baseCamera.Camera);

        Container.DeclareSignal<IslandUpdatedSignal>();
        Container.DeclareSignal<IslandUpdatingSignal>();
        Container.DeclareSignal<CantUpdateIslandSignal>();
    }

    private void BindStepsViewModel(int currentLevelNumber){
        bool isBonusReceivedEarlier = Container.Resolve<PlayerData>().CompletedLevelsWithBonus.Contains(currentLevelNumber);
        Container.Bind<StepsViewModel>().AsSingle().WithArguments(isBonusReceivedEarlier);
    }
}