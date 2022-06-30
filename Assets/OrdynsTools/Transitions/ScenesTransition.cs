using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScenesTransition : MonoBehaviour
{
    private static ScenesTransition _instance;

    [SerializeField] private Color transitionsColor = Color.black;
    [SerializeField] private Transition transitionPanelPrefab;

    private Transition _transitionPanel;

    private void Awake() => _instance = this;

    public static Transition CreateTransition(System.Action onStartScenesLoading){
        if(_instance._transitionPanel != null){
            CloseCurrentTransition();
        }

        Transition transition = Instantiate(_instance.transitionPanelPrefab);
        transition.OnStartTransitionFinished = onStartScenesLoading;
        transition.ChangeColor(_instance.transitionsColor);
        DontDestroyOnLoad(transition.gameObject);

        _instance._transitionPanel = transition;
        return transition;
    }

    public static void CloseCurrentTransition() => _instance._transitionPanel.Close();
}