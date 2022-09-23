using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class CameraAnimator : MonoBehaviour
{
    [field:Header("Size")]
    [field:SerializeField] public float SizeOffset { get; private set; } = 5;
    public float Duration => duration;

    [field:SerializeField] public float OriginalSize { get; private set; }
    
    [field:Header("Animation")]
    [SerializeField] private bool playOnStart;
    [Space]
    [SerializeField] private float inAnimationDelay = 0.3f;
    [Space]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Camera _camera;

    private void OnValidate() {
        _camera = GetComponent<Camera>();
    }

    private void Start() {
        if(playOnStart == false)
            return;
        
        _camera.orthographicSize = OriginalSize + SizeOffset;
        PlayInAnimation();
    }

    public void SetOriginalSize(float newSize){
        OriginalSize = Mathf.Clamp(newSize, 0, Mathf.Infinity);
    }

    [NaughtyAttributes.Button] public void PlayInAnimation() => _camera.DOOrthoSize(OriginalSize, duration).SetEase(ease).SetDelay(inAnimationDelay);
    [NaughtyAttributes.Button] public void PlayOutAnimation(float delay = 0) => _camera.DOOrthoSize(OriginalSize + SizeOffset, duration).SetEase(ease).SetDelay(delay);
}