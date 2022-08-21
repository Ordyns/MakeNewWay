using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader : MonoBehaviour
{
    public static ScenesLoader Instance;

    public event System.Action<int> GameLevelLoaded;
    public event System.Action<int> GameLevelLoading;

    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string baseSceneName = "Base";
    [SerializeField] private string levelSceneName = "Level";

    public int LastLoadedLevelNumber {get; private set;}

    private AdsManager _adsManager;
    private SaveSystem _saveSystem;

    private Transition _transition;

    private void Awake() => Instance = this;

    private void Start() {
        _adsManager = AdsManager.Instance;
        _saveSystem = SaveSystem.Instance;    

        GameLevelLoaded += (levelNumber) => CloseTransition(null);
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
        SaveSystem.Instance.SaveAll();
        
        string stringIndex = SceneManager.GetActiveScene().name.Substring(levelSceneName.Length);
        LoadLevel(System.Int32.Parse(stringIndex) + 1);
    }

    private void LoadScene(string name){
        SceneChanged();
        AddTransition(() => {
            SaveSystem.Instance.SaveAll();
            SceneManager.LoadSceneAsync(name).completed += CloseTransition;
        });
    }

    private void SceneChanged(){
        _adsManager.LoadAds();
        if(AdsManager.AdLevel(LastLoadedLevelNumber)) _adsManager.ShowInterstitialAd(null, null);
    }

    public void RestartLevel(){
        LoadLevel(LastLoadedLevelNumber);
    }

    private void AddTransition(System.Action onBeginLoad) => _transition = ScenesTransition.CreateTransition(onBeginLoad);
    private void CloseTransition(AsyncOperation asyncOperation) => _transition.Close();
}