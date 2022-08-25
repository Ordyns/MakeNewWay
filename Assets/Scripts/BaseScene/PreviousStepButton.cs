using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousStepButton : AnimatedButton
{
    [Header("ViewModel")]
    [SerializeField] private StepsViewModel stepsViewModel;

    private void Start() {
        OnClick.AddListener(() => stepsViewModel.MoveToPreviousStepCommand.Execute(new object()));
        stepsViewModel.MoveToPreviousStepCommand.CanExecuteChanged += UpdateInteractivity;
        UpdateInteractivity();
    }

    private void UpdateInteractivity(){
        Interactable = stepsViewModel.MoveToPreviousStepCommand.CanExecute(new object());
    }
}