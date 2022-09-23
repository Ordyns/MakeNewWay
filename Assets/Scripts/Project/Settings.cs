using System.Linq;
using UnityEngine;

public class Settings : Zenject.IInitializable
{
    private Localization _localization;

    public bool IsMusicEnabled { 
        get => _data.IsMusicEnabled; 
        set{
            _data.IsMusicEnabled = value;
            _signalBus.Fire(new IsMusicEnabledChangedSignal() { IsEnabled = value });
        } 
    }
    public bool IsSoundsEnabled { 
        get => _data.IsSoundsEnabled;
        set{
            _data.IsSoundsEnabled = value;
            _signalBus.Fire(new IsSoundsEnabledChangedSignal() { IsEnabled = value });
        } 
    }

    private Zenject.SignalBus _signalBus;

    private Data _data;
    private SaveSystem<Data> _saveSystem;

    public Settings(Zenject.SignalBus signalBus, Localization localization){
        _signalBus = signalBus;
        _localization = localization;

        signalBus.Subscribe<OnQuitSignal>(OnQuit);
    }

    void Zenject.IInitializable.Initialize(){
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        _saveSystem = new SaveSystem<Data>();
        _data = _saveSystem.LoadData();

        IsMusicEnabled = _data.IsMusicEnabled;
        IsSoundsEnabled = _data.IsSoundsEnabled;
    }

    public void ChangeLocalizationToNextLanguage() => _localization.ChangeToNextLanguage();

    public string GetDisplayingNameOfCurrentLanguage() 
        => _localization.Languages.FirstOrDefault(language => language.LanguageCode == _localization.CurrentLanguageCode).DisplayingName;

    public void OnQuit(){
        _saveSystem.SaveData(_data);
    }

    public class Data : ISaveable
    {
        public bool IsSoundsEnabled = true;
        public bool IsMusicEnabled = true;

        public string FileName => "settings";
    }

    public struct IsSoundsEnabledChangedSignal 
    { 
        public bool IsEnabled;
    }

    public struct IsMusicEnabledChangedSignal 
    { 
        public bool IsEnabled;
    }
}