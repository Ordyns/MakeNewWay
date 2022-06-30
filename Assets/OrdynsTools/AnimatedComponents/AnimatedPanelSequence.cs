using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPanelSequence : MonoBehaviour
{
    [SerializeField] private AnimatedPanel[] sequence;

    [Header("Settings")]
    [SerializeField] private bool playOnStart;
    [Space]
    [SerializeField] private float playbackDelay;
    [SerializeField] private float delayBetweenPanels;

    private void Start() {
        if(playOnStart)
            StartCoroutine(AnimatePanelsCo(open: true));
    }

    public void OpenPanels() => StartCoroutine(AnimatePanelsCo(open: true));
    public void ClosePanels() => StartCoroutine(AnimatePanelsCo(open: false));

    public void ClosePanelsImmediatly(){
        foreach (AnimatedPanel panel in sequence){
            panel.CloseInstantly();
        }
    }

    private IEnumerator AnimatePanelsCo(bool open) {
        yield return new WaitForSecondsRealtime(playbackDelay);

        foreach (AnimatedPanel panel in sequence) {
            if(open) panel.Open();
            else panel.Close();
            yield return new WaitForSecondsRealtime(delayBetweenPanels);
        }
    }
}
