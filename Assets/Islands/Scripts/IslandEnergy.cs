using UnityEngine;

public class IslandEnergy : MonoBehaviour
{
    public bool IsActive { get; private set; }
    
    public bool IsOutputEnabled { get; private set; } = true;
    public bool IsInputEnabled { get; private set; } = true;

    [HideInInspector, SerializeField] private Direction outputEnergyFlowDirection;
    [HideInInspector, SerializeField] private Direction inputEnergyFlowDirection;

#if UNITY_EDITOR
    [HideInInspector, SerializeField] private bool isInputMirrored;
    public string IsInputMirroredFieldName => nameof(isInputMirrored);

    public string OutputFieldName => nameof(outputEnergyFlowDirection);
    public string InputFieldName => nameof(inputEnergyFlowDirection);
#endif

    private Material _rendererMaterial;
    private Color _defaultEmissionColor;

    public void Init(Material rendererMaterial, bool isOutputEnabled, bool isInputEnabled){
        if(Application.isEditor && UnityEditor.EditorApplication.isPlaying){
            _rendererMaterial = rendererMaterial;
            _defaultEmissionColor = _rendererMaterial.GetColor("_EmissionColor");
        }

        IsOutputEnabled = isOutputEnabled;
        IsInputEnabled = isInputEnabled;
    }

    public void Activate(){
        SetEnergyActive(true);
    }

    public void Deactivate(){
        SetEnergyActive(false);
    }

    private void SetEnergyActive(bool active){
        IsActive = active;
        _rendererMaterial.SetColor("_EmissionColor", active ? _defaultEmissionColor : Color.black);
    }

    public Direction GetInputDirection(bool consideringRotation = true) => GetEnergyFlowDirection(inputEnergyFlowDirection, consideringRotation);
    public Direction GetOutputDirection(bool consideringRotation = true) => GetEnergyFlowDirection(outputEnergyFlowDirection, consideringRotation);

    private Direction GetEnergyFlowDirection(Direction direction, bool consideringRotation = true){
        if(consideringRotation == false)
            return direction;

        float angle = direction.ToDegrees() + transform.rotation.eulerAngles.y;
        return DirectionExtensions.GetDirectionFromAngle(angle);
    }
}
