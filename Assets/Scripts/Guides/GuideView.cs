using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(CanvasGroup))]
public class GuideView : MonoBehaviour
{
    [field:SerializeField] public int TargetLevelNumber { get; private set; }
    [Space]
    [SerializeField] private GuideStep[] steps;

    private AnimatedPanel _continueButton;
    private CanvasGroup _canvasGroup;

    private int _currentGuideStepIndex;
    private GuideStep _currentGuideStep => steps[_currentGuideStepIndex];

    private System.Action<VideoClip> PlayVideoClip;
    

    public void StartGuide(AnimatedPanel continueButton, System.Action<VideoClip> playVideoClipAction){
        _continueButton = continueButton;
        PlayVideoClip = playVideoClipAction;

        _canvasGroup = GetComponent<CanvasGroup>();

        foreach(GuideStep step in steps){
            foreach(AnimatedPanel animatedPanel in step.AnimatedPanels){
                animatedPanel.gameObject.SetActive(true);
            }
        }

        MoveToStep(0);
    }

    public bool MoveToNextStep(){
        _currentGuideStepIndex++;

        if(_currentGuideStepIndex >= steps.Length)
            return false;
        
        MoveToStep(_currentGuideStepIndex);

        return true;
    }

    private void MoveToStep(int stepIndex){
        _continueButton.Close();

        GuideStep step = steps[stepIndex];

        if(stepIndex >= 1 && step.ClosePreviousSteps){
            foreach(AnimatedPanel animatedPanel in steps[stepIndex - 1].AnimatedPanels){
                animatedPanel.Close();
            }
        }

        foreach(AnimatedPanel animatedPanel in step.AnimatedPanels)
            animatedPanel.Open();

        if(step.VideoClip)
            PlayVideoClip(step.VideoClip);

        Timer.StartNew(this, _currentGuideStep.StepDuration, _continueButton.Open);
    }
        
    public void CloseGuide(){
        foreach(AnimatedPanel animatedPanel in steps[steps.Length - 1].AnimatedPanels)
            animatedPanel.Close();

        _canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutSine).OnComplete(() => gameObject.SetActive(false));
    }

    [System.Serializable]
    private struct GuideStep {
        public float StepDuration;
        public bool ClosePreviousSteps;
        [Space]
        public AnimatedPanel[] AnimatedPanels;

        [Header("Optional")]
        public VideoClip VideoClip;
    }
}
