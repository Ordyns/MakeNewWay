/*
    [TEXT]
*/

using UnityEngine;
//using Firebase;
//using Firebase.Analytics;
//using Firebase.Extensions;

public class Analytics : MonoBehaviour
{
    public static Analytics Instance;

    private void Awake() {
        Instance = this;

        /*FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            else
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        });*/
    }

    private void Start() => ScenesLoader.Instance.OnGameLevelLoaded += GameLevelLoaded;

    private void GameLevelLoaded(int levelNumber){
        PathChecker.Instance.OnPathChecked += (pathCorrect) => {
            if(pathCorrect){
                LevelCompleted(levelNumber);
            }
        };
    }
    
    /*public static void TutorialCompleted() => FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialComplete);

    public static void LevelCompleted(int levelNumber) => FirebaseAnalytics.LogEvent($"event_level_{levelNumber}_completed");
    public static void AdFailedToLoad() => FirebaseAnalytics.LogEvent("event_ad_failed_to_load");*/

    public static void TutorialCompleted() => Debug.Log("event_tutorial_completed");
    public static void LevelCompleted(int levelNumber) => Debug.Log($"event_level_{levelNumber}_completed");
    public static void AdFailedToLoad() => Debug.Log("event_ad_failed_to_load");
}
