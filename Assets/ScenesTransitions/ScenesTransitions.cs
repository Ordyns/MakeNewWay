using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScenesTransitions : MonoBehaviour
{
    [SerializeField] private Color transitionsColor = Color.black;
    [Space]
    [SerializeField] private Transition transitionPanel;

    private void Awake() {
        transitionPanel.Init(transitionsColor);
    }

    public void CreateNewTransition(System.Action onInAnimationFinished){
        transitionPanel.gameObject.SetActive(true);
        transitionPanel.StartInAnimation();

        transitionPanel.InAnimationFinished = onInAnimationFinished;

        // if(transitionPanel != null){
            // CloseCurrentTransition();
        // }

        // Transition transition = Instantiate(transitionPanelPrefab);
        // transition.ChangeColor(transitionsColor);

        // transitionPanel = transition;
    }

    public void CloseCurrentTransition() => transitionPanel.Close();
}