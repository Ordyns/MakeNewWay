using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public event System.Action TutorialFinished;

    [SerializeField] private Step[] steps;
    private Step _currentStep{ 
        get{
            if(_currentStepIndex >= 0 && isTutorialCompleted == false) return steps[_currentStepIndex]; 
            else return new Step();
        }
    }
    private int _currentStepIndex;

    [HorizontalLine]
    [SerializeField] private Transform tutorialCompletedPanel;
    [HorizontalLine]
    [SerializeField] private RectTransform canvas;
    [SerializeField] private CanvasGroup background;
    [SerializeField] private CanvasGroup hand;
    [SerializeField] private LineRenderer swipeDirectionLine;
    [Space]
    [SerializeField] private CanvasGroup moveIslandUI;
    [Space]
    [SerializeField] private Vector3 islandPivotOffset = new Vector3(0, 1.45f, 0);

    private BaseCamera _baseCamera;
    private IslandsUpdater _islandsUpdater;

    private bool isTutorialCompleted;

    private Sequence _handAnimationSequence;
    private Coroutine _handAnimationRoutine;

    private SaveSystem<PlayerData> _saveSystem;
    private PlayerData _data = new PlayerData();

    public void Init(IslandsUpdater islandsUpdater, BaseCamera baseCamera){
        _saveSystem = new SaveSystem<PlayerData>(_data);
        _data = _saveSystem.LoadData();
        
        _baseCamera = baseCamera;
        _islandsUpdater = islandsUpdater;
        
        _currentStepIndex = -1;
        
        moveIslandUI.alpha = 0;
        SetBackgroundVisibility(false);
        
        swipeDirectionLine.positionCount = 2;
        swipeDirectionLine.SetPositions(new Vector3[]{ Vector3.zero, Vector3.zero });
        
        _islandsUpdater.IslandUpdated += OnIslandUpdated;
    }

    public void BeginTutorial(){
        NextStep();
    }

    private void Update() {
        if(isTutorialCompleted || _currentStepIndex < 0 || _islandsUpdater.IsIslandUpdating)
            return;

        if(_currentStep.Type == StepType.Info){
            if(Input.GetMouseButtonDown(0))
                NextStep();

            return;
        }
        else{
            if(Input.GetMouseButtonDown(0))
                SetMoveIslandUIVisibility(false);
            else if(Input.GetMouseButtonUp(0))
                SetMoveIslandUIVisibility(true);
        }
    }

    public void NextStep(){
        ResetUIAfterPreviousStep();

        _currentStepIndex++;
        
        if(_currentStepIndex >= steps.Length){
            FinishTutorial();
            return;
        }

        if(_currentStep.Type == StepType.Info){
            Transform infoPanelTransform = _currentStep.InfoPanelContent.Transform;
            infoPanelTransform.gameObject.SetActive(true);
            infoPanelTransform.localPosition = WorldToScreenPoint(_currentStep.InfoPanelContent.AnchorIsland.transform.position + islandPivotOffset);
            infoPanelTransform.localScale = Vector3.zero;

            infoPanelTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutCubic);

            _islandsUpdater.IsIslandsUpdatingAllowed = false;
        }
        else if(_currentStep.Type == StepType.MoveIsland){
            Step step = _currentStep;
            step.IslandScreenPosition = WorldToScreenPoint(_currentStep.MoveIslandContent.IslandTransform.position + islandPivotOffset);
            step.TargetScreenPosition = WorldToScreenPoint(_currentStep.MoveIslandContent.TargetIslandPositionTransform.position + islandPivotOffset);
            steps[_currentStepIndex] = step;

            swipeDirectionLine.SetPositions(new Vector3[] { step.IslandScreenPosition, step.TargetScreenPosition });

            _handAnimationRoutine = StartCoroutine(HandAnimation());
            SetMoveIslandUIVisibility(true);

            _islandsUpdater.IsIslandsUpdatingAllowed = true;
        }
    }

    private void ResetUIAfterPreviousStep(){
        if(_currentStepIndex >= 0){
            if(_currentStep.Type == StepType.Info){
                Transform infoPanelTransform = _currentStep.InfoPanelContent.Transform;
                infoPanelTransform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutCubic).OnComplete(() => {
                    infoPanelTransform.gameObject.SetActive(false);
                });
            }
        }

        moveIslandUI.alpha = 0;
        background.DOFade(1, 0.25f);

        if(_handAnimationRoutine != null){
            StopCoroutine(_handAnimationRoutine);
            _handAnimationSequence.Kill();
        }
    }

    private void FinishTutorial(){
        isTutorialCompleted = true;
        _islandsUpdater.IsIslandsUpdatingAllowed = false;

        _data.TutorialCompleted = true;
        _saveSystem.SaveData(_data);

        swipeDirectionLine.positionCount = 0;
        _baseCamera.PlayOutAnimation();
        Timer.StartNew(this, _baseCamera.AnimationDuration, () => {
            tutorialCompletedPanel.gameObject.SetActive(true);
        });

        TutorialFinished?.Invoke();
    }

    private void OnIslandUpdated(){
        if(_currentStep.Type == StepType.MoveIsland)
            NextStep();
    }

    private IEnumerator HandAnimation(){
        while(isTutorialCompleted == false){
            hand.transform.localPosition = _currentStep.IslandScreenPosition;
            _handAnimationSequence.Append(hand.DOFade(1, 0.15f));
            yield return new WaitForSeconds(0.15f);

            _handAnimationSequence = DOTween.Sequence();
            _handAnimationSequence.Append(hand.transform.DOLocalMove(_currentStep.TargetScreenPosition, 0.5f).SetEase(Ease.InOutSine));
            _handAnimationSequence.Append(hand.DOFade(0, 0.25f).SetEase(Ease.InOutSine));
            yield return new WaitForSeconds(1.5f);
        }
    }

    public void LoadMenu() => ProjectContext.Instance.ScenesLoader.LoadMenu();
    public void LoadFirstLevel() => ProjectContext.Instance.ScenesLoader.LoadLevel(1);

    private Vector2 WorldToScreenPoint(Vector3 position){
        Vector2 adjustedPosition = Camera.main.WorldToScreenPoint(position);
        adjustedPosition.x *= canvas.rect.width / (float)Camera.main.pixelWidth;
        adjustedPosition.y *= canvas.rect.height / (float)Camera.main.pixelHeight;

        return adjustedPosition - canvas.sizeDelta / 2f;
    }

    private void AnimateInfoPanel(InfoPanel infoPanel, Vector3 size){
        infoPanel.Transform.DOScale(size, 0.25f).SetEase(Ease.OutCubic);
    }

    private void SetBackgroundVisibility(bool visible) => background.DOFade(visible ? 1 : 0, 0.15f);
    private void SetMoveIslandUIVisibility(bool visible){
        moveIslandUI.DOFade(visible ? 1 : 0, 0.25f);
        swipeDirectionLine.gameObject.SetActive(visible);

        SetBackgroundVisibility(visible);
    }

    [System.Serializable]
    public struct Step{
        public StepType Type;
        [Space]
        public InfoPanel InfoPanelContent;
        public MoveIslandContent MoveIslandContent;

        [HideInInspector] public Vector3 IslandScreenPosition;
        [HideInInspector] public Vector3 TargetScreenPosition;
    }

    [System.Serializable]
    public struct InfoPanel{
        public Transform Transform;
        public Transform AnchorIsland;
    }

    [System.Serializable]
    public struct MoveIslandContent{
        public Transform IslandTransform;
        public Transform TargetIslandPositionTransform;
    }

    public enum StepType{
        Info,
        MoveIsland
    }
}
