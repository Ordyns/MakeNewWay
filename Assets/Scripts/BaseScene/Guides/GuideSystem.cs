using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;
using System;

[RequireComponent(typeof(VideoPlayer))]
public class GuideSystem : MonoBehaviour
{
    public bool IsGuideShowing { get; private set; }
    public event System.Action GuideFinished;
    
    [SerializeField] private CanvasGroup background;
    [SerializeField] private AnimatedPanel continueButton;
    [field:Space]
    [SerializeField] private Transform guideViewsParent;
    [field:SerializeField] public GuideView[] GuideViews { get; private set; }

    private VideoPlayer _videoPlayer;

    private GuideView _currentGuideView;
    private SaveSystem _saveSystem;

    private void Start() {
        _videoPlayer = GetComponent<VideoPlayer>();
        _saveSystem = ProjectContext.Instance.SaveSystem;

        IsGuideShowing = false;

        foreach(GuideView guideView in GuideViews){
            if(guideView.TargetLevelNumber == ProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber)
                _currentGuideView = guideView;

            guideView.gameObject.SetActive(false);
        }

        background.alpha = 0;
        background.gameObject.SetActive(false);
        
        if(_currentGuideView && _saveSystem.Data.CompletedLevelsWithGuides.Contains(_currentGuideView.TargetLevelNumber) == false)
            StartGuide(_currentGuideView);
        else
            GuideFinished?.Invoke();
    }

    private void StartGuide(GuideView guideView){
        IsGuideShowing = true;
        guideViewsParent.gameObject.SetActive(true);

        _currentGuideView.gameObject.SetActive(true);
        _currentGuideView.StartGuide(continueButton, PlayGuideVideo);
        
        GuideFinished += () => _saveSystem.Data.CompletedLevelsWithGuides.Add(_currentGuideView.TargetLevelNumber);

        background.gameObject.SetActive(true);
        background.DOFade(1, 0.25f).SetEase(Ease.InOutSine);
    }

    public void MoveToNextGuideStep(){
        _videoPlayer.Stop();
        IsGuideShowing = true;

        if(_currentGuideView.MoveToNextStep() == false)
            FinishGuide();
    }

    private void FinishGuide(){
        _currentGuideView.CloseGuide();
        continueButton.Close();
        background.DOFade(0, 0.2f).SetEase(Ease.InOutSine);

        IsGuideShowing = false;
        GuideFinished?.Invoke();
    }

    public void PlayGuideVideo(VideoClip videoClip){
        if(videoClip == null)
            return;

        _videoPlayer.targetTexture.Release();
        _videoPlayer.clip = videoClip;
        _videoPlayer.Play();
    }
}
