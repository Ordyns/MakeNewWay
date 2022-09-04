using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsViewModel : ViewModel
{
    public int StartStepsCount { get; private set; }

    public ObservableProperty<int> StepsLeft { get; private set; } = new ObservableProperty<int>();
    public ObservableProperty<int> StepsForBonus { get; private set; } = new ObservableProperty<int>();

    public DelegateCommand<object> MoveToPreviousStepCommand { get; private set; }

    private bool isBonusReceivedEarlier;

    public void Init(List<int> completedLevelsWithBonus) {
        InitCommands();

        int currentLevel = ProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber;
        isBonusReceivedEarlier = completedLevelsWithBonus.Contains(currentLevel);

        LevelSettings levelSettings = LevelContext.Instance.LevelSettings;
        StartStepsCount = StepsLeft.Value = levelSettings.Steps;
        StepsForBonus.Value = levelSettings.StepsForBonus;
    }

    private void InitCommands(){
        StepsRecorder stepsRecorder = LevelContext.Instance.StepsRecorder;
        MoveToPreviousStepCommand = new DelegateCommand<object>(
            (parameter) => { 
                stepsRecorder.MoveToPreviousStep();
                MoveToPreviousStepCommand.InvokeCanExecuteChanged();
            },
            (parameter) => stepsRecorder.CanMoveToPrevStep()
        );

        stepsRecorder.StepRecorded += MoveToPreviousStepCommand.InvokeCanExecuteChanged;
    }

    public bool IsBonusReceived(){
        if(isBonusReceivedEarlier)
            return true;

        return StartStepsCount - StepsLeft == StepsForBonus;
    }
}
