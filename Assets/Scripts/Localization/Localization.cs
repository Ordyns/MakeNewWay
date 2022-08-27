using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NaughtyAttributes;
using System.Collections.ObjectModel;

public class Localization : MonoBehaviour
{
    public event System.Action LanguageChanged;

    [SerializeField] private string defaultLanguageCode;
    [Space]
    [SerializeField] private List<Language> languages;
    public ReadOnlyCollection<Language> Languages => languages.AsReadOnly();

    public string CurrentLanguageCode { private set; get; }

    private LocalizedStrings _currentLocalization;

    private void Awake() {
        CurrentLanguageCode = GetUserLanguage();
        LoadLocalization(CurrentLanguageCode);
    }

    private void LoadLocalization(string languageCode){
        var localizationFile = Resources.Load<TextAsset>($"Localizations/{languageCode}");
        _currentLocalization = JsonUtility.FromJson<LocalizedStrings>(localizationFile.text);
    }

    public void ChangeLanguage(string languageCode){
        LoadLocalization(languageCode);
        LanguageChanged?.Invoke();
        CurrentLanguageCode = languageCode;
        PlayerPrefs.SetString("Language", languageCode);
    }

    public string ChangeToNextLanguage(){
        int nextLanguageID = 0;
        
        for(int i = 0; i < languages.Count; i++){
            if(languages[i].LanguageCode == CurrentLanguageCode)
                nextLanguageID = i + 1;
        }

        if(nextLanguageID == languages.Count)
            nextLanguageID = 0;
        
        ChangeLanguage(languages[nextLanguageID].LanguageCode);
        return languages[nextLanguageID].LanguageCode;
    }

    public string GetLocalizedValue(string key){
        try{
            return _currentLocalization.LocalizationList.Find(l => l.Key == key).Value;
        }
        catch{
            Debug.LogError("LocalizationErorr: Key not founded! \n key = " + key);
            return null;
        }
    }

    private string GetUserLanguage(){
        if(PlayerPrefs.HasKey("Language"))
            return PlayerPrefs.GetString("Language");

        foreach(Language language in languages){
            if(Application.systemLanguage == language.SystemLanguage)
                return language.LanguageCode;
        }

        return defaultLanguageCode;
    }

    [System.Serializable]
    public struct Language{
        public string LanguageCode;
        public string DisplayingName;
        public SystemLanguage SystemLanguage;
    }

    [System.Serializable]
    public class LocalizationString{
        public string Key;
        [ResizableTextArea] public string Value;
    }

    [System.Serializable]
    public class LocalizedStrings{
        public List<LocalizationString> LocalizationList;
    }
}