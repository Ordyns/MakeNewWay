using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] private LevelSettings levelSettings;
    [Space]
    [SerializeField] private IslandsProvider islandsProvider;
    [SerializeField] private IslandsAnimator islandsAnimator;
    [Space]
    [SerializeField] private LevelHintSteps levelHintSteps;
    
    public override void InstallBindings(){
        DeclareSignals();

        BindPlayerData();
        
        Container.BindInstance(levelSettings).AsSingle();

        Container.BindInstance(levelHintSteps).AsSingle();

        Container.BindInstances(islandsAnimator, islandsProvider);

        Container.Bind<PathChecker>().AsSingle().NonLazy();
    }

    private void BindPlayerData(){
        SaveSystem<PlayerData> saveSystem = new SaveSystem<PlayerData>();
        PlayerData data = saveSystem.LoadData();
        Container.BindInstance(data).AsSingle();
    }

    private void DeclareSignals(){
        Container.DeclareSignal<LevelCompletedSignal>();
    }
}