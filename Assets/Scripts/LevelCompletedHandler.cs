public class LevelCompletedHandler
{
    private BaseUI _baseUI;
    private BaseSoundsPlayer _baseSoundsPlayer;
    private BaseCamera _baseCamera;

    private int _levelNumber;
    private PlayerData _playerData;

    private StepsViewModel _stepsViewModel;
    private LevelsInfoProvider _levelsInfoProvider;
    
    [Zenject.Inject]
    private void Init(LevelsInfoProvider levelsInfoProvider, PlayerData playerData, StepsViewModel stepsViewModel) {
        _levelsInfoProvider = levelsInfoProvider;
        _stepsViewModel = stepsViewModel;
        _playerData = playerData;
    }

    [Zenject.Inject]
    private void InitBase(BaseUI baseUI, BaseSoundsPlayer baseSoundsPlayer, BaseCamera baseCamera){
        _baseUI = baseUI;
        _baseSoundsPlayer = baseSoundsPlayer;
        _baseCamera = baseCamera;
    }

    [Zenject.Inject]
    private void InitSignals(Zenject.SignalBus signalBus, int levelNumber){
        _levelNumber = levelNumber;

        signalBus.Subscribe<LevelCompletedSignal>(OnLevelCompleted);
    }

    private void OnLevelCompleted(){
        _playerData.LastUnlockedLevel = _levelNumber + 1;
        if(_stepsViewModel.IsBonusReceived() && _playerData.CompletedLevelsWithBonus.Contains(_levelNumber) == false)
            _playerData.CompletedLevelsWithBonus.Add(_levelNumber);
    
        _baseCamera.PlayOutAnimation();

        bool isLastLevelCompleted = _levelNumber + 1 > _levelsInfoProvider.LevelsCount;
        _baseUI.LevelCompleted(isLastLevelCompleted);

        _baseSoundsPlayer.PlayLevelCompletedSound(BaseUI.PanelsAnimationDuration);
    }
}
