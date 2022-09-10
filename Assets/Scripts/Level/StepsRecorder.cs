using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class StepsRecorder
{
    public event System.Action StepRecorded;
    public event System.Action MovedToPreviousStep;

    private List<Transform> _islandsTransforms = new List<Transform>();
    private Stack<IslandsState> _islandsStates = new Stack<IslandsState>();

    public const float IslandAnimationDuration = 0.1f;

    public StepsRecorder(List<Island> islands){
        HashSet<ComplexIsland> parents = new HashSet<ComplexIsland>();

        for(int j = 0; j < islands.Count; j++){
            if(islands[j].Parent != null && parents.Contains(islands[j].Parent) == false)
                _islandsTransforms.Add(islands[j].Parent.transform);
            else if(islands[j].Parent == null)
                _islandsTransforms.Add(islands[j].transform);
        }
    }

    public void RecordStep(){
        IslandsState islandsState = new IslandsState(new List<Vector3>(), new List<Vector3>());

        foreach(Transform island in _islandsTransforms){
            islandsState.IslandsPositions.Add(island.transform.localPosition);
            islandsState.IslandsRotations.Add(island.transform.localEulerAngles);
        }

        _islandsStates.Push(islandsState);
        StepRecorded?.Invoke();
    }

    public void MoveToPreviousStep(){
        if(CanMoveToPrevStep()){
            IslandsState state = _islandsStates.Pop();
            
            for(int i = 0; i < _islandsTransforms.Count; i++){
                _islandsTransforms[i].transform.DOLocalMove(state.IslandsPositions[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
                _islandsTransforms[i].transform.DOLocalRotate(state.IslandsRotations[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
            }
        }
    }

    public bool CanMoveToPrevStep() => _islandsStates.Count > 0;
    
    [System.Serializable]
    private struct IslandsState{
        public IslandsState(List<Vector3> islandsPositions, List<Vector3> islandsRotations){
            IslandsPositions = islandsPositions;
            IslandsRotations = islandsRotations;
        }

        public List<Vector3> IslandsPositions;
        public List<Vector3> IslandsRotations;
    }
}