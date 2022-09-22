using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HintStepsRecorder : StepsRecorder
{
    [SerializeField] private List<IslandsState> _islandsStates;
    private int _currentStepIndex = -1;

    public HintStepsRecorder(IslandsProvider islandsProvider) : base(islandsProvider) {}

    public HintStepsRecorder(List<Transform> islandsTransforms, int stepsCount, float animationDuration) : base(islandsTransforms, animationDuration) {
        _islandsStates = new List<IslandsState>(stepsCount);
    }

    public override void RecordStep(){
        _currentStepIndex++;

        if(_currentStepIndex <= _islandsStates.Count - 1)
            return;

        IslandsState islandsState = new IslandsState(new List<Vector3>(), new List<Vector3>());

        foreach(Transform island in IslandsTransforms){
            islandsState.IslandsPositions.Add(island.transform.localPosition);
            islandsState.IslandsRotations.Add(island.transform.localEulerAngles);
        }

        _islandsStates.Add(islandsState);

        InvokeStepRecorded();
    }

    public override void MoveToPreviousStep(){
        _currentStepIndex--;

        if (CanMoveToPrevStep()){
            IslandsState state = _islandsStates[_currentStepIndex];
            
            for(int i = 0; i < IslandsTransforms.Count; i++){
                IslandsTransforms[i].transform.DOLocalMove(state.IslandsPositions[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
                IslandsTransforms[i].transform.DOLocalRotate(state.IslandsRotations[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
            }
        }
    }

    public override bool CanMoveToPrevStep() => _islandsStates.Count > 0;
}