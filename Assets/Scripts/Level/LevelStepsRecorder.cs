using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelStepsRecorder : StepsRecorder
{
    private Stack<IslandsState> _islandsStates = new Stack<IslandsState>();

    public LevelStepsRecorder(IslandsProvider islandsProvider) : base(islandsProvider) { }
    public LevelStepsRecorder(List<Transform> islandsTransforms) : base(islandsTransforms) {}

    public override void RecordStep(){
        IslandsState islandsState = new IslandsState(new List<Vector3>(), new List<Vector3>());

        foreach(Transform island in IslandsTransforms){
            islandsState.IslandsPositions.Add(island.transform.localPosition);
            islandsState.IslandsRotations.Add(island.transform.localEulerAngles);
        }

        _islandsStates.Push(islandsState);
        InvokeStepRecorded();
    }

    public override void MoveToPreviousStep(){
        if (CanMoveToPrevStep()){
            IslandsState state = _islandsStates.Pop();
            
            for(int i = 0; i < IslandsTransforms.Count; i++){
                IslandsTransforms[i].transform.DOLocalMove(state.IslandsPositions[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
                IslandsTransforms[i].transform.DOLocalRotate(state.IslandsRotations[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
            }
        }
    }

    public override bool CanMoveToPrevStep() => _islandsStates.Count > 0;
}
