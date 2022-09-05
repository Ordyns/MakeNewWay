using System.Linq;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private Localization localization;
    [SerializeField] private MusicPlayer musicPlayer;

    public bool IsMusicEnabled { 
        get => _data.IsMusicEnabled; 
        set{
            _data.IsMusicEnabled = value;

            if(value) musicPlayer.PlayMusic();
            else musicPlayer.StopMusic();
        } 
    }
    public bool IsSoundsEnabled { 
        get => _data.IsSoundsEnabled;
        set => _data.IsSoundsEnabled = value;
    }

    private Data _data = new Data();
    private SaveSystem<Data> _saveSystem;

    private void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void Init(){
        _saveSystem = new SaveSystem<Data>(_data);
        _data = _saveSystem.LoadData();

        if(_data.IsMusicEnabled == false)
            musicPlayer.StopMusic();
    }

    public void ChangeLocalizationToNextLanguage() => localization.ChangeToNextLanguage();
    public string GetCurrentLanguageCode(){
        return localization.Languages.FirstOrDefault(language => language.LanguageCode == localization.CurrentLanguageCode).DisplayingName;
    }

    private void OnApplicationQuit() {
        _saveSystem.SaveData(_data);
    }

    public class Data : ISaveable
    {
        public bool IsSoundsEnabled = true;
        public bool IsMusicEnabled = true;

        public string FileName => "settings";
    }
}