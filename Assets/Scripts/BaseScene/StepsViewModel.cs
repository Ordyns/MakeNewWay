using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsViewModel : ViewModel
{
    public int StartStepsCount { get; private set; }

    public ObservableProperty<int> StepsLeft { get; private set; } = new ObservableProperty<int>();
    public ObservableProperty<int> StepsForBonus { get; private set; } = new ObservableProperty<int>();

    public event Action CantUpdateIsland;
    
    public DelegateCommand MoveToPreviousStepCommand { get; private set; }

    private bool isBonusReceivedEarlier;

    public void Init(IslandsUpdater islandsUpdater, List<int> completedLevelsWithBonus, int loadedlevelNumber) {
        InitCommands();

        islandsUpdater.CantUpdateIsland += () => CantUpdateIsland?.Invoke();

        int currentLevel = loadedlevelNumber;
        isBonusReceivedEarlier = completedLevelsWithBonus.Contains(currentLevel);

        LevelSettings levelSettings = LevelContext.Instance.LevelSettings;
        StartStepsCount = StepsLeft.Value = levelSettings.Steps;
        StepsForBonus.Value = levelSettings.StepsForBonus;
    }

    private void InitCommands(){
        StepsRecorder stepsRecorder = LevelContext.Instance.StepsRecorder;
        MoveToPreviousStepCommand = new DelegateCommand(
            () => { 
                stepsRecorder.MoveToPreviousStep();
                MoveToPreviousStepCommand.InvokeCanExecuteChanged();
            },
            () => stepsRecorder.CanMoveToPrevStep()
        );

        stepsRecorder.StepRecorded += MoveToPreviousStepCommand.InvokeCanExecuteChanged;
    }

    public bool IsBonusReceived(){
        if(isBonusReceivedEarlier)
            return true;

        return StartStepsCount - StepsLeft == StepsForBonus;
    }
}
