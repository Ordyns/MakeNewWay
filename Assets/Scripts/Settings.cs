using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public Localization Localization { private set; get; }

    public bool isMusicEnabled { 
        private set => _saveSystem.Data.isMusicEnabled = value;
        get => _saveSystem.Data.isMusicEnabled; 
    }
    public bool isSoundsEnabled { 
        private set => _saveSystem.Data.isSoundsEnabled = value;
        get => _saveSystem.Data.isSoundsEnabled; 
    }

    private SaveSystem _saveSystem;

    private void Awake() {
        Instance = this;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization(){
        while(SaveSystem.Instance == null)
            yield return null;

        _saveSystem = SaveSystem.Instance;
        Localization = Localization.Instance;
    }

    public void ChangeLocalizationToNextLanguage() => Localization.ChangeToNextLanguage();

    public void MusicEnabled(){
        isMusicEnabled = true;
        AudioPlayer.PlayMusic();
    }

    public void MusicDisabled(){
        isMusicEnabled = false;
        AudioPlayer.StopMusic();
    }

    public void SoundsEnabled() => isSoundsEnabled = true;
    public void SoundsDisabled() => isSoundsEnabled = false;
}