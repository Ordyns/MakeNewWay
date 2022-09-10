using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindableAnimatedButton : AnimatedButton
{
    private ICommand _bindedCommand;

    public void Bind(ICommand command){
        _bindedCommand = command;
        OnClick.AddListener(_bindedCommand.Execute);
        command.CanExecuteChanged += () => Interactable = command.CanExecute();
        Interactable = command.CanExecute();
    }

    public void Unbind(){
        _bindedCommand = null;
        OnClick.RemoveListener(_bindedCommand.Execute);
        _bindedCommand.CanExecuteChanged += OnCanExecuteChanged;
    }

    private void OnCanExecuteChanged(){
        Interactable = _bindedCommand.CanExecute();
    }
}