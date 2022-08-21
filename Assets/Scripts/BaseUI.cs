using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class BaseUI : MonoBehaviour
{
    public static BaseUI Instance;

    public bool isGamePaused { private set; get; }
    public bool isLevelCompleted { private set; get; }

    [HideInInspector] public HintUI HintUI;

    [SerializeField] private CameraAnimator cameraAnimator;

    [Header("===== Main UI =====")]
    [SerializeField] private CanvasGroup mainUI;
    [Space]
    [SerializeField] private TextMeshProUGUI stepsLeftText;
    [SerializeField] private AnimatedButton previousStepButton;

    [Header("===== Review =====")]
    [SerializeField] private ReviewRequestPanel reviewRequestPanelPrefab;
    [SerializeField] private int requestReviewLevelNumber = 10;

    [Header("===== Panels =====")]
    [SerializeField] private CanvasGroup background;
    [Space]
    [SerializeField] private PanelAnimator levelNotPassedPanel;
    [SerializeField] private PanelAnimator levelCompletedPanel;
    [SerializeField] private PanelAnimator allLevelsCompletedPanel;
    [Space]
    [SerializeField] private PanelAnimator pausePanel;

    [Header("===== Bonus =====")]
    [SerializeField] private AnimatedPanel bonusRecievedPanel;
    [SerializeField] private TextMeshProUGUI bonusRecievedText;
    [Space]
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private TextMeshProUGUI stepsForBonusText;
    [SerializeField] private GameObject bonusFilledStar;

    [Header("===== Sounds =====")]
    [SerializeField] private AudioClip levelCompletedSound;

    private SaveSystem _saveSystem;
    private StepsRecorder _stepsRecorder;
    
    private int _levelNumber;
    private int _stepsForBonus;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if(LevelSettings.Instance == null)
            return;
        
        _levelNumber = ScenesLoader.Instance.LastLoadedLevelNumber;
        _stepsForBonus = LevelSettings.Instance.StepsForBonus;

        _saveSystem = SaveSystem.Instance;

        _stepsRecorder = StepsRecorder.Instance;
        _stepsRecorder.StepRecorded += UpdatePreviousStepButton;

        if(_levelNumber >= requestReviewLevelNumber && _saveSystem.Data.ReviewRequested == false){
            Instantiate(reviewRequestPanelPrefab);
            _saveSystem.Data.ReviewRequested = true;
        }

        PathChecker.Instance.PathChecked += OnPathChecked;
        PlayerInput.Instance.OnStepsCountChanged += OnStepsCountChanged;

        OnStepsCountChanged();
        InitMainUI();
    }

    private void InitMainUI(){
        mainUI.alpha = 0;

        int minSteps = LevelSettings.Instance.StepsForBonus;

        bonusPanel.SetActive(minSteps > 0);
        if(SaveSystem.Instance) bonusFilledStar.SetActive(_saveSystem.Data.CompletedLevelsWithBonus.Contains(_levelNumber));
        bonusRecievedText.text = string.Format(bonusRecievedText.text, _stepsForBonus);
        stepsForBonusText.text = string.Format(stepsForBonusText.text, minSteps);

        if(GuideSystem.Instance.isGuideShowing)
            GuideSystem.Instance.GuideFinished += ShowMainUI;
        else
            ShowMainUI();

        UpdatePreviousStepButton();
    }

    private void ShowMainUI() 
        => mainUI.DOFade(1, 1f).SetEase(Ease.OutCubic).SetEase(Ease.OutCubic).SetDelay(0.5f);

    private void OnPathChecked(bool pathCorrect){
        if(pathCorrect)
            LevelCompleted();
        else if(PlayerInput.Instance.StepsLeft == 0)
            LevelNotPassed();
    }

    private void Update() {
        if(isLevelCompleted || GuideSystem.Instance.isGuideShowing)
            return;

        if(Input.GetKeyDown(KeyCode.Escape)){
            if(HintUI.HintOpened){
                HintUI.ClosePanel();
            }
            else if(isGamePaused){
                UnpauseGame();
            }
            else{
                PauseGame();
            }
        }
    }

    public void PauseGame() => SetPauseState(true);
    public void UnpauseGame() => SetPauseState(false);

    private void SetPauseState(bool paused){
        PlayerInput.Instance.IsInputAllowed = !paused;
        isGamePaused = paused;
        ChangeBackgroundVisibility(paused);

        if(paused)
            pausePanel.OpenPanel();
        else
            pausePanel.ClosePanel(deactivateGameObject: true);
    }

    private void OnStepsCountChanged(){
        stepsLeftText.text = PlayerInput.Instance.StepsLeft.ToString();
        stepsLeftText.transform.DOScale(new Vector2(1.1f, 1.1f), 0.2f).SetEase(Ease.OutCubic).OnComplete(() => {
            stepsLeftText.transform.DOScale(Vector2.one, 0.15f).SetEase(Ease.OutCubic);
        });
    }

    public void GoToPrevStep(){
        _stepsRecorder.GoToPreviousStep();

        UpdatePreviousStepButton();
    }

    private void UpdatePreviousStepButton() => previousStepButton.Interactable = _stepsRecorder.CurrentStep > 0;

    public void RestartLevel() => ScenesLoader.Instance.RestartLevel();
    public void LoadMenu() => ScenesLoader.Instance.LoadMenu();
    public void LoadNextLevel() => ScenesLoader.Instance.NextLevel();

    public void LevelCompleted(){
        isLevelCompleted = true;

        float panelsAnimationDuration = 1f;
        bool bonusReceived = LevelSettings.Instance.IsBonusRecieved(PlayerInput.Instance.StepsLeft);

        _saveSystem.LevelCompleted(_levelNumber, bonusReceived);

        mainUI.DOFade(0, 0.2f).SetEase(Ease.OutCubic);
        cameraAnimator.PlayOutAnimation();
        ChangeBackgroundVisibility(true, panelsAnimationDuration);

        if(_levelNumber + 1 <= LevelsContainer.Instance.LevelsCount)
            EnablePanelAfterDelay(levelCompletedPanel.gameObject, panelsAnimationDuration);
        else
            EnablePanelAfterDelay(allLevelsCompletedPanel.gameObject, panelsAnimationDuration);

        if(bonusReceived){
            bonusRecievedPanel.gameObject.SetActive(true);
            bonusRecievedText.text = string.Format(bonusRecievedText.text, _stepsForBonus);
            TimeOperations.CreateTimer(panelsAnimationDuration + 0.5f, null, bonusRecievedPanel.Open);

            if(_saveSystem.Data.CompletedLevelsWithBonus.Contains(_levelNumber) == false)
                _saveSystem.Data.CompletedLevelsWithBonus.Add(_levelNumber);
        }
        
        AudioPlayer.PlayClip(levelCompletedSound, panelsAnimationDuration);
    }

    private void EnablePanelAfterDelay(GameObject panel, float delay) 
        => TimeOperations.CreateTimer(delay, null, () => panel.SetActive(true));

    public void LevelNotPassed(){
        mainUI.DOFade(0, 0.2f).SetEase(Ease.OutCubic);
        levelNotPassedPanel.OpenPanel();
        ChangeBackgroundVisibility(true);
    }

    public void ChangeBackgroundVisibility(bool visible, float delay = 0){
        background.gameObject.SetActive(true);
        
        background.DOFade(visible ? 1 : 0, 0.25f).SetDelay(delay).OnComplete(() => {
            if(visible == false) 
                background.gameObject.SetActive(false);
        });
    }
}