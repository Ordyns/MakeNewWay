using UnityEngine;
using Zenject;

public class MenuInstaller : MonoInstaller
{
    [SerializeField] private Menu menu;

    public override void InstallBindings(){
        Container.BindInstance(menu).AsSingle().NonLazy();
    }
}