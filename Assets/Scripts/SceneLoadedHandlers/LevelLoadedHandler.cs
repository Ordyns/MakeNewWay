using System;
using UnityEngine;

public class LevelLoadedHandler : MonoBehaviour
{
    [SerializeField] private PathHandler pathHandler;
    [SerializeField] private BaseSoundsPlayer baseSoundsPlayer;
    [Space]
    [SerializeField] private ReviewRequestPanel reviewRequestPanel;

    private BaseSceneContext _baseContext;
    private LevelContext _levelContext;

    private StepsViewModel _stepsViewModel;

    private PlayerData _data = new PlayerData();
    private SaveSystem<PlayerData> _saveSystem;

    private int _loadedlevelNumber;

    private void Awake() {
        _saveSystem = new SaveSystem<PlayerData>(_data);
        _data = _saveSystem.LoadData();

        _baseContext = BaseSceneContext.Instance;
        _levelContext = LevelContext.Instance;

        _loadedlevelNumber = LegacyProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber;

        InitAll();
    }

    private void InitAll(){
        StepsRecorder stepsRecorder = new LevelStepsRecorder(_levelContext.IslandsContainer.Islands);

        bool isBonusReceivedEarlier = _data.CompletedLevelsWithBonus.Contains(_loadedlevelNumber);
        _stepsViewModel = new StepsViewModel(_levelContext.LevelSettings, _baseContext.IslandsUpdater, stepsRecorder, isBonusReceivedEarlier);
        
        _baseContext.BaseCamera.Init(_levelContext.LevelSettings);
        _baseContext.BaseUI.Init(_stepsViewModel, _baseContext.GuideSystem, _baseContext.PauseManager);

        baseSoundsPlayer.IsEnabled = LegacyProjectContext.Instance.Settings.IsSoundsEnabled;

        InitHint();

        _levelContext.PathChecker.Init(_levelContext.IslandsContainer.Islands);
        
        InitGuideSystem();
        InitPathHandler();    
        InitIslandsUpdater();

        _levelContext.IslandsAnimator.Init(_levelContext.IslandsContainer.Islands);
        _levelContext.IslandsAnimator.Animate();

        InitReviewRequestPanel();
    }

    private void InitHint(){
        HintViewModel hintViewModel = new HintViewModel(_levelContext.HintSystem, LegacyProjectContext.Instance.AdsManager);
        Func<string, string> getLocalizedValue = LegacyProjectContext.Instance.Localization.GetLocalizedValue;
        _baseContext.BaseUI.HintUI.Init(hintViewModel, getLocalizedValue);

        HintIslandFactory factory = new HintIslandFactory(_baseContext.HintRenderer);
        _baseContext.HintRenderer.Init(_levelContext.LevelSettings);
        _levelContext.HintSystem.Init(_baseContext.HintRenderer, factory);
    }

    private void InitGuideSystem(){
        _baseContext.GuideSystem.GuideFinished += _baseContext.BaseCamera.PlayInAnimation;
        _baseContext.GuideSystem.GuideFinished += _baseContext.BaseUI.ShowMainUI;
        _baseContext.GuideSystem.Init();
    }

    private void InitPathHandler(){
        pathHandler.LevelCompleted += _baseContext.BaseCamera.PlayOutAnimation;
        pathHandler.LevelCompleted += () => _saveSystem.SaveData(_data);
        pathHandler.Init(_data, _stepsViewModel);
    }

    private void InitIslandsUpdater(){
        _baseContext.IslandsUpdater.Init(_baseContext.PauseManager);
        _baseContext.IslandsUpdater.IslandUpdated += _levelContext.PathChecker.CheckPath;
    }

    private void InitReviewRequestPanel(){
        if(_loadedlevelNumber >= reviewRequestPanel.TargetLevelNumber && _data.ReviewRequested == false){
            _data.ReviewRequested = true;
            reviewRequestPanel.ShowPanel();  
        }
        else{
            reviewRequestPanel.Disable();
        }
    }

    private void OnDestroy() {
        _saveSystem.SaveData(_data);
    }
}
