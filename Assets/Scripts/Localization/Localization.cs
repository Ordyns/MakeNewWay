using System.Collections.Generic;
using UnityEngine;
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

    private LinkedList<LocalizedText> _localizedTexts;

    [Zenject.Inject]
    private void Init(){
        _localizedTexts = new LinkedList<LocalizedText>();

        CurrentLanguageCode = GetUserLanguage();
        LoadLocalization(CurrentLanguageCode);
    }

    private void Start() {
        UpdateTexts();
    }

    private void LoadLocalization(string languageCode){
        var localizationFile = Resources.Load<TextAsset>($"Localizations/{languageCode}");
        _currentLocalization = JsonUtility.FromJson<LocalizedStrings>(localizationFile.text);
    }

    public void ChangeLanguage(string languageCode){
        LoadLocalization(languageCode);
        CurrentLanguageCode = languageCode;
        LanguageChanged?.Invoke();

        PlayerPrefs.SetString("Language", languageCode);

        UpdateTexts();
    }

    private void UpdateTexts(){
        foreach (LocalizedText text in _localizedTexts){
            if(text == null)
                _localizedTexts.Remove(text);

            text.UpdateText(GetLocalizedValue(text.LocalizationKey));
        }
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

    public void AddLocalizedText(LocalizedText localizedText){
        _localizedTexts.AddLast(localizedText);
        localizedText.UpdateText(GetLocalizedValue(localizedText.LocalizationKey));
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