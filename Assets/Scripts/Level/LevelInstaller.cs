using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] private LevelSettings levelSettings;
    [SerializeField] private LevelHintSteps levelHintSteps;
    [Space]
    [SerializeField] private IslandsProvider islandsProvider;
    [SerializeField] private IslandsAnimator islandsAnimator;
    
    public override void InstallBindings(){
        DeclareSignals();
        
        Container.BindInstance(levelSettings).AsSingle();

        Container.BindInstance(levelHintSteps).AsSingle();

        Container.BindInstances(islandsAnimator, islandsProvider);

        Container.Bind<PathChecker>().AsSingle().NonLazy();
    }
    
    private void DeclareSignals(){
        Container.DeclareSignal<LevelCompletedSignal>();
    }
}