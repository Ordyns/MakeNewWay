using UnityEngine;
using NaughtyAttributes;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance;

    public int Steps;
    public int StepsForBonus;

    [Header("CameraSettins")]
    public float CameraSize = 33f;
    [Space]
    public bool CustomCameraPosition;
    [ShowIf(nameof(CustomCameraPosition))] public Vector3 CameraPosition;

    private void Awake() => Instance = this;

    public bool isBonusRecieved(int stepsLeft) => Steps - stepsLeft == StepsForBonus;
}
