using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;

public class MenuSettingsUI : MonoBehaviour
{
    [SerializeField] private RectTransform settingsButton;
    [SerializeField] private CanvasGroup[] buttons;
    
    [Header("Audio")]
    [SerializeField] private AudioButton musicButton;
    [SerializeField] private AudioButton soundsButton;
    
    [Header("Language")]
    [SerializeField] private TextMeshProUGUI languageText;

    private bool isSettingsOpened;

    private Settings _settings;

    private void Start() {
        _settings = ProjectContext.Instance.Settings;

        UpdateAudioButtonIcon(soundsButton, _settings.isSoundsEnabled);
        UpdateAudioButtonIcon(musicButton, _settings.isMusicEnabled);

        for(int i = 0; i < buttons.Length; i++){
            buttons[i].alpha = 0;
            buttons[i].blocksRaycasts = false;
        }
       
        UpdateLanguageText();
    }

    public void OnLanguageButtonPressed(){
        _settings.ChangeLocalizationToNextLanguage();
        UpdateLanguageText();
    }

    public void OnSoundsButtonPressed(){
        bool isSoundsEnabled = !_settings.isSoundsEnabled;

        if(isSoundsEnabled) _settings.SoundsEnabled();
        else _settings.SoundsDisabled();

        UpdateAudioButtonIcon(soundsButton, isSoundsEnabled);
    }

    public void OnMusicButtonPressed(){
        bool isMusicEnabled = !_settings.isMusicEnabled;

        if(isMusicEnabled) _settings.MusicEnabled();
        else _settings.MusicDisabled();

        UpdateAudioButtonIcon(musicButton, isMusicEnabled);
    }

    private void UpdateLanguageText() {
        var currentLanguage = _settings.Localization.Languages.FirstOrDefault(language => language.LanguageCode == _settings.Localization.CurrentLanguageCode);
        languageText.text = currentLanguage.DisplayingName;
    }

    private void UpdateAudioButtonIcon(AudioButton audioButton, bool enabled)
        => audioButton.ButtonIcon.sprite = enabled ? audioButton.EnabledSprite : audioButton.DisabledSprite;

    private const float ButtonsAnimationDuration = 0.15f;
    private const float ButtonsAnimationDelay = 0.025f;
    public void ChangeSettingsPanelVisibility(){
        float targetAlpha = isSettingsOpened ? 0 : 1;
        float targetPosition = isSettingsOpened ? 0 : settingsButton.rect.height;

        for(int i = 0; i < buttons.Length; i++){
            buttons[i].transform.DOLocalMoveY(targetPosition * (i + 1), ButtonsAnimationDuration).SetEase(Ease.OutCubic).SetDelay(ButtonsAnimationDelay * i);
            buttons[i].DOFade(targetAlpha, ButtonsAnimationDuration).SetDelay(ButtonsAnimationDelay * i);
            buttons[i].blocksRaycasts = !isSettingsOpened;
        }

        isSettingsOpened = !isSettingsOpened;
    }

    [System.Serializable]
    private struct AudioButton{
        public Image ButtonIcon;
        [Space]
        public Sprite EnabledSprite;
        public Sprite DisabledSprite;
    }
}
