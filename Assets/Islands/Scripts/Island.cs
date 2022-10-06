using System;
using UnityEngine;
using static DirectionExtensions;

[SelectionBase] [Serializable] [RequireComponent(typeof(IslandEnergy))]
public class Island : MonoBehaviour
{
    private ComplexIsland _parent;
    public ComplexIsland Parent { 
        get => _parent;
        set{
            if(_parent != null){
                Debug.LogError("Can't set parent because it's already defined");
                return;
            }

            _parent = value;
        }
    }

    [SerializeField] public IslandType Type;
    public bool IsEnergyIsland => _islandEnergy && (_islandEnergy.IsInputEnabled || _islandEnergy.IsOutputEnabled);

#if UNITY_EDITOR
    public string DefaultRendererFieldName = nameof(defaultRenderer);
    public string CornerRendererFieldName = nameof(cornerRenderer);
#endif

    [Header("Visual")]
    [SerializeField] private MeshRenderer defaultRenderer;
    [SerializeField] private MeshRenderer cornerRenderer;

    private MeshRenderer _currentRenderer;

    private IslandEnergy _islandEnergy;

    private void OnValidate() {
        _islandEnergy = GetComponent<IslandEnergy>();
    }

    private void Awake(){
        RendererUpdated();

        if (Type == IslandType.Start)
            AcivateEnergy();
        else
            DeactivateEnergy();
    }

    private void RendererUpdated(){
        _currentRenderer = Type == IslandType.Corner ? cornerRenderer : defaultRenderer;

        Material material = null;
        if((Application.isEditor && UnityEditor.EditorApplication.isPlaying) || Application.isEditor == false)
            material = _currentRenderer.material;
        
        bool isOutputEnabled = Type != IslandType.Finish && Type != IslandType.Empty;
        bool isInputEnabled = Type != IslandType.Start && Type != IslandType.Empty;
        _islandEnergy?.Init(material, isOutputEnabled, isInputEnabled);
    }

    public bool TryGetNextIsland(out Island nextIsland){
        if(_islandEnergy?.IsOutputEnabled == true && TryGetIslandInDirection(GetOutputDirection(), out Island island, true)){
            nextIsland = island;
            return true;
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

    public void AcivateEnergy(){
        if(IsEnergyIsland == false)
            return;

        _islandEnergy?.Activate();
    }

    public void DeactivateEnergy(){
        if(IsEnergyIsland == false || Type == IslandType.Start)
            return;

        _islandEnergy?.Deactivate();
    }

    public static bool IsInputAndOutputCorrespond(Direction inputDirection, Direction outputDirection)
        => DirectionExtensions.GetMirroredDirection(inputDirection) == outputDirection;

    public Direction GetInputDirection(bool consideringRotation = true) => _islandEnergy.GetInputDirection(consideringRotation);
    public Direction GetOutputDirection(bool consideringRotation = true) => _islandEnergy.GetOutputDirection(consideringRotation);

    protected Vector3 ConvertDirectionToVector(Direction direction){
        float angle = DirectionExtensions.ToDegrees(direction);
        return new Vector3(Mathf.Sin(angle / Mathf.Rad2Deg), 0, Mathf.Cos(angle / Mathf.Rad2Deg));
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if(Type == IslandType.Empty)
            return;
            
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + ConvertDirectionToVector(GetInputDirection()) * 3.27f, Vector3.one / 2);

        if(Type != IslandType.Finish){   
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position + ConvertDirectionToVector(GetOutputDirection()) * 3.27f, Vector3.one / 2);

            Vector3 vectorDirection = ConvertDirectionToVector(GetOutputDirection());
            Gizmos.DrawRay(transform.position, new Vector3(vectorDirection.x, 0, vectorDirection.z) * 3.25f);
        }
    }
#endif

    public enum IslandType
    {
        Default,
        Corner,
        Start,
        Finish,
        Empty
    }
}
