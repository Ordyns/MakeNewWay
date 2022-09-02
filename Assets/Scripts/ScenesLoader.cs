using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader : MonoBehaviour
{
    public event System.Action<int> GameLevelLoaded;
    public event System.Action<int> GameLevelLoading;

    [SerializeField] private ScenesTransitions scenesTransitions;
    [Space]
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string baseSceneName = "Base";
    [SerializeField] private string levelSceneName = "Level";

    public int LastLoadedLevelNumber {get; private set;}

    private AdsManager _adsManager;
    private SaveSystem _saveSystem;
    
    private void Start() {
        _adsManager = ProjectContext.Instance.AdsManager;
        _saveSystem = ProjectContext.Instance.SaveSystem;    

        GameLevelLoaded += (levelNumber) => CloseTransition();
        GameLevelLoaded += (levelNumber) => SceneChanged();
        
        if(_saveSystem.Data.TutorialCompleted)
            LoadMenu();
        else
            LoadTutorial();
    }

    public void LoadLevel(int number){
        if(number == 0) number = 1;
        GameLevelLoading?.Invoke(number);

        AddTransition(() => {
            LastLoadedLevelNumber = number;
            SceneManager.LoadSceneAsync($"{levelSceneName} {number}").completed += (asyncOperation) => GameLevelLoaded?.Invoke(number);
            SceneManager.LoadSceneAsync(baseSceneName, LoadSceneMode.Additive);
        });
    }

    public void LoadMenu(){
        LoadScene(menuSceneName);
    } 
    
    private void LoadTutorial() => LoadScene(tutorialSceneName);

    public void NextLevel(){
        ProjectContext.Instance.SaveSystem.SaveAll();
        
        string stringIndex = SceneManager.GetActiveScene().name.Substring(levelSceneName.Length);
        LoadLevel(System.Int32.Parse(stringIndex) + 1);
    }

    private void LoadScene(string name){
        SceneChanged();
        AddTransition(() => {
            ProjectContext.Instance.SaveSystem.SaveAll();
            SceneManager.LoadSceneAsync(name).completed += (asyncOperation) => CloseTransition();
        });
    }

    private void SceneChanged(){
        _adsManager.LoadAds();
        if(AdsManager.AdLevel(LastLoadedLevelNumber)) _adsManager.ShowInterstitialAd(null, null);
    }

    public void RestartLevel(){
        LoadLevel(LastLoadedLevelNumber);
    }

    private void AddTransition(System.Action onBeginLoad) => scenesTransitions.CreateNewTransition(onBeginLoad);
    private void CloseTransition() => scenesTransitions.CloseCurrentTransition();
}