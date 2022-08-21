using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StepsRecorder : MonoBehaviour
{
    public static StepsRecorder Instance { get; private set; }
    public event System.Action StepRecorded;

    private List<Transform> _islandsTransforms;
    private Stack<IslandsState> _islandsStates;

    private IslandsUpdater _islandsUpdater;
    private PathChecker _pathChecker;

    private const float IslandAnimationDuration = 0.1f;

    private void Awake() => Instance = this;

    private IEnumerator Start() {
        _islandsStates = new Stack<IslandsState>();

        Init();

        while(IslandsUpdater.Instance == null)
            yield return null;
    
        _islandsUpdater = IslandsUpdater.Instance;
        _islandsUpdater.IslandUpdating += RecordStep;
    }

    private void Init(){
        List<Island> islands = IslandsContainer.Instance.Islands;
        _islandsTransforms = new List<Transform>();

        List<Island> islandsWithoutParent = islands.FindAll(island => island.Parent == null);
        List<ComplexIsland> parents = new List<ComplexIsland>();

        for(int j = 0; j < islands.Count; j++){
            if(islands[j].Parent != null && parents.Contains(islands[j].Parent) == false)
                parents.Add(islands[j].Parent);
        }

        islandsWithoutParent.ForEach(island => _islandsTransforms.Add(island.transform));
        parents.ForEach(island => _islandsTransforms.Add(island.transform));
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

    public void GoToPreviousStep(){
        if(CanMoveToPrevStep() && _islandsUpdater.IsIslandUpdating == false){
            _islandsUpdater.ExternalUpdate(IslandAnimationDuration, _islandsUpdater.StepsLeft + 1);
            IslandsState state = _islandsStates.Pop();
            
            for(int i = 0; i < _islandsTransforms.Count; i++){
                _islandsTransforms[i].transform.DOLocalMove(state.IslandsPositions[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
                _islandsTransforms[i].transform.DOLocalRotate(state.IslandsRotations[i], IslandAnimationDuration).SetEase(Ease.OutCubic);
            }
        }
    }

    public bool CanMoveToPrevStep() => _islandsStates.Count > 0;
    
    [System.Serializable]
    public struct IslandsState{
        public IslandsState(List<Vector3> islandsPositions, List<Vector3> islandsRotations){
            IslandsPositions = islandsPositions;
            IslandsRotations = islandsRotations;
        }

        public List<Vector3> IslandsPositions;
        public List<Vector3> IslandsRotations;
    }
}