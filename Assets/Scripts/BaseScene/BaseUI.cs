using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class BaseUI : MonoBehaviour
{
    [HideInInspector] public HintUI HintUI;

    [SerializeField] private StepsViewModel stepsViewModel;

    [Header("===== Main UI =====")]
    [SerializeField] private CanvasGroup mainUI;

    [Header("===== Panels =====")]
    [SerializeField] private CanvasGroup background;
    [Space]
    [SerializeField] private PanelAnimator levelNotPassedPanel;
    [SerializeField] private PanelAnimator levelCompletedPanel;
    [SerializeField] private PanelAnimator allLevelsCompletedPanel;
    [Space]
    [SerializeField] private PanelAnimator pausePanel;

    [Header("===== Bonus =====")]
    [SerializeField] private BonusReceivedView bonusReceivedView;

    [Header("===== Sounds =====")]
    [SerializeField] private AudioClip levelCompletedSound;

    private bool isGamePaused;
    private bool isLevelCompleted;

    private GuideSystem _guideSystem;
    
    private int _levelNumber;

    private Timer _bonusReceivedViewTimer;

    private void Start() {
        if(LevelContext.Instance.LevelSettings == null)
            return;
        
        _levelNumber = ScenesLoader.Instance.LastLoadedLevelNumber;

        _guideSystem = BaseSceneContext.Instance.GuideSystem;

        LevelContext.Instance.PathChecker.PathChecked += OnPathChecked;

        InitMainUI();
    }

    private void InitMainUI(){
        mainUI.alpha = 0;

        if(_guideSystem.IsGuideShowing)
            _guideSystem.GuideFinished += ShowMainUI;
        else
            ShowMainUI();
    }

    private void ShowMainUI() 
        => mainUI.DOFade(1, 1f).SetEase(Ease.OutCubic).SetEase(Ease.OutCubic).SetDelay(0.5f);

    private void OnPathChecked(bool pathCorrect){
        if(pathCorrect)
            LevelCompleted();
        else if(stepsViewModel.StepsLeft == 0)
            LevelNotPassed();
    }

    private void Update() {
        if(isLevelCompleted || _guideSystem.IsGuideShowing)
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
        BaseSceneContext.Instance.IslandsUpdater.IsIslandsUpdatingAllowed = !paused;
        isGamePaused = paused;
        ChangeBackgroundVisibility(paused);

        if(paused)
            pausePanel.OpenPanel();
        else
            pausePanel.ClosePanel(deactivateGameObject: true);
    }

    public void RestartLevel() => ScenesLoader.Instance.RestartLevel();
    public void LoadMenu() => ScenesLoader.Instance.LoadMenu();
    public void LoadNextLevel() => ScenesLoader.Instance.NextLevel();

    public void LevelCompleted(){
        isLevelCompleted = true;

        float panelsAnimationDuration = 1f;
        bool bonusReceived = stepsViewModel.IsBonusReceived();

        mainUI.DOFade(0, 0.2f).SetEase(Ease.OutCubic);
        ChangeBackgroundVisibility(true, panelsAnimationDuration);

        PanelAnimator panel = _levelNumber + 1 <= LevelsContainer.Instance.LevelsCount ? levelCompletedPanel : allLevelsCompletedPanel;
        EnablePanelAfterDelay(panel.gameObject, panelsAnimationDuration);

        if(bonusReceived){
            bonusReceivedView.gameObject.SetActive(true);
            _bonusReceivedViewTimer = TimeOperations.CreateTimer(panelsAnimationDuration + 0.5f, null, () => bonusReceivedView.Show(stepsViewModel.StepsForBonus));
        }

        SaveSystem.Instance.LevelCompleted(_levelNumber, bonusReceived);
        
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

    private void OnDestroy() {
        if(_bonusReceivedViewTimer != null)
            _bonusReceivedViewTimer.Stop();
    }
}