using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using static DirectionExtensions;

[SelectionBase] [Serializable]
public class Island : MonoBehaviour
{
    public ComplexIsland Parent;

    [SerializeField] public IslandTypes IslandType;
    [Space]
    public bool isInputMirrored;
    [Space]
    [SerializeField] public Direction outputEnergyFlowDirection;
    [SerializeField] public Direction inputEnergyFlowDirection;

#if UNITY_EDITOR
    public string OutputPropertyName => nameof(outputEnergyFlowDirection);
    public string InputPropertyName => nameof(inputEnergyFlowDirection);
    public string UpdatingSoundPropertyName => nameof(updatingSound);
#endif

    [Header("Visual")]
    public MeshRenderer defaultRenderer;
    public MeshRenderer cornerRenderer;

    private MeshRenderer _currentRenderer;

    [SerializeField] protected AudioClip updatingSound;

    private bool isEnergyGoing;
    private Material _rendererMaterial;
    private Color _defaultEmissionColor;

    private void Awake() {
        _currentRenderer = defaultRenderer;
        if(IslandType == IslandTypes.Corner) _currentRenderer = cornerRenderer;
        _rendererMaterial = _currentRenderer.material;

        _defaultEmissionColor = _rendererMaterial.GetColor("_EmissionColor");

        if(IslandType == IslandTypes.Start){
            isEnergyGoing = true;
            EnergyIsGoing();
        }
        else{
            EnergyIsNotGoing();
        }
    }

    public virtual bool OnClick(Action onUpdated) => false;
    public virtual bool OnSwipe(Direction direction, Action onUpdated) => false;

    public bool TryGetNextIsland(out Island nextIsland){
        if(TryGetIslandInDirection(GetOutputDirection(), out Island island, true)){
            if(island.IslandType != IslandTypes.Empty){
                bool isCorrespond = CheckInputAndOutputConformity(GetOutputDirection(), island.GetInputDirection());
                if(isCorrespond){
                    if(isEnergyGoing)
                        island.EnergyIsGoing();
                    else
                        island.EnergyIsNotGoing();
                }

                nextIsland = island;
                return isCorrespond;
            }
        }

        nextIsland = null;
        return false;
    }

    public bool TryGetIslandInDirection(Direction direction, out Island island, bool includingParentIslands = false){
        Debug.DrawRay(transform.position, ConvertDirectionToVector(direction) * 6.5f, new Color(0, 166, 255), 2.5f);
        if(Physics.Raycast(transform.position, ConvertDirectionToVector(direction), out RaycastHit hit, 6.5f)){
            if(hit.transform.TryGetComponent<Island>(out Island hittedIsland)){
                island = hittedIsland;

                if(includingParentIslands == false){
                    if(Parent && Parent.ChildIslands.Contains(hittedIsland) == true){
                        return false;
                    }
                }

                Debug.DrawRay(transform.position, ConvertDirectionToVector(direction) * 6.5f, Color.red, 2.5f);
                return true;
            }
        }

        island = null;
        return false;
    }

    public bool TryGetWallInDirection(Direction direction, out Transform wall){
        if(Physics.Raycast(transform.position, ConvertDirectionToVector(direction), out RaycastHit hit, 6.5f)){
            if(hit.transform.tag == "Wall"){
                Debug.DrawRay(transform.position, ConvertDirectionToVector(direction) * 6.5f, Color.red, 2.5f);
                wall = hit.transform;
                return true;
            }
        }

        wall = null;
        return false;
    }

    public virtual bool AdditionalUpdatingCondition(Direction direction) => true;

    public void EnergyIsGoing() => SetEnergyGoingState(true);
    public void EnergyIsNotGoing() => SetEnergyGoingState(false);

    private void SetEnergyGoingState(bool isGoing){
        if(IslandType == IslandTypes.Empty)
            return;

        if(_rendererMaterial == null) Awake();
        isEnergyGoing = isGoing;
        _rendererMaterial.SetColor("_EmissionColor", isGoing ? _defaultEmissionColor : Color.black);
    }

    private bool CheckInputAndOutputConformity(Direction inputDirection, Direction outputDirection)
        => DirectionExtensions.GetMirroredDirection(inputDirection) == outputDirection;

    public Direction GetInputDirection(bool consideringRotation = true) => GetEnergyFlowDirection(inputEnergyFlowDirection, consideringRotation);
    public Direction GetOutputDirection(bool consideringRotation = true) => GetEnergyFlowDirection(outputEnergyFlowDirection, consideringRotation);

    private Direction GetEnergyFlowDirection(Direction direction, bool consideringRotation = true){
        if(consideringRotation == false)
            return direction;

        float angle = direction.ToDegrees() + transform.rotation.eulerAngles.y;
        return GetDirectionFromAngle(angle);
    }

    public Vector3 ConvertDirectionToVector(Direction direction){
        float angle = DirectionExtensions.ToDegrees(direction);
        return new Vector3(Mathf.Sin(angle / Mathf.Rad2Deg), 0, Mathf.Cos(angle / Mathf.Rad2Deg));
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if(IslandType == IslandTypes.Empty)
            return;
            
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + ConvertDirectionToVector(GetInputDirection()) * 3.27f, Vector3.one / 2);

        if(IslandType != IslandTypes.Finish){   
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position + ConvertDirectionToVector(GetOutputDirection()) * 3.27f, Vector3.one / 2);

            Vector3 vectorDirection = ConvertDirectionToVector(GetOutputDirection());
            Gizmos.DrawRay(transform.position, new Vector3(vectorDirection.x, 0, vectorDirection.z) * 3.25f);
        }
    }
#endif

    public enum IslandTypes{
        Default,
        Corner,
        Start,
        Finish,
        Empty
    }
}
