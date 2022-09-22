using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

    [Zenject.Inject]
    private void Init(Settings settings){
        _settings = settings;
    }

    private void Start() {
        UpdateAudioButtonIcon(soundsButton, _settings.IsSoundsEnabled);
        UpdateAudioButtonIcon(musicButton, _settings.IsMusicEnabled);

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
        _settings.IsSoundsEnabled = !_settings.IsSoundsEnabled;
        UpdateAudioButtonIcon(soundsButton, _settings.IsSoundsEnabled);
    }

    public void OnMusicButtonPressed(){
        _settings.IsMusicEnabled = !_settings.IsMusicEnabled;
        UpdateAudioButtonIcon(musicButton, _settings.IsMusicEnabled);
    }

    private void UpdateLanguageText() {
        languageText.text = _settings.GetDisplayingNameOfCurrentLanguage();
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
