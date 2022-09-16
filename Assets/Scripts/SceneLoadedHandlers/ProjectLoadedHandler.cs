using UnityEngine;

public class ProjectLoadedHandler : MonoBehaviour
{
    [SerializeField] private ProjectContext context;

    private void Awake() {
        SavesAdapter savesAdapter = new SavesAdapter();
        if(savesAdapter.IsLegacyDataExists())
            savesAdapter.Adapt();

        context.Settings.Init();
    }

    private void Start() {
        context.AdsManager.Init(context.Localization.GetLocalizedValue);

        LoadNextScene();
    }

    private void LoadNextScene(){
        PlayerData data = new PlayerData();
        SaveSystem<PlayerData> saveSystem = new SaveSystem<PlayerData>(data);
        data = saveSystem.LoadData();

        if(data != null && data.TutorialCompleted)
            context.ScenesLoader.LoadMenu();
        else
            context.ScenesLoader.LoadTutorial();
    }
}
