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

    public int LastLoadedLevelNumber { get; private set; }

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

    public void NextLevel(){
        string stringIndex = SceneManager.GetActiveScene().name.Substring(levelSceneName.Length);
        LoadLevel(System.Int32.Parse(stringIndex) + 1);
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

    private void AddTransition(System.Action onBeginLoad) => scenesTransitions.CreateNewTransition(onBeginLoad);
    private void CloseTransition() => scenesTransitions.CloseCurrentTransition();
}