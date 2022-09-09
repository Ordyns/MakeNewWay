using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLoadedHandler : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private BaseSoundsPlayer soundsPlayer;
    [Space]
    [SerializeField] private BaseCamera baseCamera;
    [SerializeField] private LevelSettings levelSettings;
    [Space]
    [SerializeField] private StepsViewModel stepsViewModel;
    [SerializeField] private IslandsUpdater islandsUpdater;
    [Space]
    [SerializeField] private PathChecker pathChecker;
    [SerializeField] private IslandsContainer islandsContainer;

    private void Start() {
        pathChecker.Init(islandsContainer.Islands);
        stepsViewModel.InitOnlySteps(levelSettings.Steps);

        InitIslandsUpdater();
        InitBaseCamera();
        InitTutorial();
    }

    private void InitIslandsUpdater(){
        islandsUpdater.IsIslandsUpdatingAllowed = false;
        islandsUpdater.IslandUpdated += pathChecker.CheckPath;
        islandsUpdater.Init(stepsViewModel, new PauseManager());
    }

    private void InitBaseCamera(){
        baseCamera.Init(levelSettings);
        baseCamera.PlayInAnimation();
    }

    private void InitTutorial(){
        tutorial.TutorialFinished += OnTutorialFinished;
        tutorial.Init(islandsUpdater, baseCamera);
        Timer.StartNew(this, baseCamera.AnimationDuration, tutorial.BeginTutorial);
    }

    private void OnTutorialFinished(){
        Analytics.TutorialCompleted();
        baseCamera.PlayOutAnimation();
        Timer.StartNew(this, baseCamera.AnimationDuration, () => soundsPlayer.PlayLevelCompletedSound());
    }
}
