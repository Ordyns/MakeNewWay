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
        DeclareSignals();

        BindSettings();
        BindScenesLoader();
        BindLocalization();
        BindAudio();
        
        Container.BindInstances(adsManager, levelsInfoProvider);

        Container.Bind<ProjectInitializer>().AsSingle().NonLazy();
    }

    private void DeclareSignals(){
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<OnQuitSignal>();
    }

    private void BindSettings(){
        Container.BindInterfacesAndSelfTo<Settings>().AsSingle();
        
        Container.DeclareSignal<Settings.IsMusicEnabledChangedSignal>();
        Container.DeclareSignal<Settings.IsSoundsEnabledChangedSignal>();
    }

    private void BindScenesLoader(){
        Container.BindInstance(scenesLoader).AsSingle();
        Container.Bind<ScenesTransitions>().FromInstance(scenesTransitions).WhenInjectedInto<ScenesLoader>();

        Container.DeclareSignal<LoadMenuSignal>();
        Container.DeclareSignal<LoadNextLevelSignal>();
        Container.DeclareSignal<ReloadLevelSignal>();
        Container.DeclareSignal<LoadLevelSignal>();
    }

    private void BindLocalization(){
        Container.BindInstance(localization).AsSingle();
        Container.BindInstance((Func<string, string>) localization.GetLocalizedValue).AsSingle();
    }

    private void BindAudio(){
        Container.BindInstance(musicPlayer).AsSingle();
        Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle().NonLazy();
    }

    private void OnApplicationQuit() {
        Container.Resolve<SignalBus>().Fire<OnQuitSignal>();
    }
}