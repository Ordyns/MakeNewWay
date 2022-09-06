using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadedHandler : MonoBehaviour
{
    [SerializeField] private StepsViewModel stepsViewModel;
    [Space]
    [SerializeField] private PathHandler pathHandler;
    [SerializeField] private BaseSoundsPlayer baseSoundsPlayer;
    [Space]
    [SerializeField] private ReviewRequestPanel reviewRequestPanel;

    private BaseSceneContext _baseContext;
    private LevelContext _levelContext;

    private PlayerData _data = new PlayerData();
    private SaveSystem<PlayerData> _saveSystem;

    private int _loadedlevelNumber;

    private void Awake() {
        _saveSystem = new SaveSystem<PlayerData>(_data);
        _data = _saveSystem.LoadData();

        _baseContext = BaseSceneContext.Instance;
        _levelContext = LevelContext.Instance;

        _loadedlevelNumber = ProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber;

        InitAll();
    }

    private void InitAll(){
        stepsViewModel.Init(_baseContext.IslandsUpdater, new List<int>(_data.CompletedLevelsWithBonus), _loadedlevelNumber);
        
        _baseContext.BaseCamera.Init(_levelContext.LevelSettings);
        _baseContext.BaseUI.Init(stepsViewModel, _baseContext.GuideSystem, _baseContext.PauseManager);

        baseSoundsPlayer.IsEnabled = ProjectContext.Instance.Settings.IsSoundsEnabled;

        _levelContext.HintSystem.Init(_baseContext.BaseUI.HintUI);

        _levelContext.PathChecker.Init(_levelContext.IslandsContainer.Islands);
        
        InitGuideSystem();
        InitPathHandler();    
        InitIslandsUpdater();

        InitReviewRequestPanel();
    }

    private void InitGuideSystem(){
        _baseContext.GuideSystem.GuideFinished += _baseContext.BaseCamera.PlayInAnimation;
        _baseContext.GuideSystem.GuideFinished += _baseContext.BaseUI.ShowMainUI;
        _baseContext.GuideSystem.Init();
    }

    private void InitPathHandler(){
        pathHandler.LevelCompleted += _baseContext.BaseCamera.PlayOutAnimation;
        pathHandler.LevelCompleted += () => _saveSystem.SaveData(_data);
        pathHandler.Init(_data, stepsViewModel);
    }

    private void InitIslandsUpdater(){
        _baseContext.IslandsUpdater.Init(stepsViewModel, _baseContext.PauseManager);
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
}
