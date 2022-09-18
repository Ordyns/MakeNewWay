using System.Linq;
using UnityEngine;

public class Settings
{
    private Localization _localization;
    private MusicPlayer _musicPlayer;

    public bool IsMusicEnabled { 
        get => _data.IsMusicEnabled; 
        set{
            _data.IsMusicEnabled = value;

            if(value) _musicPlayer.PlayMusic();
            else _musicPlayer.StopMusic();
        } 
    }
    public bool IsSoundsEnabled { 
        get => _data.IsSoundsEnabled;
        set => _data.IsSoundsEnabled = value;
    }

    private Data _data = new Data();
    private SaveSystem<Data> _saveSystem;

    public Settings(Localization localization, MusicPlayer musicPlayer){
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        _localization = localization;
        _musicPlayer = musicPlayer;

        _saveSystem = new SaveSystem<Data>(_data);
        _data = _saveSystem.LoadData();

        if(_data.IsMusicEnabled == false)
            musicPlayer.StopMusic();
    }

    public void ChangeLocalizationToNextLanguage() => _localization.ChangeToNextLanguage();

    public string GetDisplayingNameOfCurrentLanguage() 
        => _localization.Languages.FirstOrDefault(language => language.LanguageCode == _localization.CurrentLanguageCode).DisplayingName;
    
    // TODO
    // private void OnApplicationQuit() { 
        // _saveSystem.SaveData(_data);
    // }

    public class Data : ISaveable
    {
        public bool IsSoundsEnabled = true;
        public bool IsMusicEnabled = true;

        public string FileName => "settings";
    }
}