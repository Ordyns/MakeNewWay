using UnityEngine;
using DG.Tweening;

public class BaseUI : MonoBehaviour, IPauseHandler
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

    private bool isLevelCompleted;
    private int _levelNumber;

    private GuideSystem _guideSystem;
    private PauseManager _pauseManager;
    
    
    private Timer _bonusReceivedViewTimer;

    private void Start() {
        if(LevelContext.Instance.LevelSettings == null)
            return;
        
        _levelNumber = ProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber;

        _guideSystem = BaseSceneContext.Instance.GuideSystem;

        _pauseManager = ProjectContext.Instance.PauseManager;
        _pauseManager.Subscribe(this);

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
            else if(_pauseManager.IsPaused){
                UnpauseGame();
            }
            else{
                PauseGame();
            }
        }
    }

    public void SetPaused(bool isPaused){
        ChangeBackgroundVisibility(isPaused);
        
    }

    public void PauseGame(){
        pausePanel.OpenPanel();
        _pauseManager.SetPaused(true);
    }
    public void UnpauseGame(){
        pausePanel.ClosePanel(deactivateGameObject: true);
        _pauseManager.SetPaused(false);
    }

    public void RestartLevel() => ProjectContext.Instance.ScenesLoader.RestartLevel();
    public void LoadMenu() => ProjectContext.Instance.ScenesLoader.LoadMenu();
    public void LoadNextLevel() => ProjectContext.Instance.ScenesLoader.NextLevel();

    public void LevelCompleted(){
        isLevelCompleted = true;

        float panelsAnimationDuration = 1f;
        bool bonusReceived = stepsViewModel.IsBonusReceived();

        mainUI.DOFade(0, 0.2f).SetEase(Ease.OutCubic);
        ChangeBackgroundVisibility(true, panelsAnimationDuration);

        PanelAnimator panel = _levelNumber + 1 <= ProjectContext.Instance.LevelsContainer.LevelsCount ? levelCompletedPanel : allLevelsCompletedPanel;
        Timer.StartNew(this, panelsAnimationDuration, () => panel.gameObject.SetActive(true));

        if(bonusReceived){
            bonusReceivedView.gameObject.SetActive(true);
            _bonusReceivedViewTimer = Timer.StartNew(this, panelsAnimationDuration + 0.5f, () => bonusReceivedView.Show(stepsViewModel.StepsForBonus));
        }

        ProjectContext.Instance.SaveSystem.LevelCompleted(_levelNumber, bonusReceived);
        
        AudioPlayer.PlayClip(levelCompletedSound, panelsAnimationDuration);
    }

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
            _bonusReceivedViewTimer.Dispose();

        _pauseManager.Unsubscribe(this);
    }
}