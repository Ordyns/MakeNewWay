using UnityEngine;
using NaughtyAttributes;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance;

    [field:SerializeField] public int Steps { get; set; }
    [field:SerializeField] public int StepsForBonus { get; set; }

    [field:Header("CameraSettins")]
    [field:SerializeField] public float CameraSize { get; set; } = 33f;
    [field:Space]
    [field:SerializeField] public bool CustomCameraPosition { get; set; }
    [field:SerializeField] [field:ShowIf(nameof(CustomCameraPosition))] public Vector3 CameraPosition { get; set; }

    private void Awake() => Instance = this;

    public bool IsBonusRecieved(int stepsLeft) => Steps - stepsLeft == StepsForBonus;
}
