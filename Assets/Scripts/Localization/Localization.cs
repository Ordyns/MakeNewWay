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
    private void Init(Zenject.SignalBus signalBus){
        _localizedTexts = new LinkedList<LocalizedText>();
        InitSignals(signalBus);

        CurrentLanguageCode = GetUserLanguage();
        LoadLocalization(CurrentLanguageCode);
    }

    private void InitSignals(Zenject.SignalBus signalBus){
        signalBus.DeclareSignal<ObjectCreatedSignal<LocalizedText>>();
        signalBus.Subscribe<ObjectCreatedSignal<LocalizedText>>(AddLocalizedText);

        signalBus.DeclareSignal<ObjectDestroyedSignal<LocalizedText>>();
        signalBus.Subscribe<ObjectDestroyedSignal<LocalizedText>>(RemoveLocalizedText);
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
        var current = _localizedTexts.First;
        while(current != null){
            if(current == null)
                _localizedTexts.Remove(current);

            current.Value.UpdateText(GetLocalizedValue(current.Value.LocalizationKey));

            current = current.Next;
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
            Debug.LogError("LocalizationErorr: Key not founded! \n key: " + key);
            return null;
        }
    }

    private void AddLocalizedText(ObjectCreatedSignal<LocalizedText> signal){
        LocalizedText localizedText = signal.Object;
        localizedText.UpdateText(GetLocalizedValue(localizedText.LocalizationKey));
        _localizedTexts.AddLast(localizedText);
    }

    private void RemoveLocalizedText(ObjectDestroyedSignal<LocalizedText> signal){
        _localizedTexts.Remove(signal.Object);
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