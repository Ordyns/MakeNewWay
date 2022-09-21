using UnityEngine;
using Zenject;

public class BaseInstaller : MonoInstaller
{
    [SerializeField] private HintSystem hintSystem;
    [SerializeField] private GuideSystem guideSystem;
    [SerializeField] private IslandsUpdater islandsUpdater;
    [SerializeField] private BaseCamera baseCamera;
    [SerializeField] private HintRenderer hintRenderer;
    [SerializeField] private BaseUI baseUI;
    [SerializeField] private BaseSoundsPlayer baseSoundsPlayer;

    public override void InstallBindings(){
        Container.Bind<StepsRecorder>().To<LevelStepsRecorder>().AsSingle().NonLazy();
        Container.Bind<PauseManager>().FromNew().AsSingle().NonLazy();

        BindIslandsUpdater();
        BindHint();

        int currentLevelNumber = Container.Resolve<ScenesLoader>().LastLoadedLevelNumber;
        BindGuideSystem(currentLevelNumber);
        BindStepsViewModel(currentLevelNumber);
        BindLevelCompletedHandler(currentLevelNumber);

        BindOtherBaseScipts();

        Container.Bind<BaseInitializer>().AsSingle().NonLazy();
    }

    private void BindHint(){
        Container.Bind<HintViewModel>().AsSingle();
        Container.Bind<HintIslandFactory>().AsSingle();

        Container.BindInstance(hintRenderer).AsSingle();
        Container.BindInstance(baseCamera.Camera).WhenInjectedInto<HintRenderer>().NonLazy();

        Container.BindInstance(hintSystem).AsSingle();
    }

    private void BindIslandsUpdater(){
        Container.BindInstance(islandsUpdater).AsSingle().NonLazy();

        Container.DeclareSignal<IslandUpdatedSignal>();
        Container.DeclareSignal<IslandUpdatingSignal>();
        Container.DeclareSignal<CantUpdateIslandSignal>();
    }

    private void BindGuideSystem(int currentLevelNumber){
        Container.BindInstance(guideSystem).AsSingle().NonLazy();
        Container.BindInstance(currentLevelNumber).WhenInjectedInto<GuideSystem>();
        Container.DeclareSignalWithInterfaces<GuideSystem.GuideCompletedSignal>();
    }

    private void BindStepsViewModel(int currentLevelNumber){
        System.Func<PlayerData, bool> getter = data => data.CompletedLevelsWithBonus.Contains(currentLevelNumber);
        Container.Bind<bool>().FromResolveGetter<PlayerData>(getter).WhenInjectedInto<StepsViewModel>();
        Container.Bind<StepsViewModel>().AsSingle();
    }

    private void BindLevelCompletedHandler(int currentLevelNumber){
        Container.BindInstance(currentLevelNumber).WhenInjectedInto<LevelCompletedHandler>();
        Container.Bind<LevelCompletedHandler>().AsSingle().NonLazy();
    }

    private void BindOtherBaseScipts(){
        Container.BindInstance(baseUI).AsSingle();
        Container.BindInstance(baseCamera).AsSingle();
        Container.BindInstance(baseSoundsPlayer).AsSingle();
    }
}