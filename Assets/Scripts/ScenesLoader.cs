using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader : MonoBehaviour
{
    public event System.Action<int> GameLevelLoaded;
    public event System.Action<int> GameLevelLoading;

    public int LastLoadedLevelNumber { get; private set; }

    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string baseSceneName = "Base";
    [SerializeField] private string levelSceneName = "Level";

    private ScenesTransitions _scenesTransitions;

    [Zenject.Inject]
    private void Init(ScenesTransitions scenesTransitions){
        _scenesTransitions = scenesTransitions;
    }

    public void LoadLevel(int number){
        if(number == 0) number = 1;
        GameLevelLoading?.Invoke(number);

        AddTransition(() => {
            SceneManager.LoadSceneAsync($"{levelSceneName} {number}").completed += (asyncOperation) => LevelLoaded(number);
            SceneManager.LoadSceneAsync(baseSceneName, LoadSceneMode.Additive);
        });
    }

    public void LoadMenu(){
        LoadScene(menuSceneName);
    } 
    
    public void LoadTutorial() => LoadScene(tutorialSceneName);

    public void LoadNextLevel(){
        LoadLevel(LastLoadedLevelNumber + 1);
    }

    private void LoadScene(string name){
        AddTransition(() => {
            SceneManager.LoadSceneAsync(name).completed += (asyncOperation) => CloseTransition();
        });
    }

    public void RestartLevel(){
        LoadLevel(LastLoadedLevelNumber);
    }

    private void LevelLoaded(int levelNumber){
        LastLoadedLevelNumber = levelNumber;
        GameLevelLoaded?.Invoke(levelNumber);
        CloseTransition();
    }

    private void AddTransition(System.Action onBeginLoad) => _scenesTransitions.CreateNewTransition(onBeginLoad);
    private void CloseTransition() => _scenesTransitions.CloseCurrentTransition();
}