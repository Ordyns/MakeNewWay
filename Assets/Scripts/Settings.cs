using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public Localization Localization { get; private set; }

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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization(){
        while(ProjectContext.Instance == null)
            yield return null;

        _saveSystem = ProjectContext.Instance.SaveSystem;
        Localization = ProjectContext.Instance.Localization;
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