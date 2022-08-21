using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StepsRecorder : MonoBehaviour
{
    public static StepsRecorder Instance { get; private set; }
    
    public event System.Action StepRecorded;

    public int CurrentStep { get; private set; }

    private List<Transform> islandsTransforms;
    private List<IslandsState> _islandsStates;

    private PlayerInput _playerInput;

    void Awake() => Instance = this;

    private IEnumerator Start() {
        while(PlayerInput.Instance == null)
            yield return null;

        _playerInput = PlayerInput.Instance;

        _islandsStates = new List<IslandsState>();
        PathChecker.Instance.PathChecked += (pathCorrect) => RecordStep();

        List<Island> islands = IslandsContainer.Instance.Islands;
        islandsTransforms = new List<Transform>();

        List<Island> islandsWithoutParent = islands.FindAll(island => island.Parent == null);
        List<ComplexIsland> parents = new List<ComplexIsland>();

        for(int j = 0; j < islands.Count; j++){
            if(islands[j].Parent != null && parents.Contains(islands[j].Parent) == false)
                parents.Add(islands[j].Parent);
        }

        islandsWithoutParent.ForEach(island => islandsTransforms.Add(island.transform));
        parents.ForEach(island => islandsTransforms.Add(island.transform));

        CurrentStep = -1;
        RecordStep();
    }

    public void RecordStep(){
        CurrentStep++;

        IslandsState islandsState = new IslandsState(new List<Vector3>(), new List<Vector3>());

        foreach(Transform island in islandsTransforms){
            islandsState.IslandsPositions.Add(island.transform.localPosition);
            islandsState.IslandsRotations.Add(island.transform.localEulerAngles);
        }

        _islandsStates.Add(islandsState);
        StepRecorded?.Invoke();
    }

    public void GoToPreviousStep(){
        if(CurrentStep > 0 && !_playerInput.IsIslandUpdating){
            CurrentStep -= 1;
            _playerInput.AddStep();
            _playerInput.IsIslandUpdating = true;
            
            for(int i = 0; i < islandsTransforms.Count; i++){
                islandsTransforms[i].transform.DOLocalMove(_islandsStates[CurrentStep].IslandsPositions[i], 0.1f).SetEase(Ease.OutCubic);
                islandsTransforms[i].transform.DOLocalRotate(_islandsStates[CurrentStep].IslandsRotations[i], 0.1f).SetEase(Ease.OutCubic);
            }

            _islandsStates.Remove(_islandsStates[CurrentStep + 1]);

            TimeOperations.CreateTimer(0.1f, null, () => {
                _playerInput.IsIslandUpdating = false;
                PathChecker.Instance.ChechPathWithoutEvent();
            });
        }
    }

    public bool CanMoveToPrevStep(){
        return CurrentStep > 0;
    }

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