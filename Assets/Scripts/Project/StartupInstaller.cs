using UnityEngine;
using Zenject;

public class StartupInstaller : MonoInstaller
{
    private ProjectLoadedHandler _projectLoadedHandler;

    public override void InstallBindings(){    
        Container.Bind<ProjectLoadedHandler>().AsSingle().NonLazy();
    }

    public override void Start(){
        _projectLoadedHandler = Container.Resolve<ProjectLoadedHandler>();
        _projectLoadedHandler.Start();
    }
}