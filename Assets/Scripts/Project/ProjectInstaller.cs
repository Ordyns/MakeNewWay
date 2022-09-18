using UnityEngine;
using Zenject;
using System;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] public AdsManager adsManager;
    [SerializeField] public LevelsInfoProvider levelsInfoProvider;
    [SerializeField] public Localization localization;
    [SerializeField] public ScenesLoader scenesLoader;
    [SerializeField] public MusicPlayer musicPlayer;
    [SerializeField] public ScenesTransitions scenesTransitions;

    public override void InstallBindings(){       
        Container.BindInstances(musicPlayer, levelsInfoProvider, scenesLoader);
        Container.BindInstances(localization, (Func<string, string>) localization.GetLocalizedValue);

        Container.Bind<ScenesTransitions>().FromInstance(scenesTransitions).WhenInjectedInto<ScenesLoader>();

        Container.Bind<Settings>().FromNew().AsSingle().NonLazy();

        Container.BindInstance(adsManager).NonLazy();
    }
}