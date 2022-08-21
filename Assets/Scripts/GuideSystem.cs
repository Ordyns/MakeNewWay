using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class GuideSystem : MonoBehaviour
{
    public bool isGuideShowing { get; private set; }
    public event System.Action GuideFinished;

    public static GuideSystem Instance;

    [SerializeField] private List<Guide> guides;
    [Space]
    [SerializeField] private GameObject guidesParent;
    [SerializeField] private CanvasGroup background;
    [SerializeField] private AnimatedPanel continueButton;

    private VideoPlayer _videoPlayer;

    private Guide _currentGuide;
    private GuideStep _currentGuideStep => _currentGuide.Steps[_currentGuideStepIndex];
    private int _currentGuideStepIndex;

    private void Awake() => Instance = this;

    private void Start() {
        _videoPlayer = GetComponent<VideoPlayer>();
        isGuideShowing = false;

        foreach(Guide guide in guides){
            guide.Parent.transform.gameObject.SetActive(false);

            for(int j = 0; j < guide.Steps.Length; j++){
                foreach(AnimatedPanel animatedPanel in guide.Steps[j].AnimatedPanels){
                    animatedPanel.gameObject.SetActive(true);
                }
            }
        }

        background.alpha = 0;
        guidesParent.SetActive(false);

        Guide currentLevelGuide = guides.Find(guide => guide.TargetLevelNumber == ScenesLoader.Instance.LastLoadedLevelNumber);
        if(currentLevelGuide != null){
            bool guideCompleted = SaveSystem.Instance.Data.CompletedLevelsWithGuides.Contains(currentLevelGuide.TargetLevelNumber);
            if(guideCompleted == false){
                StartGuide(currentLevelGuide);
                return;
            }
        }

        GuideFinished?.Invoke();
    }

    private void StartGuide(Guide guide){
        isGuideShowing = true;
        _currentGuide = guide;
        _currentGuideStepIndex = -1;

        guidesParent.SetActive(true);
        guide.Parent.transform.gameObject.SetActive(true);

        background.DOFade(1, 0.25f).SetEase(Ease.InOutSine);

        GuideFinished += () => SaveSystem.Instance.Data.CompletedLevelsWithGuides.Add(_currentGuide.TargetLevelNumber);

        NextGuideStep();
    }

    public void NextGuideStep(){
        _currentGuideStepIndex++;
        
        continueButton.Close();
        _videoPlayer.Stop();

        if(_currentGuideStepIndex >= _currentGuide.Steps.Length){
            FinishGuide();
            return;
        }

        if(_currentGuideStepIndex >= 1 && _currentGuideStep.ClosePreviousSteps){
            foreach(AnimatedPanel animatedPanel in _currentGuide.Steps[_currentGuideStepIndex - 1].AnimatedPanels){
                animatedPanel.Close();
            }
        }

        foreach(AnimatedPanel animatedPanel in _currentGuideStep.AnimatedPanels)
            animatedPanel.Open();

        PlayVideo(_currentGuideStep.VideoClip);

        TimeOperations.CreateTimer(_currentGuideStep.StepDuration, null, () => continueButton.Open());
    }

    private void PlayVideo(VideoClip videoClip){
        if(videoClip == null)
            return;

        _videoPlayer.targetTexture.Release();
        _videoPlayer.clip = _currentGuideStep.VideoClip;
        _videoPlayer.Play();
    }

    private void FinishGuide(){
        isGuideShowing = false;
        GuideFinished?.Invoke();

        _currentGuide.Parent.DOFade(0, 0.2f).SetEase(Ease.InOutSine);
        background.DOFade(0, 0.2f).SetEase(Ease.InOutSine);
        TimeOperations.CreateTimer(0.2f, null, () => guidesParent.SetActive(false));
    }

    [System.Serializable]
    public class Guide{
        public int TargetLevelNumber;
        [Space]
        public CanvasGroup Parent;
        public GuideStep[] Steps;
    }

    [System.Serializable]
    public struct GuideStep{
        public float StepDuration;
        public bool ClosePreviousSteps;
        [Space]
        public AnimatedPanel[] AnimatedPanels;

        [Header("Optional")]
        public VideoClip VideoClip;
    }
}
