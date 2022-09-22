public class ProjectLoadedHandler
{
    private ScenesLoader _scenesLoader;

    public ProjectLoadedHandler(ScenesLoader scenesLoader){
        SavesAdapter savesAdapter = new SavesAdapter();
        if(savesAdapter.IsLegacyDataExists())
            savesAdapter.Adapt();

        _scenesLoader = scenesLoader;
    }

    public void Start() {
        Analytics.Init();
        
        LoadNextScene();
    }

    private void LoadNextScene(){
        PlayerData data = new PlayerData();
        SaveSystem<PlayerData> saveSystem = new SaveSystem<PlayerData>();
        data = saveSystem.LoadData();

        if(data != null && data.TutorialCompleted)
            _scenesLoader.LoadMenu();
        else
            _scenesLoader.LoadTutorial();
    }
}
