using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class HintSystem : MonoBehaviour
{
    public int CurrentStepIndex => _currentStepIndex + 1;
    public int StepsCount => steps.Count;

    private int _currentStepIndex;

    [SerializeField] private List<Step> steps;

    private const float IslandAnimationDuration = 0.3f;
    private const Ease IslandAnimationEase = Ease.OutCubic;

    private HintRenderer _hintsRenderer;

    private HintStepsRecorder _stepsRecorder;

    private SaveSystem<Data> _saveSystem;
    private Data _data = new Data();

    private Sequence _islandsAnimationSequence;

    public void Init(HintRenderer hintsRenderer, HintIslandFactory factory, HintUI hintUI) {
        _saveSystem = new SaveSystem<Data>(_data);
        _data = _saveSystem.LoadData() as Data;

        _hintsRenderer = hintsRenderer;
        _hintsRenderer.gameObject.SetActive(false);

        hintUI.Init(() => _data.IsAdViewed, () => _data.IsAdViewed = true);

        _currentStepIndex = -1;
        
        CreateHintIslands(factory);
    }

    private void CreateHintIslands(HintIslandFactory factory){
        List<Transform> islandsTransforms = IslandsContainer.GetIslandsTransforms(LevelContext.Instance.IslandsContainer.Islands);
        List<Transform> hintIslands = factory.GetHintIslands(islandsTransforms);

        for (int j = 0; j < hintIslands.Count; j++){
            for(int i = 0; i < steps.Count; i++){
                if(steps[i].IslandTransform == islandsTransforms[j]){
                    Step step = steps[i];
                    step.IslandTransform = hintIslands[j];
                    steps[i] = step;
                }
            }
        }

        _stepsRecorder = new HintStepsRecorder(hintIslands, steps.Count);
    }

    public void NextStep(){
        if(_currentStepIndex < steps.Count - 1){
            _currentStepIndex++;

            _islandsAnimationSequence?.Kill();

            if(_currentStepIndex > 0){
                Step prevStep = steps[_currentStepIndex - 1];
                if(prevStep.Rotatable)
                    prevStep.IslandTransform.eulerAngles = prevStep.IslandTargetCoordinates;
                else
                    prevStep.IslandTransform.localPosition = prevStep.IslandTargetCoordinates;
            }

            _stepsRecorder.RecordStep();
            UpdateLineRenderer(steps[_currentStepIndex]);
            AnimateIsland();
        }
    }

    public void PreviousStep(){
        if(_currentStepIndex > 0){
            _currentStepIndex--;

            _islandsAnimationSequence?.Kill();
            
            _stepsRecorder.MoveToPreviousStep();
            Timer.StartNew(this, StepsRecorder.IslandAnimationDuration, () => {
                UpdateLineRenderer(steps[_currentStepIndex]);
                AnimateIsland();
            });
        }
    }

    private void AnimateIsland(){
        Step step = steps[_currentStepIndex];
        Vector3 targetValue = step.IslandTargetCoordinates;

        _islandsAnimationSequence = DOTween.Sequence().SetLoops(-1);
        _islandsAnimationSequence.PrependInterval(0.2f);
        if(step.Rotatable)
            _islandsAnimationSequence.Append(step.IslandTransform.DOLocalRotate(targetValue, IslandAnimationDuration).SetEase(IslandAnimationEase));
        else
            _islandsAnimationSequence.Append(step.IslandTransform.DOLocalMove(targetValue, IslandAnimationDuration).SetEase(IslandAnimationEase));
        _islandsAnimationSequence.AppendInterval(0.5f);
    }

    private void UpdateLineRenderer(Step step){
        _hintsRenderer.HintLineRenderer.gameObject.SetActive(step.Rotatable == false);
        _hintsRenderer.HintLineRenderer.SetPositions(new Vector3[] { step.IslandTransform.localPosition, step.IslandTargetCoordinates });
    }

    public void OnHintPanelOpened(){
        if(_currentStepIndex == -1)
            NextStep();
            
        _hintsRenderer.gameObject.SetActive(true);
        _hintsRenderer.HintCamera.gameObject.SetActive(true);
    }
    public void OnHintPanelClosed(){
        _hintsRenderer.gameObject.SetActive(false);
        _hintsRenderer.HintCamera.gameObject.SetActive(false);
    }

    public class Data : ISaveable
    {
        public bool IsAdViewed;
        
        public string FileName => "hints_data";
    }

    [Serializable]
    public struct Step{
        public Transform IslandTransform;
        [Space]
        public bool Rotatable;
        [Space]
        public Vector3 IslandTargetCoordinates;
    }
}