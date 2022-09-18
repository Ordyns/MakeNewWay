using System;
using UnityEngine;
using Zenject;

public class MenuInstaller : MonoInstaller
{
    [SerializeField] private Menu menu;

    public override void InstallBindings(){
        Container.BindInstance(menu).AsSingle().NonLazy();

        var addLocalizedText = (Action<LocalizedText>) Container.Resolve<Localization>().AddLocalizedText;
        Container.BindInstance(addLocalizedText).WhenInjectedInto<LocalizedText>().NonLazy();

        var loadLevel = (Action<int>) Container.Resolve<ScenesLoader>().LoadLevel;
        Container.BindInstance(loadLevel).WhenInjectedInto<Menu>().NonLazy();
    }
}