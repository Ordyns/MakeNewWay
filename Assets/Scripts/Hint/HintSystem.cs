using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class HintSystem : MonoBehaviour
{
    public int CurrentStepIndex { get; private set; }
    public int StepsCount => steps.Count;

    [SerializeField] private List<Step> steps;

    private const float IslandAnimationDuration = 0.3f;
    private const Ease IslandAnimationEase = Ease.OutCubic;

    private HintRenderer _hintsRenderer;

    private HintStepsRecorder _stepsRecorder;

    private Sequence _islandsAnimationSequence;

    [Zenject.Inject]
    private void Init(LevelHintSteps hintSteps, HintRenderer hintsRenderer, HintIslandFactory factory) {
        steps = hintSteps.GetSteps();

        _hintsRenderer = hintsRenderer;
        _hintsRenderer.Deactivate();

        CurrentStepIndex = -1;
        
        CreateHintIslands(factory);
    }

    private void CreateHintIslands(HintIslandFactory factory){
        List<Transform> hintIslands = factory.GetHintIslands();
        List<Transform> orignalIslands = factory.GetOriginalIslands();

        for (int j = 0; j < hintIslands.Count; j++){
            for(int i = 0; i < steps.Count; i++){
                if(steps[i].IslandTransform == orignalIslands[j]){
                    Step step = steps[i];
                    step.IslandTransform = hintIslands[j];
                    steps[i] = step;
                }
            }
        }

        _stepsRecorder = new HintStepsRecorder(hintIslands, steps.Count, 0f);
    }

    public void NextStep(){
        if(CanMoveToNextStep()){
            CurrentStepIndex++;

            _islandsAnimationSequence?.Kill();

            if(CurrentStepIndex > 0){
                Step prevStep = steps[CurrentStepIndex - 1];
                if(prevStep.Rotatable)
                    prevStep.IslandTransform.eulerAngles = prevStep.IslandTargetCoordinates;
                else
                    prevStep.IslandTransform.localPosition = prevStep.IslandTargetCoordinates;
            }

            _stepsRecorder.RecordStep();
            UpdateLineRenderer(steps[CurrentStepIndex]);
            AnimateIsland();
        }
    }

    public void PreviousStep(){
        if(CanMoveToPreviousStep()){
            CurrentStepIndex--;

            _islandsAnimationSequence?.Kill();
            
            _stepsRecorder.MoveToPreviousStep();
            Timer.StartNew(this, _stepsRecorder.IslandAnimationDuration, () => {
                UpdateLineRenderer(steps[CurrentStepIndex]);
                AnimateIsland();
            });
        }
    }

    public bool CanMoveToNextStep() => CurrentStepIndex < steps.Count - 1;
    public bool CanMoveToPreviousStep() => CurrentStepIndex > 0;

    private void AnimateIsland(){
        Step step = steps[CurrentStepIndex];
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
        _hintsRenderer.UpdateLineRenderer(step.Rotatable == false, step.IslandTransform.localPosition, step.IslandTargetCoordinates);
    }

    public void ActivateHint(){
        if(CurrentStepIndex == -1)
            NextStep();
            
        _hintsRenderer.Activate();
    }
    public void DeactivateHint(){
        _hintsRenderer.Deactivate();
    }

    [Serializable]
    public struct Step
    {
        public Transform IslandTransform;
        [Space]
        public bool Rotatable;
        [Space]
        public Vector3 IslandTargetCoordinates;
    }
}