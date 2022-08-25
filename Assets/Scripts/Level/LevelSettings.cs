using UnityEngine;
using NaughtyAttributes;

public class LevelSettings : MonoBehaviour
{
    [field:Header("Steps")]
    [field:SerializeField] public int Steps { get; private set; }
    [field:SerializeField] public int StepsForBonus { get; private set; }

    [field:Header("CameraSettings")]
    [field:SerializeField] public float CameraSize { get; private set; } = 33f;
    [field:Space]
    [field:SerializeField] public bool CustomCameraPosition { get; private set; }
    [field:SerializeField] [field:ShowIf(nameof(CustomCameraPosition))] public Vector3 CameraPosition { get; private set; }

    public bool IsBonusRecieved(int stepsLeft) => Steps - stepsLeft == StepsForBonus;
}
