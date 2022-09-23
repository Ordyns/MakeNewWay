using UnityEngine;
//using Firebase;
//using Firebase.Analytics;
//using Firebase.Extensions;

public static class Analytics
{
    public static void Init() {
        /*FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            else
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        });*/
    }
    
    /*public static void TutorialCompleted() => FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialComplete);

    public static void LevelCompleted(int levelNumber) => FirebaseAnalytics.LogEvent($"event_level_{levelNumber}_completed");
    public static void AdFailedToLoad() => FirebaseAnalytics.LogEvent("event_ad_failed_to_load");*/

    public static void TutorialCompleted() { }
    public static void LevelCompleted(int levelNumber) { }
    public static void AdFailedToLoad() { }
}
