using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class HintSystem : MonoBehaviour
{
    public static HintSystem Instance;

    public event Action OnInitializationFinished;

    public int CurrentStepIndex { get; private set; }
    public int StepsCount => steps.Count;

    [SerializeField] private List<Step> steps;

    private const float ISLAND_ANIMATION_DURATION = 0.3f;
    private const Ease ISLAND_ANIMATION_EASE = Ease.OutCubic;

    private HintsRenderer _hintsRenderer;
    private List<Transform> _hintIslands = new List<Transform>();

    private List<IslandsStates> _islandsStatesAtSteps;

    private Coroutine _islandsAnimationRoutine;

    private void Awake() => Instance = this;

    private IEnumerator Start() {
        while(HintsRenderer.Instance == null)
            yield return null;

        _hintsRenderer = HintsRenderer.Instance;

        _islandsStatesAtSteps = new List<IslandsStates>();
        CurrentStepIndex = 1;
        
        SaveStartIslandsStates();
        PrecalculateIslandsStates();

        OnInitializationFinished();
    }

    [NaughtyAttributes.Button]
    public void Fix(){
        for(int i = 0; i< steps.Count; i++){
            Step step = steps[i];
            step.IslandTargetCoordinates += new Vector3(0, 0, 6.5f);
            steps[i] = step;
        }
    }

    private void SaveStartIslandsStates(){
        IslandsStates islandsStates = new IslandsStates(new List<IslandState>());

        foreach(Transform child in IslandsContainer.Instance.IslandsParent){
            if(child.gameObject.activeSelf == false)
                continue;

            Transform hintIsland = Instantiate(child.gameObject, _hintsRenderer.HintIslandsParent).transform;
            hintIsland.localScale = Vector3.one;
            islandsStates.Islands.Add(new IslandState(hintIsland));
            _hintIslands.Add(hintIsland);

            for(int i = 0; i < steps.Count; i++){
                if(steps[i].IslandTransform == child){
                    Step step = steps[i];
                    step.IslandTransform = hintIsland;
                    steps[i] = step;
                }
            }
        }
        
        _islandsStatesAtSteps.Add(islandsStates);
    }

    private void PrecalculateIslandsStates(){
        for(int i = 0; i < steps.Count; i++){
            IslandsStates islandsStates = new IslandsStates(_islandsStatesAtSteps.Last().Islands);
            int islandIndex = islandsStates.Islands.FindIndex(state => state.IslandTransform == steps[i].IslandTransform);
            var island = islandsStates.Islands[islandIndex];

            if(steps[i].Rotatable){
                island.Rotation = steps[i].IslandTargetCoordinates;
                island.Rotated = true;
            }
            else{
                island.LocalPosition = steps[i].IslandTargetCoordinates;
            }

            if(i > 0){
                int previousUpdatedIslandIndex = islandsStates.Islands.FindIndex(island => island.StateUpdated);
                IslandState previousUpdatedIsland = islandsStates.Islands[previousUpdatedIslandIndex];
                previousUpdatedIsland.StateUpdated = false;
                islandsStates.Islands[previousUpdatedIslandIndex] = previousUpdatedIsland;
            }

            island.StateUpdated = true;
            islandsStates.Islands[islandIndex] = island;
            _islandsStatesAtSteps.Add(islandsStates);
        }
    }

    public void NextStep(){
        if(CurrentStepIndex < steps.Count){
            CurrentStepIndex++;

            MoveToStep(CurrentStepIndex);
        }
    }

    public void PreviousStep(){
        if(CurrentStepIndex > 1){
            CurrentStepIndex--;

            MoveToStep(CurrentStepIndex);
        }
    }

    private void MoveToStep(int step){
        UpdateLineRenderer(step);

        AnimateAllIslandsToState(_islandsStatesAtSteps[CurrentStepIndex - 1], 0);

        if(_islandsAnimationRoutine != null) StopCoroutine(_islandsAnimationRoutine);
        _islandsAnimationRoutine = StartCoroutine(IslandsAnimationRoutine(step));
    }

    private void UpdateLineRenderer(int step){
        IslandState endState = _islandsStatesAtSteps[step].Islands.Find(island => island.StateUpdated);
        IslandState startState = _islandsStatesAtSteps[step - 1].Islands.Find(island => island.IslandTransform == endState.IslandTransform);
        
        _hintsRenderer.HintLineRenderer.gameObject.SetActive(endState.Rotated == false);
        _hintsRenderer.HintLineRenderer.SetPositions(new Vector3[] { startState.LocalPosition, endState.LocalPosition });
    }

    private IEnumerator IslandsAnimationRoutine(int step){
        while(_hintsRenderer.gameObject.activeSelf){
            AnimateAllIslandsToState(_islandsStatesAtSteps[CurrentStepIndex], ISLAND_ANIMATION_DURATION);
            yield return new WaitForSeconds(ISLAND_ANIMATION_DURATION + 0.5f);
                
            AnimateAllIslandsToState(_islandsStatesAtSteps[CurrentStepIndex - 1], 0);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void AnimateAllIslandsToState(IslandsStates islandsStates, float duration){
        for(int i = 0; i < islandsStates.Islands.Count; i++){
            IslandState islandState = islandsStates.Islands[i];

            islandState.IslandTransform.DOLocalMove(islandState.LocalPosition, duration).SetEase(ISLAND_ANIMATION_EASE);
            islandState.IslandTransform.DOLocalRotate(islandState.Rotation, duration).SetEase(ISLAND_ANIMATION_EASE);
        }
    }

    public void OnHintPanelOpened(){
        _hintsRenderer.gameObject.SetActive(true);
        MoveToStep(CurrentStepIndex);
    }

    public void OnHintPanelClosed(){
        _hintsRenderer.gameObject.SetActive(false);
    }

    [Serializable]
    public struct IslandState{
        public IslandState(Transform islandTransform){
            IslandTransform = islandTransform;
            Rotation = islandTransform.eulerAngles;
            LocalPosition = islandTransform.localPosition;
            StateUpdated = false;
            Rotated = false;
        }

        public Transform IslandTransform;
        public Vector3 Rotation;
        public Vector3 LocalPosition;

        public bool Rotated;
        public bool StateUpdated;
    }

    [Serializable]
    private struct IslandsStates{
        public IslandsStates(List<IslandState> islands){
            Islands = new List<IslandState>(islands);
        }

        public List<IslandState> Islands;
    }
}

[Serializable]
public struct Step{
    public Transform IslandTransform;
    [Space]
    public bool Rotatable;
    [Space]
    public Vector3 IslandTargetCoordinates;
}